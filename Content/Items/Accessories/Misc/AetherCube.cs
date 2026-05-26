using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class AetherCube : ModItem
{
    int standingStillTimer = 0;
    public override void SetDefaults()
    {
        Item.width = 56;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (MathF.Abs(player.velocity.X) <= 0.02f)
        {
            standingStillTimer++;
            if (standingStillTimer > 180)
            {
                player.AddBuff(BuffID.Invisibility, 180);
                if (!hideVisual && standingStillTimer < 360)
                {
                    player.AddBuff(BuffID.Shimmer, 2);
                }
            }
        }
        else
        {
            standingStillTimer = 0;
        }

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<GhostCube>(), 1);
        recipe.AddIngredient(ItemID.FallenStar, 3);
        recipe.AddCondition(Condition.NearShimmer);
        recipe.Register();
    }
}
