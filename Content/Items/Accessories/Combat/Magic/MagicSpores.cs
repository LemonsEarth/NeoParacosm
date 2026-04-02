using Mono.Cecil;
using NeoParacosm.Core.Players;
using static System.Net.Mime.MediaTypeNames;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class MagicSpores : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 0, 50);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.statManaMax2 += 20;
        if (Main.myPlayer == player.whoAmI && NPPlayer.timer % 10 == 0 && player.velocity.LengthSquared() > 5 * 5)
        {
            Projectile.NewProjectileDirect(
                player.GetSource_Accessory(Item),
                player.Center,
                Vector2.UnitY.RotatedByRandom(6.28f) * 0.2f,
                Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1),
                10,
                0f,
                player.whoAmI
                );
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.ManaCrystal);
        recipe1.AddIngredient(ItemID.JungleSpores, 5);
        recipe1.AddTile(TileID.WorkBenches);
        recipe1.Register();
    }
}