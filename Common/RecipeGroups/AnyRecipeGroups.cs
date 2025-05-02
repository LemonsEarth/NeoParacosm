using Terraria.Localization;

namespace NeoParacosm.Common.RecipeGroups
{
    public class AnyRecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            QuickRecipeGroup("AnyCopperBar", ItemID.CopperBar, ItemID.CopperBar, ItemID.TinBar);
            QuickRecipeGroup("AnySilverBar", ItemID.SilverBar, ItemID.SilverBar, ItemID.TungstenBar);
            QuickRecipeGroup("AnyGoldBar", ItemID.GoldBar, ItemID.GoldBar, ItemID.PlatinumBar);
            QuickRecipeGroup("AnyEvilBar", ItemID.DemoniteBar, ItemID.DemoniteBar, ItemID.CrimtaneBar);
            QuickRecipeGroup("AnyEvilMaterial", ItemID.ShadowScale, ItemID.ShadowScale, ItemID.TissueSample);
        }

        void QuickRecipeGroup(string name, int mainItem, params int[] validItems)
        {
            RecipeGroup group = new RecipeGroup(
                () => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(mainItem)}",
                validItems
            );

            RecipeGroup.RegisterGroup("NeoParacosm:" + name, group);
        }
    }
}
