using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon;

public class ArcaneStoneProjectile : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    int AITimer = 0;
    ref float ScaleMul => ref Projectile.ai[0];
    ref float SizeX => ref Projectile.ai[1];
    ref float SizeY => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.MinionShot[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = 3;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.timeLeft = 10;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            float dustFactor = SizeX / Projectile.width;
            LemonUtils.DustBurst(8, Projectile.Center, DustType<FireDust>(), 1 * dustFactor, 1 * dustFactor, 0.5f * dustFactor, 1f * dustFactor, Color.Red);
            Projectile.Resize((int)SizeX * 3, (int)SizeY * 3);
        }
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}
