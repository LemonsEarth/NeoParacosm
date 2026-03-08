using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile.Misc;

public class BombExplosion : ModProjectile
{
    public override string Texture => "Terraria/Images/Item_0"; // Use Iron Pickaxe texture cause im lazy

    ref float AITimer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 2;
        Projectile.penetrate = -1;
        Projectile.Opacity = 0f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void AI()
    {
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        for (int i = 0; i < 6; i++)
        {
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2)), Main.rand.Next(61, 64), Main.rand.NextFloat(0.5f, 2f));
        }
        for (int i = 0; i < 12; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Torch, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(1.5f, 2.5f));
        }
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int explosionRadius = 5;
            int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
            int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
            int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
            int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);
            bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
            Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
        }
    }
}
