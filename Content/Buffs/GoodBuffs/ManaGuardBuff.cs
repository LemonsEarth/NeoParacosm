using Terraria.Localization;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class ManaGuardBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += (int)(10 * MathF.Pow(player.GetElementalExpertiseBoost(BaseSpell.SpellElement.Pure), 2));
        player.endurance += 15f / 100f;
        player.GetDamage(DamageClass.Generic) -= 10f / 100f;
    }
}
