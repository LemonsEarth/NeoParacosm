

using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class SnowgraveProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Duration => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 1f, 3f, 5f);
        }

        Projectile.Opacity = 1 - AITimer / 60f;
        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<SnowgraveDebuff>(), (int)(Duration * Projectile.GetOwner().GetElementalExpertiseBoost(BaseSpell.SpellElement.Ice)));
    }

    public override void OnKill(int timeLeft)
    {
        //SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 7f, color: Color.LightBlue);
    }
}
