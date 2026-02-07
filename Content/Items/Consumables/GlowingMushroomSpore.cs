using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Consumables;

public class GlowingMushroomSpore : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 14;
        Item.value = Item.buyPrice(0, 0, 0, 2);
        Item.rare = ItemRarityID.White;
        Item.ammo = Item.type;
        Item.shoot = ProjectileType<GlowingMushroomProj>();
        Item.consumable = true;
        Item.maxStack = 9999;
    }

    public override void AddRecipes()
    {
        CreateRecipe(20)
            .AddIngredient(ItemID.GlowingMushroom, 1)
            .AddCondition(Condition.TimeNight)
            .Register();
    }
}
