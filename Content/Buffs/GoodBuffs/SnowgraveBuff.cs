using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class SnowgraveBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
    }
}

public class SnowgravePlayer : ModPlayer
{
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HasBuff(BuffType<SnowgraveBuff>()))
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(
                    Player.GetSource_OnHurt(info.DamageSource),
                    Player.Center,
                    Vector2.Zero,
                    ProjectileType<SnowgraveProjectile>(),
                    (int)Player.GetDamage(DamageClass.Magic).ApplyTo(30),
                    5f,
                    Player.whoAmI,
                    ai1: 60
                    );
            }
        }
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (Player.HasBuff(BuffType<SnowgraveBuff>()))
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(
                    Player.GetSource_OnHurt(damageSource),
                    Player.Center,
                    Vector2.Zero,
                    ProjectileType<SnowgraveProjectile>(),
                    (int)Player.GetDamage(DamageClass.Magic).ApplyTo(100),
                    5f,
                    Player.whoAmI,
                    ai1: 180
                    );
            }
        }
    }
}
