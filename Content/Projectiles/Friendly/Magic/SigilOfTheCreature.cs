using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class SigilOfTheCreature : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float CycleDurationMul => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 56;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
            LemonUtils.DustCircle(
            Projectile.Center,
            8,
            8,
            DustID.GemTopaz,
            1.5f
            );
        }

        if (AITimer >= (int)(360 * CycleDurationMul))
        {
            Projectile.Kill();
        }

        if (AITimer > 10 && AITimer % 5 == 0)
        {
            SoundEngine.PlaySound(SoundID.Item4 with { PitchRange = (0.5f, 0.7f) }, Projectile.Center);
            if (LemonUtils.NotClient())
            {
                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis("NeoParacosm:GoldenSigil"),
                    Projectile.RandomPos() + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(AITimer)) * 36,
                    Vector2.UnitX.RotatedBy(MathHelper.ToRadians(AITimer)) * Main.rand.NextFloat(8, 12) * CycleDurationMul,
                    ProjectileID.GoldenBullet,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                    );
            }
        }

        Projectile.rotation = MathHelper.ToRadians(AITimer);


        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);

        LemonUtils.DustCircle(
            Projectile.Center,
            8,
            8,
            DustID.GemTopaz,
            1.5f
            );
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        float sin = (MathF.Sin(AITimer / 24f) + 4) * 0.25f;
        LemonUtils.DrawGlow(Projectile.Center, Color.LightGoldenrodYellow, Projectile.Opacity * 0.5f, Projectile.scale * 1f * sin);
        Projectile.DrawProjectile(Color.White);
        LemonUtils.DrawGlow(Projectile.Center, Color.PaleGoldenrod, Projectile.Opacity * 1f, Projectile.scale * 1f * sin);
        return false;
    }
}

public class SigilGoldenBulletGlobalProjectile : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return lateInstantiation && entity.type == ProjectileID.GoldenBullet;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "NeoParacosm:GoldenSigil")
        {
            projectile.DamageType = DamageClass.Magic;
        }
    }
}
