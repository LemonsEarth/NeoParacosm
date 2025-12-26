using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Projectiles.Effect;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil;

public class GiantMeatball : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float LostSoulWaitTime => ref Projectile.ai[1];
    ref float LostSoulTimeLeft => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.Opacity = 1f;
    }

    int spinDir = -1;
    float savedSpeed = 1f;
    public override void AI()
    {
        if (TimeLeft == 0)
        {
            TimeLeft = 30;
        }
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }
        Projectile.velocity = Main.rand.NextVector2Unit();
        int pulseInterval = (int)TimeLeft / 4;
        if (AITimer >= TimeLeft / 2)
        {
            if (AITimer % pulseInterval == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f), Volume = 0.5f }, Projectile.Center);
                Projectile.scale = 0.7f;
            }
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 1 / 40f);
        }
        else
        {
            Projectile.scale = Math.Clamp(AITimer / (30f), 0, 1f);

        }

        Projectile.Opacity = AITimer / 15f;

        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        Dust.NewDustDirect(Projectile.RandomPos(0, 0), 2, 2, DustID.RedMoss, 0, Main.rand.NextFloat(5, 10), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        Dust.NewDustDirect(Projectile.RandomPos(0, 0), 2, 2, DustID.Crimson, 0, Main.rand.NextFloat(5, 10), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        LemonUtils.DrawGlow(Projectile.Center, Color.Red, Projectile.Opacity, Projectile.scale * 10);
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture,
                drawPos - Main.screenPosition,
                null,
                Color.Red * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * Projectile.Opacity,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length),
                SpriteEffects.None);
        }
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        LemonUtils.QuickPulse(Projectile, Projectile.Center, 1.5f, 15, 5, Color.Red);
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.Crimson, 2f);
        for (int i = 0; i < 8; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * savedSpeed, ProjectileType<CrimsonLostSoul>(), ai0: LostSoulWaitTime, ai1: LostSoulTimeLeft);
        }
    }
}
