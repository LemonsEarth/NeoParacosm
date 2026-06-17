namespace NeoParacosm.Content.Items.Materials;

public class DivineFlesh : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.value = Item.sellPrice(0, 0, 0, 10);
        Item.rare = ItemRarityID.Red;
        Item.maxStack = 9999;
    }
}
