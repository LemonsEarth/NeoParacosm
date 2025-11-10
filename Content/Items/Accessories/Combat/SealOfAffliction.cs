using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class SealOfAffliction : ModItem
{
    readonly float damageBoost = 4f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost);

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 42;
        Item.height = 42;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1, 0, 0);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        foreach (int buffType in player.buffType)
        {
            if (buffType != 0 && Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
            {
                player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
            }
        }
    }
}
