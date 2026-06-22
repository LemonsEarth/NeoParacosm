namespace NeoParacosm.Content.Items.Tools;

public class GiantPickaxe : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 999;
    }

    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.DamageType = DamageClass.Melee;
        Item.width = 128;
        Item.height = 128;
        // On the official wiki, https://terraria.wiki.gg/wiki/Pickaxes, the "Use time" column corresponds to Item.useAnimation and the "Mining speed" column corresponds to Item.useTime.
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 15;
        Item.value = Item.sellPrice(silver: 2);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.pick = 59;
        Item.tileBoost = 10;
        Item.scale = 2;
    }

    public override void HoldItem(Player player)
    {

    }

    public override void UpdateInventory(Player player)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.GoldPickaxe, 10);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
