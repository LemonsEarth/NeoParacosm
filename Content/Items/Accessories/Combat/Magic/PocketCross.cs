using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class PocketCross : ModItem
{
    readonly float holyDamageBoost = 15f;
    readonly float holyExpertiseBoost = 15f;
    readonly float drBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(holyDamageBoost, holyExpertiseBoost, drBoost);
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddElementalDamageBoost(SpellElement.Holy, holyDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Holy, holyExpertiseBoost / 100f);
        if (player.GetLifePercent() <= 0.5f)
        {
            player.endurance += drBoost / 100f;
        }
    }
}
