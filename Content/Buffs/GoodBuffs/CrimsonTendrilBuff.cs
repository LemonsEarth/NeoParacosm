using Terraria.Localization;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class CrimsonTendrilBuff : ModBuff
{
    /*readonly float speedBoost = 10f;
    readonly float damageBoost = 10f;
    public override LocalizedText Description => base.Description.WithFormatArgs(speedBoost, damageBoost);*/

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        
    }
}
