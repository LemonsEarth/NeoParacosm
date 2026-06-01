using NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class SnakeWhip : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToWhip(ProjectileType<SnakeWhipProjectile>(), 20, 2, 4);
        Item.rare = ItemRarityID.Green;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.GetModPlayer<SnakeWhipPlayer>().UseCount++;
        if (player.GetModPlayer<SnakeWhipPlayer>().UseCount >= 3)
        {
            player.GetModPlayer<SnakeWhipPlayer>().UseCount = 0;
        }
        return true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.MysticCoilSnake, 1);
        recipe.AddIngredient(ItemID.Amber, 6);
        recipe.AddIngredient(ItemID.FossilOre, 12);
        recipe.AddTile(TileID.Extractinator);
        recipe.Register();
    }

    public override bool MeleePrefix()
    {
        return true;
    }
}

public class SnakeWhipPlayer : ModPlayer
{
    public int UseCount { get; set; } = 0;

    public override void ResetEffects()
    {

    }
}
