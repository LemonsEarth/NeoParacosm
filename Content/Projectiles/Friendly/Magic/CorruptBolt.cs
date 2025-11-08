using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CorruptBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float angle => ref Projectile.ai[1];

    static BasicEffect BasicEffect;

    public override void Load()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect = new BasicEffect(PrimHelper.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
        });
    }

    public override void Unload()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect?.Dispose();
            BasicEffect = null;
        });
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 90;
        Projectile.penetrate = 3;
        Projectile.Opacity = 0f;
        Projectile.tileCollide = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, Projectile.Center);
        }

        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Scale: 1.5f).noGravity = true;
        }

        Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathHelper.ToRadians(angle);
        //Main.NewText(AITimer);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.Purple, Color.Transparent, BasicEffect);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.Corruption, 4f);
    }
}
