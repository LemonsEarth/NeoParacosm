using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged
{
    public class SharpBoneProjectile : ModProjectile
    {
        int AITimer;
        ref float TimeLeft => ref Projectile.ai[0];
        ref float HitNPCWhoAmI => ref Projectile.ai[1];
        Vector2 offset;
        float savedRotation;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 600;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return HitNPCWhoAmI == -1;
        }

        public override void AI()
        {
            if (AITimer == 0)
            {
                HitNPCWhoAmI = -1;
            }

            if (HitNPCWhoAmI >= 0)
            {
                NPC hitNPC = Main.npc[(int)HitNPCWhoAmI];
                Projectile.Center = hitNPC.Center + offset;
                if (!hitNPC.active || hitNPC.life == 0)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Opacity = hitNPC.Opacity;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4;
                if (Projectile.velocity.X < 0)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.rotation -= MathHelper.PiOver2;
                }

                if (AITimer > 30)
                {
                    Projectile.velocity.Y += 0.1f;
                }
            }

            Projectile.StandardAnimation(6, 6);
            AITimer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HitNPCWhoAmI == -1 && !NPCID.Sets.ProjectileNPC[target.type])
            {
                HitNPCWhoAmI = target.whoAmI;
                offset = Projectile.Center - target.Center;
                savedRotation = offset.ToRotation();
                Projectile.timeLeft = 90;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { PitchRange = (0.8f, 1f) }, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Vector2.Zero,
                    ProjectileType<DeathflameExplosionFriendly>(),
                    Projectile.originalDamage,
                    7f,
                    ai0: 3,
                    ai1: 1f
                    );
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
            Main.EntitySpriteDraw(
                texture,
                drawPos,
                sourceRect,
                Color.White * Projectile.Opacity,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection),
                0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}
