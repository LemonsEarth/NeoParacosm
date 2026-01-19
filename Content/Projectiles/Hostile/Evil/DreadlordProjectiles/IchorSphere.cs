using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorSphere : PrimProjectile
{
    string GlowMask_Path => Texture + "_Glow";

    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float RedirectInterval => ref Projectile.ai[1];
    ref float RedirectCount => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 999;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
        }

        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ichor);
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6);

        if (AITimer > 0 && AITimer % RedirectInterval == 0 && RedirectCount > 0)
        {
            Player closestPlayer = LemonUtils.GetClosestPlayer(Projectile.Center, 2000);
            Projectile.velocity = Projectile.DirectionTo(closestPlayer.Center) * savedSpeed;
            RedirectCount--;
        }

        if (AITimer > TimeLeft) Projectile.Kill();
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }

    public override void OnKill(int timeLeft)
    {
       
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Main.dedServ) return true;
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.Yellow, Color.Yellow, BasicEffect);
        Main.EntitySpriteDraw(
            TextureAssets.Projectile[Type].Value,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            TextureAssets.Projectile[Type].Size() * 0.5f,
            Projectile.scale,
            SpriteEffects.None
            );
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        
    }
}
