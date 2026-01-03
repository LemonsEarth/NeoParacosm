
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class DeathbirdFeatherFX : ModProjectile
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 120;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.DD2_BetsySummon with { PitchRange = (0f, 0.2f) }, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Color color = i == 0 ? Color.Black : Color.White;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color * ((14 - i) / (float)Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        }
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
