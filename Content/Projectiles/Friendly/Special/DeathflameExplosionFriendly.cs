using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class DeathflameExplosionFriendly : ModProjectile
{
    int AITimer;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.timeLeft = 600;

        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.Resize(128 * Scale, 128 * Scale);
        }

        for (int i = 0; i < Scale * 5; i++)
        {
            ParticleSystem.SpawnParticle(
                ParticleID.Gas,
                Projectile.Center,
                Main.rand.NextVector2Circular(6, 6) * Scale,
                Color.Black,
                scale: Main.rand.NextFloat(0.5f, 1.5f)
                );
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 120);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

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
