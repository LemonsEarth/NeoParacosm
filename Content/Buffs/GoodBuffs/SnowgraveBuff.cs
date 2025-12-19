using Terraria.Localization;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;

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
