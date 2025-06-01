using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class MarksmanBolt : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.timeLeft = 240;
        Projectile.penetrate = 3;
        Projectile.tileCollide = true;
        Projectile.Opacity = 0f;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.extraUpdates = 50;
    }

    public override void AI()
    {
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Scale: 1.5f).noGravity = true;
        }

        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item21 with { PitchRange = (-0.2f, 0.2f)}, Projectile.Center);
        }
        
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.BrokenArmor, 600);
        target.AddBuff(BuffID.Bleeding, 1200);
    }
}
