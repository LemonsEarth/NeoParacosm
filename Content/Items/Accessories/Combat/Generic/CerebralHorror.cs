using NeoParacosm.Content.Items.Materials;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class CerebralHorror : ModItem
{
    readonly float damageBoost = 10f;
    readonly float critBoost = 10f;
    readonly float drBoost = 10f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost, drBoost);
    public override void SetDefaults()
    {
        Item.width = 74;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        float sum = LemonUtils.GetLightingAroundPos(player.Center, 1);

        if (sum <= 2)
        {
            player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
            player.GetCritChance(DamageClass.Generic) += critBoost;
            player.endurance += drBoost / 100f;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrankensteinsHead>(), 1);
        recipe.AddIngredient(ItemType<SwampThingsHead>(), 1);
        recipe.AddIngredient(ItemType<CreatureFromTheDeepsHead>(), 1);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}
