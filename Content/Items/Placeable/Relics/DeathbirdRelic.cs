namespace NeoParacosm.Content.Items.Placeable.Relics;

// Common code for a Master Mode boss relic
// Supports optional Item.placeStyle handling if you wish to add more relics but use the same tile type (then it would be wise to name this class something more generic like BossRelic)
// If you want to add more relics but don't want to use the Item.placeStyle approach, see the inheritance example at the bottom of the file
public class DeathbirdRelic : BossRelic
{
    // Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
    // If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture
    public override string RelicTextureName => "NeoParacosm/Content/Items/Placeable/Relics/DeathbirdRelic";
}

public class DeathbirdRelicItem : ModItem
{
    public override void SetDefaults()
    {
        // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle as well as setting a few values that are common across all placeable items
        // The place style (here by default 0) is important if you decide to have more than one relic share the same tile type (more on that in the tiles' code)
        Item.DefaultToPlaceableTile(TileType<DeathbirdRelic>(), 0);

        Item.width = 30;
        Item.height = 48;
        Item.rare = ItemRarityID.Master;
        Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
        Item.value = Item.buyPrice(0, 1);
    }
}
