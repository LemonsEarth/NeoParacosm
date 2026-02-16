using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee
{
    public class SpiritProjDamage : ModProjectile
    {
        ref float AITimer => ref Projectile.ai[0];
        ref float Speed => ref Projectile.ai[1];
        NPC closestNPC;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            if (AITimer <= 0)
            {
                Projectile.damage = Projectile.originalDamage;
                Projectile.Opacity += 0.1f;
                closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 500);
                if (closestNPC != null)
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 1 / 60f);
                    Projectile.MoveToPos(closestNPC.Center, 0.05f, 0.05f, 0.3f * Speed, 0.3f * Speed);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 1 / 30f);
                }
                else
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 0f, 1 / 60f);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 1 / 30f);
                }
            }
            else
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0.5f, 1 / 30f);
                Projectile.damage = 0;
                Projectile.velocity /= 1.1f;
                Projectile.rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }

            for (int i = 0; i < 3; i++)
            {
                var dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.SpectreStaff);
                dust.noGravity = true;
            }

            Projectile.StandardAnimation(6, 4);
            Lighting.AddLight(Projectile.Center, 1, 1, 1);

            AITimer--;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit36 with { PitchRange = (-0.3f, 0.3f), Volume = 0.5f }, Projectile.position);
            LemonUtils.DustCircle(Projectile.Center, 16, 5, DustID.GemDiamond);
        }
    }
}
