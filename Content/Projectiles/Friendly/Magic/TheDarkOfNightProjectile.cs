using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class TheDarkOfNightProjectile : ModProjectile
{
    int AITimer = 0;
    ref float SpreadSpeed => ref Projectile.ai[0];
    ref float AttackInterval => ref Projectile.ai[1];

    float spreadTimer;
    bool released = false;
    int releasedTimer = 0;

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 3600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 0;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        Projectile.Center = player.MountedCenter;
        player.velocity *= 0.9f;
        if (!player.channel && !released)
        {
            if (spreadTimer >= 180 && player.GetElementalExpertiseBoost(SpellElement.Dark) > 1.4f)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52 with { PitchRange = (-0.6f, -0.3f), MaxInstances = 0}, player.Center);
                for (int i = 0; i < 10; i++)
                {
                    if (LemonUtils.NotClient())
                    {
                        Vector2 pos = Projectile.Center;
                        float speed = Main.rand.NextFloat(2f, 5f);
                        LemonUtils.QuickProj(
                            Projectile,
                            pos,
                            Vector2.UnitY.RotatedByRandom(Main.rand.NextRotation()) * speed,
                            ProjectileType<TheDarkOfNightSmallProj>(),
                            ai0: Main.rand.Next(30, 60),
                            ai1: speed * 4,
                            ai2: 400 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 2f)
                            );
                    }
                }
            }
            released = true;
        }

        Projectile.timeLeft = 2;

        if (!released)
        {
            ChannelingBehavior(player);
        }
        else
        {
            ReleasedBehavior();
        }

        AITimer++;
    }

    void ChannelingBehavior(Player player)
    {
        float size = MathHelper.Clamp(spreadTimer / 60f, 0f, 3f);
        for (int i = 0; i < size * 6; i++)
        {
            ParticleSystem.SpawnParticle<BeforeDustParticleRenderer>(
                ParticleID.Gas,
                Projectile.Center + Main.rand.NextVector2Circular(250 * size, 250 * size),
                Vector2.UnitY.RotatedBy(Main.rand.NextRotation()) * Main.rand.NextFloat(size * 1f, size * 3f),
                Main.rand.NextFromList(Color.Black),
                Main.rand.NextFloat(0.5f, 1f),
                Main.rand.NextFloat(0.5f, size * 4)
                );
        }

        if (AITimer % AttackInterval == 0)
        {
            if (LemonUtils.NotClient())
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(150 * size, 150 * size);
                float speed = Main.rand.NextFloat(2f, 6f);
                LemonUtils.QuickProj(
                    Projectile,
                    pos,
                    Vector2.UnitY.RotatedByRandom(Main.rand.NextRotation()) * speed / 5f,
                    ProjectileType<TheDarkOfNightSmallProj>(),
                    ai0: Main.rand.Next(90, 150),
                    ai1: speed * 2,
                    ai2: 400 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 2f)
                    );
            }
        }

        ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
            ParticleID.Glowy,
            player.headPosition,
            Vector2.Zero,
            Main.rand.NextFromList(Color.DarkSlateBlue, Color.CornflowerBlue, Color.LightBlue, Color.SlateBlue),
            scale: Main.rand.NextFloat(0.25f, 0.6f),
            data0: 45,
            data1: 10,
            data2: 20,
            data3: 0.95f
            );

        spreadTimer += SpreadSpeed;
    }

    void ReleasedBehavior()
    {
        if (releasedTimer > 90)
        {
            Projectile.Kill();
            return;
        }
        releasedTimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {

    }
}

