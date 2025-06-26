using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalProjectiles;

public class LightningFriendly : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    bool friendly = false;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type == ProjectileID.CultistBossLightningOrbArc;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "CrimsonCloud")
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
        if (friendly)
        {
            return Color.Red;
        }
        return null;
    }

    public override void AI(Projectile projectile)
    {

    }
}
