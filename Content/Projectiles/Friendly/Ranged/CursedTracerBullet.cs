using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using SteelSeries.GameSense.DeviceZone;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CursedTracerBullet : ModProjectile
{
    int AITimer = 0;

    Vector2 StartPos
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 8;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 200;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.extraUpdates = 10;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.damage = 0;
        Projectile.velocity = Vector2.Zero;
        return false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            //Projectile.Center = StartPos + Projectile.velocity.SafeNormalize(Vector2.Zero);
            StartPos = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);

        if (Projectile.velocity != Vector2.Zero)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        if (Projectile.Center.Distance(StartPos) > 4000) Projectile.velocity = Vector2.Zero; // so it still draws because of ProjectileID.Sets.DrawScreenCheckFluff[Type]

        /*var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ScourgeOfTheCorruptor);
        dust.noGravity = true;*/

        if (Projectile.timeLeft < 180)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 180f);
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return null;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer < 2) return false;
        Texture2D Texture = TextureAssets.Projectile[Type].Value;

        Vector2 drawPos = StartPos;
        int segmentCount = (int)(Projectile.Center.Distance(StartPos) / Texture.Width);
        int segmentsDrawn = 0;

        Vector2 StartToProj = StartPos.DirectionTo(Projectile.Center);

        while (segmentsDrawn < segmentCount)
        {
            drawPos += StartToProj * 32;
            Main.EntitySpriteDraw(Texture, drawPos - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);
            segmentsDrawn++;
        }

        Main.EntitySpriteDraw(Texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int duration = hit.Crit ? 300 : 60;
        target.AddBuff(BuffID.CursedInferno, duration);
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemEmerald, 2f);
    }
}
