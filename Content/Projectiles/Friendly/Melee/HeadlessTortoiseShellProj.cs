using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class HeadlessTortoiseShellProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float SpinDirection => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 58;
        Projectile.height = 58;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 10;
        Projectile.timeLeft = 180;
        Projectile.scale = 1.5f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (SpinDirection == 0) SpinDirection = 1;
        }
        Player player = Projectile.GetOwner();
        Projectile.Resize((int)(58 * Projectile.scale), (int)(58 * Projectile.scale));
        if (AITimer % 20 == 0)
        {
            SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
        }
        //Projectile.Center.NewText();
        if (!player.IsAlive())
        {
            Projectile.Kill();
            return;
        }

        float distanceToPlayer = Projectile.Center.Distance(player.Center);
        Projectile.scale = MathHelper.Lerp(1.5f, 1f, MathHelper.Clamp(distanceToPlayer / 300f, 0.1f, 1f));
        Lighting.AddLight(Projectile.Center, 1, 0, 0);

        if (AITimer < 60)
        {
            Projectile.velocity *= 0.95f;
        }
        else
        {
            Vector2 toPlayer = Projectile.DirectionTo(player.Center);
            if (!toPlayer.HasNaNs())
            {
                if (Projectile.velocity.Length() < 20)
                {
                    Projectile.velocity += toPlayer;
                }
                else
                {
                    Projectile.velocity = toPlayer * 20;
                }
            }
        }
        //toPlayer.NewText();

        if (AITimer > 60 && distanceToPlayer < 32)
        {
            Projectile.Kill();
            return;
        }
        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 18 * SpinDirection);
        AITimer++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //Projectile.damage = (int)(Projectile.damage * 1.2f);
        /*if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }*/
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);
        Projectile.DrawProjectile(lightColor);
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 2, DustID.Crimson, 2);
    }
}
