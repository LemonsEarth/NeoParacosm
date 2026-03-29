namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class MagicMossball : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.ZoneJungle)
        {
            player.AddBuff(BuffID.ManaRegeneration, 2);
            player.AddBuff(BuffID.MagicPower, 2);
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.ManaCrystal);
        recipe1.AddIngredient(ItemID.Vine, 5);
        recipe1.AddIngredient(ItemID.JungleSpores, 8);
        recipe1.AddTile(TileID.WorkBenches);
        recipe1.Register();
    }
}
