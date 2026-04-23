using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class AncientDragonScale : ModItem
{
    readonly float fireLightningDamageBoost = 20f;
    readonly float fireLightningExpertiseBoost = 15f;
    readonly float drBoost = 5f;
    readonly float drLowHPBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireLightningDamageBoost, fireLightningExpertiseBoost, drBoost, drLowHPBoost);
    public override void SetDefaults()
    {
        Item.width = 58;
        Item.height = 60;
        Item.accessory = true;
        Item.defense = 5;
        Item.value = Item.buyPrice(0, 10);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddElementalDamageBoost(SpellElement.Fire, fireLightningDamageBoost / 100f);
        player.AddElementalDamageBoost(SpellElement.Lightning, fireLightningDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Fire, fireLightningExpertiseBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, fireLightningExpertiseBoost / 100f);

        player.endurance += drBoost / 100f;
        if (player.GetLifePercent() <= 0.5f)
        {
            player.endurance += drLowHPBoost / 100f;
        }
    }
}
