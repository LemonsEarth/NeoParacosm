using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.RecipeGroups;
using ReLogic.Content;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class CursedCandlestick : BaseCatalyst
{
    static readonly float fireDamageBoost = 25;
    static readonly float fireSpeedBoost = 25;
    static readonly float darkDamageBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireDamageBoost, fireSpeedBoost, darkDamageBoost);

    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 25;
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.mana = 22;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Fire] += fireDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire] += fireSpeedBoost / 100f;
            player.AddElementalDamageBoost(SpellElement.Dark, darkDamageBoost / 100f);
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<Candlestick>(), 1);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyTitaniumBar, 4);
        recipe.AddIngredient(ItemID.CursedFlame, 16);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}