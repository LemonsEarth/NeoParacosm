
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.Localization;

namespace NeoParacosm.Content.Projectiles.Interactable;

public class ResearcherNote : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    Vector2 startPos = Vector2.Zero;
    bool fadeOut = false;

    private static Asset<Texture2D> highlightTexture;

    public override void Load()
    {
        highlightTexture = Request<Texture2D>(Texture + "_Highlight");
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.IsInteractable[Type] = true; // Facilitates smart cursor support
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true; // Necessary for non-held projectiles using Projectile.hide
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 99999999; // Stays active for 3 minutes, or 3 * 60 * 60 game updates
        Projectile.hide = true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            startPos = Projectile.Center;
        }

        if (!fadeOut)
        {
            Projectile.timeLeft = 120;
        }
        else
        {
            if (Projectile.timeLeft == 119)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.NeoParacosm.NPCs.Researcher.ResearcherNoteMessage"), Color.Gold);
                }
            }
            Projectile.Opacity = Projectile.timeLeft / 120f;
        }
        Projectile.velocity = Vector2.Zero;
        Projectile.Center = startPos + Vector2.UnitY * MathF.Sin(AITimer / 30f) * 16;
        Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = true;
        AITimer++;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index); // This projectile draws behind other projectiles to not be in the way.
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, 1f);
        return true;
    }


    // sum bullshit
    public override void PostDraw(Color lightColor)
    {
        // We use PostDraw to draw the highlight texture over the normal texture.

        // This logic replicates the vanilla projectile drawing logic:
        Asset<Texture2D> texture = TextureAssets.Projectile[Type];
        int offsetY = 0;
        int offsetX = 0;
        float originX = (texture.Width() - Projectile.width) * 0.5f + Projectile.width * 0.5f;
        ProjectileLoader.DrawOffset(Projectile, ref offsetX, ref offsetY, ref originX);
        int frameHeight = texture.Height() / Main.projFrames[Type];
        int frameY = frameHeight * Projectile.frame;
        SpriteEffects drawEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // TryInteracting return values:
        // 0: Not highlighted, 1: draw faded highlight, 2: draw selected highlight selected
        int highlightTextureDrawMode = TryInteracting();
        if (highlightTextureDrawMode == 0)
        {
            // If not in range, or if smart cursor is off, we don't draw the highlight texture at all.
            return;
        }

        int lightValue = (lightColor.R + lightColor.G + lightColor.B) / 3;
        if (lightValue > 10)
        {
            bool isProjectileSelected = highlightTextureDrawMode == 2;
            Color selectionGlowColor = Colors.GetSelectionGlowColor(isProjectileSelected, lightValue);
            Main.EntitySpriteDraw(
                highlightTexture.Value,
                new Vector2(Projectile.position.X - Main.screenPosition.X + originX + offsetX, Projectile.position.Y - Main.screenPosition.Y + (Projectile.height / 2) + Projectile.gfxOffY),
                texture.Frame(1, 2, 0, 0),
                selectionGlowColor,
                Projectile.rotation,
                new Vector2(originX, Projectile.height / 2 + offsetY),
                1f,
                drawEffects
            );
        }
    }

    // This method handles interacting with this projectile and also returns a value indicating how the highlight texture should be drawn.
    private int TryInteracting()
    {
        if (fadeOut)
        {
            return 0;
        }
        if (Main.gamePaused || Main.gameMenu)
        {
            return 0;
        }

        bool cursorHighlights = Main.SmartCursorIsUsed || PlayerInput.UsingGamepad;
        Player localPlayer = Main.LocalPlayer;
        Vector2 compareSpot = localPlayer.Center;
        if (!localPlayer.IsProjectileInteractibleAndInInteractionRange(Projectile, ref compareSpot))
        {
            return 0;
        }

        // Due to a quirk in how projectiles drawn using behindProjectiles are implemented, we need to do some math to calculate the correct world position of the mouse instead of using Main.MouseWorld directly.
        Matrix matrix = Matrix.Invert(Main.GameViewMatrix.ZoomMatrix);
        Vector2 position = Main.ReverseGravitySupport(Main.MouseScreen);
        Vector2.Transform(Main.screenPosition, matrix);
        Vector2 realMouseWorld = Vector2.Transform(position, matrix) + Main.screenPosition;

        bool mouseDirectlyOver = Projectile.Hitbox.Contains(realMouseWorld.ToPoint());
        bool interactingWithThisProjectile = mouseDirectlyOver || Main.SmartInteractProj == Projectile.whoAmI;
        if (!interactingWithThisProjectile || localPlayer.lastMouseInterface)
        {
            if (cursorHighlights)
            {
                return 1; // Draw faded highlight texture
            }
            else
            {
                return 0; // Don't draw highlight texture
            }
        }

        Main.HasInteractibleObjectThatIsNotATile = true;
        if (mouseDirectlyOver)
        {
            localPlayer.noThrow = 2;
            // Show the corresponding item icon on the cursor when directly over the interactable projectile.
            localPlayer.cursorItemIconEnabled = true;
            localPlayer.cursorItemIconID = -1;
        }

        if (PlayerInput.UsingGamepad)
        {
            localPlayer.GamepadEnableGrappleCooldown();
        }
        if (Main.mouseRight && Main.mouseRightRelease && Player.BlockInteractionWithProjectiles == 0)
        {
            Main.mouseRightRelease = false;
            localPlayer.tileInteractAttempted = true;
            localPlayer.tileInteractionHappened = true;
            localPlayer.releaseUseTile = false;

            // This is where custom interaction logic would go. This example simply plays a sound.
            SoundEngine.PlaySound(SoundID.Chat, Projectile.Center);
            fadeOut = true;
        }

        if (cursorHighlights)
        {
            return 2; // Draw highlight texture
        }
        else
        {
            return 0;
        }
    }
}
