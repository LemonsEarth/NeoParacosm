using Terraria.Localization;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class ForestCrestBuff : ModBuff
{
    readonly float speedBoost = 10f;
    readonly float damageBoost = 10f;
    public override LocalizedText Description => base.Description.WithFormatArgs(speedBoost, damageBoost);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed += 10f / 100f;
        player.GetDamage(DamageClass.Summon) += 10f / 100f;
    }
}
