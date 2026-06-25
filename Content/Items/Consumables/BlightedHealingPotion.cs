using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;

namespace NeoParacosm.Content.Items.Consumables;

public class BlightedHealingPotion : ModItem
{
    public override void SetStaticDefaults()
    {
        //AmmoID.Sets.IsSpecialist[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 36;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.Yellow;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.healLife = 150;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.potion = true;
    }

    public override bool? UseItem(Player player)
    {
        for (int i = 0; i < player.buffType.Length; i++)
        {
            var buffType = player.buffType[i];

            if (buffType > 0 && Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
            {
                player.ClearBuff(buffType);
            }
        }
        return null;
    }

    public override void ModifyPotionDelay(Player player, ref int baseDelay)
    {
        baseDelay += 15 * 60;
    }

    public override void AddRecipes()
    {
        CreateRecipe(1)
            .AddIngredient(ItemID.GreaterHealingPotion, 1)
            .AddRecipeGroup(AnyRecipeGroups.AnySoulOfBlight, 5)
            .AddTile(TileID.AlchemyTable)
            .Register();
    }
}