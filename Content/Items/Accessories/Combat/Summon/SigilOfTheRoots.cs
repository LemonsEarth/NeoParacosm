using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Accessories.Combat.Summon;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class SigilOfTheRoots : ModItem
{
    static readonly int sentryBoost = 2;
    static readonly float moveSpeedBoost = 12;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(sentryBoost, moveSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 4);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        bool allActive = true;
        for (int i = 0; i < 3; i++)
        {
            if (player.armor[i].IsAir)
            {
                player.maxMinions++;
            }
            else
            {
                allActive = false;
            }
        }

        if (allActive)
        {
            player.maxTurrets += 1;
        }

        player.maxTurrets += sentryBoost;
        player.moveSpeed += moveSpeedBoost / 100;
        player.GetModPlayer<AshenForestCrestPlayer>().forestCrest = true;
    }


    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<AshenForestCrest>(), 1);
        recipe.AddIngredient(ItemType<OldWaysTalisman>(), 1);
        recipe.AddIngredient(ItemID.HellstoneBar, 10);
        recipe.AddIngredient(ItemID.Bone, 30);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
