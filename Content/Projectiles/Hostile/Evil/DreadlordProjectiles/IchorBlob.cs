using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorBlob : PrimProjectile
{
    int AITimer = 0;
    ref float FallSpeed => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

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
        Projectile.tileCollide = false;
        Projectile.timeLeft = 999;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13 with { PitchRange = (0.3f, 0.5f), Volume = 0.5f}, Projectile.Center);
        }

        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ichor).noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 6);


        Projectile.velocity.Y += FallSpeed;
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
        float scaleX = (MathF.Sin(AITimer / 4f) * 0.3f + 1) * Projectile.scale;

        // 1.25 when 0
        float scaleY = (MathF.Sin(AITimer / 4f + MathHelper.PiOver2) * 0.3f + 1) * Projectile.scale;
        Main.EntitySpriteDraw(
            TextureAssets.Projectile[Type].Value,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            TextureAssets.Projectile[Type].Size() * 0.5f,
            new Vector2(scaleX, scaleY),
            SpriteEffects.None
            );
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
