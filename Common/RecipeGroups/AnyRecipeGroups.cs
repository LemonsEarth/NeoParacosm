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
            QuickRecipeGroup("AnyMythrilBar", ItemID.MythrilBar, ItemID.MythrilBar, ItemID.OrichalcumBar);
            QuickRecipeGroup("AnyTitaniumBar", ItemID.TitaniumBar, ItemID.TitaniumBar, ItemID.AdamantiteBar);
            QuickRecipeGroup("AnyEvilBar", ItemID.DemoniteBar, ItemID.DemoniteBar, ItemID.CrimtaneBar);
            QuickRecipeGroup("AnyEvilMaterial", ItemID.ShadowScale, ItemID.ShadowScale, ItemID.TissueSample);
            QuickRecipeGroup("AnyEvilMaterial2", ItemID.Ichor, ItemID.Ichor, ItemID.CursedFlame);
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
