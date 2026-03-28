using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class CursebinderBigProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    int AITimer = 0;
    ref float Distance => ref Projectile.ai[0];
    ref float Power => ref Projectile.ai[1];
    ref float Timeleft => ref Projectile.ai[2];

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
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    float distanceIncreaseRate = 100; // per second
    Vector2 topPoint;
    Vector2 botPoint;
    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.scale = 1f;
            if (Timeleft == 0)
            {
                Timeleft = 60;
            }
        }

        topPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver4) * Distance;
        botPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver4) * Distance;
        float angleBetween = LemonUtils.AngleBetween(Projectile.DirectionTo(topPoint), Projectile.DirectionTo(botPoint));
        for (int i = 0; i < 30; i++)
        {
            Vector2 pos = Projectile.Center + Projectile.Center.DirectionTo(topPoint).RotatedBy(i * angleBetween / 30) * Distance;

            Dust.NewDustDirect(pos, 2, 2, DustType<StreakDust>(), newColor: Color.DarkRed, Scale: Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
        }

        Distance += Projectile.velocity.Length();
        //Main.NewText(Distance);

        if (AITimer > Timeleft)
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0;
        Vector2 centerPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;
        bool col1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), topPoint, centerPos, 64, ref _);
        bool col2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), botPoint, centerPos, 64, ref _);
        return col1 || col2;
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }
}
