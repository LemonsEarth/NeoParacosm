using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Jungle
{
    public class HerberusSpore : ModProjectile
    {
        public override string Texture => ParacosmTextures.Empty100TexPath;

        int AITimer;
        ref float TargetID => ref Projectile.ai[0];
        bool stopped = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 600;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        Vector2 startPos;
        Player player => Main.player[(int)TargetID];
        public override void AI()
        {
            if (AITimer == 0)
            {
                startPos = Projectile.Center;
            }

            if (Projectile.Center.DistanceSQ(startPos) > startPos.DistanceSQ(player.Center) && !stopped)
            {
                stopped = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.Resize(64, 64);
                Projectile.timeLeft = 60 * LemonUtils.GetDifficulty();
            }

            if (stopped)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(Projectile.RandomPos(), DustID.RuneWizard, Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                }
            }
            else
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.RuneWizard, Vector2.Zero, Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
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
