using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.Audio;
using static Terraria.GameContent.Animations.Actions.Sprites;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic
{
    public class FurnaceArrow : ModProjectile
    {
        int AITimer;
        ref float TimeLeft => ref Projectile.ai[0];
        ref float ChargeAmount => ref Projectile.ai[1];
        bool exploding = false;
        int explodeTimer = 0;

        public override string Texture => ParacosmTextures.Empty100TexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 6000;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.extraUpdates = 10;
        }

        float Power => ChargeAmount * 8;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();

            return false;
        }

        public override void AI()
        {
            if (AITimer == 0)
            {

            }
            if (Projectile.wet && ChargeAmount > 4)
            {
                ChargeAmount -= 4;
            }
            if (exploding)
            {
                for (int i = 0; i < 10 + ChargeAmount / 8; i++)
                {
                    ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                        ParticleID.Glowy,
                        Projectile.Center,
                        Main.rand.NextVector2Circular(20 + ChargeAmount / 4, 20 + ChargeAmount / 4),
                        Main.rand.NextFromList(Color.OrangeRed),
                        0f,
                        scale: Main.rand.NextFloat(2f * (1 + ChargeAmount / 120f), 4f * (1 + ChargeAmount / 120f)),
                        data0: Main.rand.Next(70, 100),
                        data1: Main.rand.Next(10, 30),
                        data2: Main.rand.Next(10, 30),
                        data3: Main.rand.NextFloat(0.93f, 0.96f)
                    );

                }
                for (int i = 0; i < 5 + ChargeAmount / 10; i++)
                {
                    ParticleSystem.SpawnParticle(
                        ParticleID.Gas,
                        Projectile.Center,
                        Main.rand.NextVector2Circular(20 + ChargeAmount / 4, 20 + ChargeAmount / 4),
                        Color.OrangeRed,
                        scale: Main.rand.NextFloat(2f * (1 + ChargeAmount / 120f), 4f * (1 + ChargeAmount / 120f))
                    );
                }
                Projectile.Resize(400 + Power, 400 + Power);
                Projectile.velocity = Vector2.Zero;
                if (explodeTimer > 60)
                {
                    Projectile.Kill();
                }
                explodeTimer++;
                return;
            }

            for (int j = 0; j < 3; j++)
            {
                ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                        ParticleID.Glowy,
                        Projectile.Center + Main.rand.NextVector2Circular(2, 2),
                        Vector2.Zero,
                        Main.rand.NextFromList(Color.OrangeRed),
                        0f,
                        scale: 1f,
                        data0: 15,
                        data1: 5,
                        data2: 5,
                        data3: 0.93f);

                for (int i = -1; i <= 1; i += 2)
                {
                    ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                        ParticleID.Glowy,
                        Projectile.Center + Main.rand.NextVector2Circular(2, 2),
                        Projectile.velocity.RotatedBy(MathHelper.Pi / 8f * i).SafeNormalize(Vector2.Zero) * 10,
                        Main.rand.NextFromList(Color.OrangeRed),
                        0f,
                        scale: 1.2f,
                        data0: 15,
                        data1: 5,
                        data2: 5,
                        data3: 0.93f);
                }
            }

            if (AITimer > TimeLeft)
            {
                Projectile.Kill();
            }
            AITimer++;
        }

        void Explode()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            if (!exploding)
            {
                exploding = true;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { PitchRange = (-0.2f, 0.2f), MaxInstances = 5 }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { PitchRange = (-0.8f, -0.5f), MaxInstances = 5 }, Projectile.Center);

                if (ChargeAmount == 120)
                {

                    SoundEngine.PlaySound(SoundID.NPCHit57 with { PitchRange = (-0.8f, -0.5f) }, Projectile.Center);

                    SoundEngine.PlaySound(SoundID.NPCDeath62 with { PitchRange = (-0.8f, -0.5f) }, Projectile.Center);

                    SoundEngine.PlaySound(SoundID.Zombie92 with { PitchRange = (-0.8f, -0.5f) }, Projectile.Center);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Explode();
        }

        public override void OnKill(int timeLeft)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*if (exploding)
            {
                float scale = explodeTimer / 60f * 10;
                LemonUtils.DrawGlow(Projectile.Center, Color.White, 1f, scale);
                LemonUtils.DrawGlow(Projectile.Center, Color.White, 1f, scale);
            }*/
            return true;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}
