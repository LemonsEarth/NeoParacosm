using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Death;

public class HolyFire : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.extraUpdates = 5;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item34 with { PitchRange = (0f, 0.3f), Volume = 0.75f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item66 with { PitchRange = (0.5f, 1f), Volume = 0.75f }, Projectile.Center);
        }

        Lighting.AddLight(Projectile.Center, 0.8f, 0.8f, 0f);
        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz, Scale: Main.rand.NextFloat(1.5f, 2f)).noGravity = true;
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, Scale: Main.rand.NextFloat(1.5f, 2f)).noGravity = true;
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }
}
