using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalProjectiles.Friendlify;

public class LightningFriendly : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    bool friendly = false;

    int AITimer = 0;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type == ProjectileID.CultistBossLightningOrbArc;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "Friendly")
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            friendly = true;
        }
    }

    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        return null;
    }

    public override void AI(Projectile projectile)
    {
        if (AITimer == 0 && friendly)
        {
            SoundEngine.PlaySound(SoundID.Thunder, projectile.Center);
        }
    }
}
