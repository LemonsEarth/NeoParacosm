namespace NeoParacosm.Content.Tiles.Relics;

// Common code for a Master Mode boss relic
// Supports optional Item.placeStyle handling if you wish to add more relics but use the same tile type (then it would be wise to name this class something more generic like BossRelic)
// If you want to add more relics but don't want to use the Item.placeStyle approach, see the inheritance example at the bottom of the file
public class DeathbirdRelic : BossRelic
{
    // Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
    // If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture
    public override string RelicTextureName => "NeoParacosm/Content/Tiles/Relics/DeathbirdRelic";
}