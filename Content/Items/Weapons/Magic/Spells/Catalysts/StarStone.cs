namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class StarStone : BaseCatalyst
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 8;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.mana = 12;
        Item.noMelee = false;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.StoneBlock, 20);
        recipe.AddIngredient(ItemID.FallenStar, 1);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}