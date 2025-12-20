using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Items.Armor.Generic.Stone;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Misc;
using System.Collections.Generic;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalItems;

public class NPBuffItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.shoot == ProjectileID.None;
    }

    public override bool? UseItem(Item item, Player player)
    {

        if (player.HasBuff(BuffType<HolyBladeBuff>()) && item.shoot == ProjectileID.None && player.ItemAnimationJustStarted)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                float maxDamage = 0;
                if (player.GetElementalExpertiseBoost(BaseSpell.SpellElement.Holy) > 1.5f)
                {
                    maxDamage = item.damage;
                }
                else
                {
                    maxDamage = item.damage * 0.25f;
                }
                Vector2 velocity = player.DirectionTo(Main.MouseWorld) * 8 * player.GetElementalExpertiseBoost(BaseSpell.SpellElement.Holy);
                float damage = player.GetDamage(DamageClass.Generic).ApplyTo(item.damage * (player.GetElementalDamageBoost(BaseSpell.SpellElement.Holy) + 1));
                damage = MathHelper.Clamp(damage, 5, maxDamage);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, velocity, ProjectileType<HolySlash>(), (int)damage, 2f, player.whoAmI, ai1: 0.95f, ai2: 30);
            }
        }
        return null;
    }
}
