using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.NPCs.Hostile.Crimson;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class AscendedUndertakerHeldProj : ModProjectile
{
    int AITimer = 0;
    ref float shotCount => ref Projectile.ai[0];
    bool thrown = false;
    float recoilRot = 0;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 28;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    float goalRotation = 270;
    float lerpT = 4f / 60f;
    float attackRate = 10;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        if (AITimer == 0) rot = player.direction == 1 ? normRot : oppRot;
        if (shotCount < 6)
        {
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(20);
            Projectile.timeLeft = 180;
            if (AITimer % attackRate == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 dir = Projectile.Center.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5)));
                    player.PickAmmo(player.HeldItem, out int proj, out float _, out int damage, out float knockback, out int usedAmmoItemID);
                    Vector2 pos = Projectile.Center + Projectile.Center.DirectionTo(Main.MouseWorld) * 5;
                    LemonUtils.QuickProj(Projectile, pos, dir * Main.rand.NextFloat(5, 16), ProjectileID.IchorBullet);
                }
                SoundEngine.PlaySound(SoundID.Item11 with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 1 }, Projectile.Center);
                rot += -player.direction * MathHelper.ToRadians(15);
                shotCount++;
            }
            SetPositionRotationDirection(player, player.Center.DirectionTo(Main.MouseWorld).ToRotation());
            //Projectile.Center = player.Center + player.Center.DirectionTo(Main.MouseWorld) * 5;
        }
        if (shotCount >= 6 && !thrown && AITimer % attackRate == 0)
        {
            thrown = true;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 25;
            }
            Projectile.netUpdate = true;
        }

        if (thrown)
        {
            Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
            Projectile.velocity.Y += 0.25f;
            Projectile.damage = Projectile.originalDamage * 4;
        }

        AITimer++;
    }

    float rot = 0;
    float normRot = 0;
    float oppRot = MathHelper.Pi;
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        float goalRot = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
        rot = Utils.AngleLerp(rot, goalRot, 1f / 10f);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + -MathHelper.PiOver2);
        float spriteRot = player.direction == 1 ? 0 : MathHelper.Pi;
        Projectile.rotation = rot + spriteRot;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;

        Texture2D originalTexture = TextureAssets.Item[ItemID.TheUndertaker].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        int xOrigin = Projectile.spriteDirection == 1 ? 0 : originalTexture.Width;
        int xGlowOrigin = Projectile.spriteDirection == 1 ? 0 : glowTexture.Width;
        Vector2 origOrigin = new Vector2(xOrigin, originalTexture.Height * 0.5f);
        Vector2 glowOrigin = new Vector2(xGlowOrigin, glowTexture.Height * 0.5f);

        //if (thrown) //afterimages
        //{
        //    for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        //    {
        //        Vector2 afterimagePos = Projectile.oldPos[k] - Main.screenPosition + glowTexture.Size() * 0.5f + new Vector2(0, Projectile.gfxOffY);
        //        Color color = new Color(70, 0, 0, 255) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
        //        color.A /= 2;
        //        SpriteEffects spriteEffects = SpriteEffects.None;
        //        if (Projectile.oldSpriteDirection[k] == -1)
        //        {
        //            spriteEffects = SpriteEffects.FlipHorizontally;
        //        }
        //        Main.EntitySpriteDraw(glowTexture, drawPos, null, color, Projectile.oldRot[k], glowTexture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
        //    }
        //}

        Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(glowTexture, drawPos, null, Color.White, Projectile.rotation, glowOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;

    }
}
