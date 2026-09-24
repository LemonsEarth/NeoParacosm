using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class PoisonGasProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    ref float AITimer => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = 5;
        Projectile.timeLeft = 180;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
        Projectile.extraUpdates = 10;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        for (int i = 0; i < 3; i++)
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                Projectile.RandomPos(16, 16),
                Main.rand.NextVector2Circular(5, 5),
                Main.rand.NextFromList(new Color(0, 30, 0, 255) * 1f),
                0f,
                scale: 3f,
                data0: 60,
                data1: 10,
                data2: 10,
                data3: 0.95f);
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
            return;
        }
        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int amount = (int)(250 + 50 * Projectile.GetOwner().GetElementalExpertiseBoostMultiplied(SpellElement.Nature, 1.5f));
        ToxicDebuff.AddToNPC(target, amount);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
