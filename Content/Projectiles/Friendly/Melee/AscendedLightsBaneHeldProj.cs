using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class AscendedLightsBaneHeldProj : ModProjectile
{
    int AITimer = 0;
    ref float speedBoost => ref Projectile.ai[0];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!AscendedLightsBane.hitNPCs.Contains(target) && AscendedLightsBane.hitNPCs.Count < 3)
        {
            AscendedLightsBane.hitNPCs.Add(target);
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 54;
        Projectile.height = 54;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    float goalRotation = 270;
    float lerpT = 8f / 60f;
    float rotValue;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        Projectile.timeLeft = 3;
        Projectile.velocity = Vector2.Zero;

        if (speedBoost > 1)
        {
            Vector2 pos = Projectile.Center - Vector2.UnitY * (Projectile.height / 2f);
            Dust.NewDustPerfect(pos, DustID.GemDiamond, Scale: 2f).noGravity = true;
        }

        SetPositionRotationDirection(player, 0);

        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpT * speedBoost * player.GetAttackSpeed(DamageClass.Melee));
        if (rotValue > goalRotation - 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));

        AITimer++;
    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * 30, -30).RotatedBy(movedRotation * player.direction);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;

        Texture2D originalTexture = TextureAssets.Item[ItemID.LightsBane].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;


        /*for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 afterimagePos = Projectile.oldPos[k] - Main.screenPosition + glowTexture.Size() * 0.5f + new Vector2(0, Projectile.gfxOffY);
            Color color = new Color(70, 0, 0, 255) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            color.A /= 2;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.oldSpriteDirection[k] == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Main.EntitySpriteDraw(glowTexture, drawPos, null, color, Projectile.oldRot[k], glowTexture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
        }*/


        Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, originalTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["color"].SetValue(Color.Purple.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(glowTexture, drawPos, null, Color.White, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;

    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
