using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class DeathflameExplosion : ModProjectile
{
    int AITimer;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
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
            Projectile.Resize(128 * Scale, 128 * Scale);
            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { PitchRange = (-0.4f, -0.3f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { PitchRange = (-0.4f, -0.3f) }, Projectile.Center);
            Projectile.rotation = Main.rand.NextRotation();
        }
        Projectile.scale = Scale;
        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        Projectile.StandardAnimation((int)(TimeLeft / Main.projFrames[Type]), 6, false);

        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 120);
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawProjectile(Color.White);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
