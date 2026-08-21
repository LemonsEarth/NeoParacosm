using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;

namespace NeoParacosm.Content.Projectiles.Hostile.Jungle
{
    public class HerberusSporeBig : ModProjectile
    {
        public override string Texture => ParacosmTextures.Empty100TexPath;

        int AITimer;
        ref float TimeLeft => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 600;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (AITimer == 0)
            {

            }

            ParticleSystem.SpawnParticle(
                ParticleID.Fire,
                Projectile.RandomPos(),
                Main.rand.NextVector2Circular(3, 3),
                new Color(Main.rand.Next(60, 190) / 255f, Main.rand.Next(180, 255) / 255f, Main.rand.Next(10, 80) / 255f),
                scale: Main.rand.NextFloat(0.6f, 1f)
                );
            Projectile.velocity *= 0.85f;
            if (AITimer > TimeLeft)
            {
                Projectile.Kill();
                return;
            }

            AITimer++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 15 * 60);
        }

        public override void OnKill(int timeLeft)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}
