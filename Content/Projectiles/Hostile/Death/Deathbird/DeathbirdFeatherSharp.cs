
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;

public class DeathbirdFeatherSharp : ModProjectile
{
    int AITimer = 0;

    ref float targetIndex => ref Projectile.ai[0];
    ref float rotDuration => ref Projectile.ai[1];
    ref float retargetInterval => ref Projectile.ai[2];

    float savedSpeed = 0;
    Vector2 savedVelocity = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 34;
        Projectile.height = 34;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 240;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.DD2_BetsySummon with { PitchRange = (0f, 0.2f) }, Projectile.Center);
            savedSpeed = Projectile.velocity.Length();
        }
        Player player = Main.player[(int)targetIndex];
        if (AITimer == retargetInterval)
        {
            savedVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
        }
        else if (AITimer > retargetInterval && AITimer < retargetInterval + rotDuration)
        {
            if (player.Alive())
            {
                Projectile.velocity = Vector2.Zero;
                float t = (AITimer - retargetInterval) / rotDuration;
                Projectile.rotation = savedVelocity.ToRotation().AngleLerp(Projectile.DirectionTo(player.Center).ToRotation(), t) + MathHelper.PiOver2;
            }
        }
        else if (AITimer >= retargetInterval + rotDuration)
        {
            Projectile.velocity = Projectile.DirectionTo(player.Center) * savedSpeed;
            AITimer = 0;
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        if (Projectile.timeLeft < 60)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 60f);
        }

        Lighting.AddLight(Projectile.Center, 1, 1, 0);
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
            LemonUtils.DrawGlow(drawPos, Color.White, Projectile.Opacity, Projectile.scale * 0.4f);
            Color color = i == 0 ? Color.Black : Color.White;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color * ((14 - i) / (float)Projectile.oldPos.Length) * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        }
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
