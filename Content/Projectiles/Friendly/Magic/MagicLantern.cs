using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class MagicLantern : PrimProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.timeLeft = 9999;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        /*for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, newColor: Color.Lime, Scale: 1.5f).noGravity = true;
        }*/
        Player player = Projectile.GetOwner();
        if (AITimer == 0)
        {
            Projectile.scale = 0.1f;
        }
        Projectile.velocity *= 0.96f;
        if (AITimer <= 30)
        {
            Projectile.scale = MathHelper.Lerp(0.1f, 1, AITimer / 30f);
        }
        else
        {
            Projectile.scale = MathHelper.Lerp(1, 0, AITimer / (Duration + 30));
        }

        if (AITimer > Duration)
        {
            Projectile.Kill();
        }
        float lightLevel = 3 * Projectile.scale * player.GetElementalExpertiseBoost(Items.Weapons.Magic.Spells.BaseSpell.SpellElement.Pure);
        Lighting.AddLight(Projectile.Center, lightLevel, lightLevel, lightLevel);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemSapphire).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.DarkSlateBlue, 1f, Projectile.scale);
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.LightBlue, Color.Transparent, BasicEffect);
        return true;
    }
}
