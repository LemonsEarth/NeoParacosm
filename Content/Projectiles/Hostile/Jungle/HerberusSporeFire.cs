using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Jungle
{
    public class HerberusSporeFire : ModProjectile
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
                SoundEngine.PlaySound(SoundID.Item74 with { PitchRange = (0.2f, 0.4f)}, Projectile.Center);
                Projectile.velocity = Vector2.Zero;
                Projectile.Resize(128, 128);
                Projectile.timeLeft = 60 * LemonUtils.GetDifficulty();
                for (int i = 0; i < 8; i++)
                {
                    Vector2 randomPos1 = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Vector2 randomPos2 = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Dust.NewDustPerfect(randomPos1, DustID.OrangeStainedGlass, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                    Dust.NewDustPerfect(randomPos2, DustID.GemTopaz, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                }
            }

            if (stopped)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 randomPos1 = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Vector2 randomPos2 = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Dust.NewDustPerfect(randomPos1, DustID.OrangeStainedGlass, Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                    Dust.NewDustPerfect(randomPos2, DustID.GemTopaz, Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                }
            }
            else
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.OrangeStainedGlass, Vector2.Zero, Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, Vector2.Zero, Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
            }
            AITimer++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 8 * 60);
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
