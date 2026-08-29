using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class TheStormProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    int AITimer = 0;
    ref float Direction => ref Projectile.ai[0];
    ref float WaitTime => ref Projectile.ai[1];
    ref float ProjInterval => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        if (AITimer > WaitTime)
        {
            if (AITimer % ProjInterval == 0)
            {
                SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                if (LemonUtils.NotClient())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = Projectile.Center
                            + new Vector2(
                                Main.rand.NextFloat(-Projectile.width / 2f, Projectile.width / 2f),
                                Main.rand.NextFloat(-Projectile.height / 2f, Projectile.height / 2f)
                                );
                        LemonUtils.QuickProj(
                            Projectile,
                            pos, Vector2.UnitY * Main.rand.NextFloat(30, 50), ProjectileType<HolyLightningSpearFriendlyRanged>(),
                            ai0: 60, ai1: 1);
                    }
                }
            }
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }
}
