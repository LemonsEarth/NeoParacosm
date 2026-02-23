using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class IceComet : PrimProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float PosX => ref Projectile.ai[1];
    ref float PosY => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 132;
        Projectile.height = 132;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 9999;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.ArmorPenetration = 5;
    }

    bool exploded = false;

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Projectile.rotation = MathHelper.ToRadians(AITimer);

        if ((AITimer > TimeLeft || Projectile.Center.Distance(new Vector2(PosX, PosY)) < Projectile.width / 2) && !exploded)
        {
            exploded = true;
            Projectile.Resize(132 * 4, 132 * 4);
            Projectile.timeLeft = 3;
        }
        Dust.NewDustDirect(Projectile.RandomPos(), 1, 1, DustID.IceTorch, Scale: Main.rand.NextFloat(3.5f, 5.5f)).noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(60, Projectile.Center, DustID.GemSapphire, 40, 40, 3f, 5f);
        if (Main.myPlayer == Projectile.owner)
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 3, 6, 5, Color.Cyan);
        }
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn2, 120);
        Projectile.damage -= (int)(0.2f * Projectile.originalDamage);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (exploded) return true;
        PrimHelper.DrawBasicProjectilePrimTrailRectangular(Projectile, Color.Cyan, Color.Transparent, BasicEffect);
        return true;
    }
}
