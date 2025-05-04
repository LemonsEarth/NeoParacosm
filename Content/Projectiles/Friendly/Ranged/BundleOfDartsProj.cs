using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class BundleOfDartsProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    bool pierce = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, 3);
            pierce = Main.rand.NextBool(3);
            if (pierce)
            {
                Projectile.penetrate = 3;
            }
        }

        if (pierce)
        {
            var shineDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond);
            shineDust.noGravity = true;
        }

        if (AITimer > 20)
        {
            Projectile.velocity.Y += 1;
        }
        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RichMahogany);
        dust.noGravity = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Poisoned, 240);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (pierce)
        {
            modifiers.DisableCrit();
            modifiers.ArmorPenetration += 5;
        }
    }
}
