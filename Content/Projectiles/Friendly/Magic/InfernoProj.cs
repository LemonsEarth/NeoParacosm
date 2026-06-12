using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class InfernoProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Torch).noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6);
        Projectile.velocity *= 0.97f;
        if (Projectile.velocity.LengthSquared() < 1 * 1)
        {
            Projectile.Kill();
            return;
        }
        //Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Stone).noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 1f, PitchRange = (-1f, 1f) }, Projectile.Center);

        for (int i = 0; i < 5; i++)
        {
            Vector2 pos = Projectile.Center;
            Vector2 velocity = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 5f)) * 3;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromAI(),
                    pos,
                    velocity * Projectile.GetOwner().GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2),
                    ProjectileID.Flames,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                    );
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        //LemonUtils.DrawAfterimages(Projectile, lightColor, 0.1f);
        //PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.White, Color.Black * 0, BasicEffect, topDistance: 7, bottomDistance: 7);
        return true;
    }
}
