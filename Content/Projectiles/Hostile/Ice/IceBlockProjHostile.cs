namespace NeoParacosm.Content.Projectiles.Hostile.Ice;

public class IceBlockProjHostile : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float TilePosX => ref Projectile.ai[1];
    ref float TilePosY => ref Projectile.ai[2];

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
        Projectile.penetrate = 1;
        Projectile.timeLeft = 900;
        Projectile.scale = 1f;
        Projectile.Opacity = 0f;
        Projectile.extraUpdates = 2;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {

        return true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        if (AITimer > TimeLeft ||
            new Vector2((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)) == new Vector2((int)(TilePosX / 16), (int)(TilePosY / 16)))
        {
            Projectile.Kill();
        }

        Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1);

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Scale: Main.rand.NextFloat(1.5f, 2.5f));
        dust.noGravity = true;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        if (LemonUtils.NotClient())
        {
            WorldGen.PlaceTile((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), TileID.MagicalIceBlock);
            NetMessage.SendTileSquare(-1, (int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Frostburn, 240);
    }
}
