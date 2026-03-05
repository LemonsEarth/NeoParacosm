using NeoParacosm.Content.Buffs.Debuffs;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class MushroomDisc : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        Lighting.AddLight(Projectile.Center, 0, 0, 1);

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GlowingMushroom);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 18);
        AITimer++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //Projectile.damage = (int)(Projectile.damage * 1.2f);
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<ShroomedDebuff>(), 300);

        var closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 500, target.whoAmI);
        if (closestNPC == null || closestNPC.whoAmI == target.whoAmI)
        {
            return;
        }
        float length = Projectile.velocity.Length();
        Projectile.velocity = Projectile.DirectionTo(closestNPC.Center) * length;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);
        return true;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 2, DustID.GlowingMushroom);
    }
}
