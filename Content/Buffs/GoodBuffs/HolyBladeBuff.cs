using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Localization;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class HolyBladeBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        
    }
}
