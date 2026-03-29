using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class ManastoneTablet : ModItem
{
    readonly float pureMagicSpellExpertiseBoost = 15f;
    readonly float earthSpellExpertiseBoost = 15f;
    readonly int manaBoost = 40;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(pureMagicSpellExpertiseBoost, earthSpellExpertiseBoost, manaBoost);
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddElementalExpertiseBoost(SpellElement.Pure, pureMagicSpellExpertiseBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Earth, earthSpellExpertiseBoost / 100f);
        player.statManaMax2 += manaBoost;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.ManaCrystal)
            .AddIngredient(ItemID.Book, 3)
            .AddIngredient(ItemID.StoneBlock, 20)
            .AddTile(TileID.Furnaces)
            .Register();
    }
}
