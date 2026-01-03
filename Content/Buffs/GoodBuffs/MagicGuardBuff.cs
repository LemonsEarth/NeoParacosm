using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class MagicGuardBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += (int)(5 * MathF.Pow(player.GetElementalExpertiseBoost(BaseSpell.SpellElement.Pure), 2));
        player.endurance += 5f / 100f;
    }
}
