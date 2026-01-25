using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class MagicStar : PrimProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float posX => ref Projectile.ai[1];
    ref float posY => ref Projectile.ai[2];
    NPC closestNPC;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 900;
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
        if (Projectile.timeLeft == 900)
        {
            Projectile.scale = 0.1f;
        }
        Player player = Projectile.GetOwner();
        if (AITimer <= 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 350);
            if (closestNPC != null)
            {
                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10;
            }
        }
        if (closestNPC == null || !closestNPC.active)
        {
            Projectile.Center = player.Center - new Vector2(posX, posY);
            Projectile.velocity = Vector2.Zero;
        }
        if (Projectile.timeLeft > 800)
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 1 / 10f);
        }
        else
        {
            Projectile.scale = MathHelper.Lerp(0, 1, Projectile.timeLeft / 800f);
        }
        Projectile.rotation = AITimer * 6;

        if (AITimer % 60 == 0)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
        }
        Lighting.AddLight(Projectile.position, 2, 2, 2);
        AITimer--;
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
