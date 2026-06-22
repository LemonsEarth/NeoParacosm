using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.RecipeGroups;
using ReLogic.Content;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class GoldenEye : BaseCatalyst
{
    static readonly float fireDamageBoost = 15;
    static readonly float fireSpeedBoost = 15;
    static readonly float darkExpBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireDamageBoost, fireSpeedBoost, darkExpBoost);

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

    public override void HoldItem(Player player)
    {
        player.AddBuff(BuffID.Ichor, 180);
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Fire] += fireDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire] += fireSpeedBoost / 100f;
            player.AddElementalExpertiseBoost(SpellElement.Dark, darkExpBoost / 100f);
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.HallowedBar, 8);
        recipe.AddIngredient(ItemID.SoulofSight, 5);
        recipe.AddIngredient(ItemID.Ichor, 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}