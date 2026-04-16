using NeoParacosm.Content.Items.Accessories.Combat.Defensive;
using NeoParacosm.Content.Items.Accessories.Combat.Melee;
using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class BeastGear : ModItem
{
    float flightTimeBoost = 33f;
    float enduranceBoost = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(flightTimeBoost, enduranceBoost);
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 32;
        Item.accessory = true;
        Item.defense = 10;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.GetLifePercent() >= 1f)
        {
            player.endurance += enduranceBoost / 100f;
        }
        player.GetModPlayer<HellhoundFangPlayer>().Active = true;
        player.spikedBoots += 2;
        player.noKnockback = true;
        player.wingTimeMax = (int)(player.wingTimeMax * (1 + flightTimeBoost / 100f));
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<ReinforcedTurtleShell>())
            .AddIngredient(ItemType<CutWyvernSegment>())
            .AddIngredient(ItemType<HellhoundFang>())
            .AddIngredient(ItemType<CorruptRatFur>())
            .AddIngredient(ItemID.TigerClimbingGear)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
