
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Researcher;

public class SavDrone : ModProjectile
{
    int AITimer = 0;
    ref float TimeToFire => ref Projectile.ai[0];
    ref float IndicatorLength => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 500;
        Projectile.scale = 1f;
    }

    float dustDistance = 64;
    public override void AI()
    {
        if (AITimer == 0)
        {
            if (IndicatorLength == 0) IndicatorLength = 1;
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.Zombie67 with { PitchRange = (0f, 0.2f) }, Projectile.Center);
        }
        if (AITimer < 60) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 60) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        Lighting.AddLight(Projectile.Center, 1, 1, 0);
        if (AITimer == TimeToFire)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.Electric, 3);
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 24, Vector2.Zero, ProjectileType<SavLaser>(), ai0: 0.1f, ai1: Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            }
            Projectile.velocity += -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2;
        }
        Projectile.velocity *= 0.96f;
        if (AITimer < TimeToFire)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        }
        if (AITimer > TimeToFire - 60 && AITimer < TimeToFire)
        {
            Vector2 randomPos = Projectile.RandomPos(dustDistance, dustDistance);
            dustDistance--;
            float randSpeed = Main.rand.NextFloat(3, 8f);
            Dust.NewDustDirect(randomPos, 2, 2, DustID.Electric,
                randomPos.DirectionTo(Projectile.Center).X * randSpeed,
                randomPos.DirectionTo(Projectile.Center).Y * randSpeed).noGravity = true;
        }
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
        Texture2D texture = TextureAssets.Projectile[ProjectileType<DeathbirdFeather>()].Value;
        if (AITimer >= TimeToFire)
        {
            return true;
        }
        // Indicator
        Vector2 indicatorOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
        float indicatorPercentComplete = AITimer / TimeToFire;
        float indicatorScale = indicatorPercentComplete * 5 * IndicatorLength;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White * indicatorPercentComplete * 0.35f, Projectile.rotation - MathHelper.PiOver2, indicatorOrigin, new Vector2(1, indicatorScale), SpriteEffects.None, 0);
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
