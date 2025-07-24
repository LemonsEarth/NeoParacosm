using NeoParacosm.Common.Utils;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class LingeringDeathflame : ModProjectile
{
    int AITimer = 0;
    bool landed = false;

    ref float playedID => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 420;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    float maxSpeed = 0;
    float currentSpeed = 0;
    float speedAddValue = 0.05f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
        }

        if (Projectile.velocity.Y == 0)
        {
            landed = true;
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomPos = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), 0);
                Dust.NewDustPerfect(randomPos, DustID.Ash, -Vector2.UnitY * Main.rand.NextFloat(2f, 4f), Scale: 2, newColor: Color.Black).noGravity = true;
                Dust.NewDustPerfect(randomPos, DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(2f, 4f), Scale: 1f, newColor: Color.White).noGravity = true;
            }
            Projectile.velocity.X = 0;
            Projectile.width = 32;
        }
        else
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ash, 0, 0, Scale: 1.5f, newColor: Color.Black).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, 0, 0, Scale: 1.25f, newColor: Color.White).noGravity = true;
            Projectile.width = 16;
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);

        Projectile.velocity.Y += 0.1f;

        Projectile.rotation = Projectile.velocity.ToRotation();
        AITimer++;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (landed)
        {
            fallThrough = false;
            return true;
        }
        fallThrough = Main.player[(int)playedID].Alive() && (Main.player[(int)playedID].Bottom.Y > Projectile.Center.Y + 16);
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }
}
