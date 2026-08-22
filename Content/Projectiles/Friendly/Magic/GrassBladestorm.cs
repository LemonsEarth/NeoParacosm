using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic
{
    public class GrassBladestorm : ModProjectile
    {
        int AITimer;
        ref float TimeLeft => ref Projectile.ai[0];
        ref float AttackInterval => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.timeLeft = 999;
            Projectile.Opacity = 0f;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void AI()
        {
            if (AITimer == 0)
            {

            }

            if (AITimer < 30)
            {
                Projectile.Opacity = AITimer / 60f;
            }

            if (AITimer % AttackInterval == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.2f, PitchRange = (-0.3f, -0.1f) }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 30,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: 0.08f
                        );
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 10,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: -0.08f
                        );

                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 30,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: 0.15f
                        );
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 10,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: -0.15f
                        );

                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 30,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: 0.2f
                        );
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 10,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: -0.2f
                        );

                    LemonUtils.QuickProj(
                       Projectile,
                       Projectile.Center + Projectile.velocity * 30,
                       Projectile.velocity,
                       ProjectileID.BladeOfGrass,
                       ai0: 0.3f
                       );
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center + Projectile.velocity * 10,
                        Projectile.velocity,
                        ProjectileID.BladeOfGrass,
                        ai0: -0.3f
                        );

                }
            }
            Projectile.rotation = MathHelper.ToRadians(AITimer * 6);
            if (AITimer > TimeLeft)
            {
                Projectile.Kill();
            }
            AITimer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

        public override void OnKill(int timeLeft)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawAfterimages(lightColor);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
            Color color1 = Color.White * Projectile.Opacity;
            Color color2 = Color.White * Projectile.Opacity * 0.5f;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPosAI = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
                Color color = (color1 * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length));
                Main.EntitySpriteDraw(texture, drawPosAI, sourceRect, color, -Projectile.oldRot[k] * 0.5f, drawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color1, -Projectile.rotation * 0.5f, drawOrigin, Projectile.scale * 1.5f, SpriteEffects.FlipHorizontally, 0);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPosAI = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
                Color color = (color2 * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length));
                Main.EntitySpriteDraw(texture, drawPosAI, sourceRect, color, Projectile.oldRot[k], drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color2, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPosAI = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
                Color color = (color1 * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length));
                Main.EntitySpriteDraw(texture, drawPosAI, sourceRect, color, -Projectile.oldRot[k] + MathHelper.PiOver4, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color1, -Projectile.rotation + MathHelper.PiOver4, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.FlipHorizontally, 0);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}
