using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class VilethornFriendly : ModProjectile
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
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            //Projectile.Center = StartPos + Projectile.velocity.SafeNormalize(Vector2.Zero);
            StartPos = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        Projectile.velocity *= 0.90f;
        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ScourgeOfTheCorruptor);
        dust.noGravity = true;

        if (Projectile.timeLeft < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        }

        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = StartPos + StartPos.DirectionTo(Projectile.Center) * StartPos.Distance(Projectile.Center);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), StartPos, endPos, Projectile.width, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (StartPos == Vector2.Zero)
        {
            return false;
        }
        Texture2D Texture = TextureAssets.Projectile[Type].Value;
        Rectangle TipTexture = Texture.Frame(1, 3, 0, 0);
        Rectangle BodyTexture1 = Texture.Frame(1, 3, 0, 1);
        Rectangle BodyTexture2 = Texture.Frame(1, 3, 0, 2);

        Vector2 drawPos = StartPos;
        int segmentCount = (int)(Projectile.Center.Distance(StartPos) / Texture.Width);
        int segmentsDrawn = 0;

        Vector2 StartToProj = StartPos.DirectionTo(Projectile.Center);

        while (segmentsDrawn < segmentCount)
        {
            drawPos += StartToProj * 18;
            Main.EntitySpriteDraw(Texture, drawPos - Main.screenPosition, BodyTexture1, lightColor * Projectile.Opacity * ((float)(segmentsDrawn + 1) / segmentCount), Projectile.rotation, new Vector2(9, 9), Projectile.scale, SpriteEffects.None);
            segmentsDrawn++;
        }

        Main.EntitySpriteDraw(Texture, Projectile.Center - Main.screenPosition, TipTexture, lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(9, 9), Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}
