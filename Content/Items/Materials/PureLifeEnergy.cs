namespace NeoParacosm.Content.Items.Materials;

public class PureLifeEnergy : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 46;
        Item.value = Item.sellPrice(0, 0, 0, 20);
        Item.rare = ItemRarityID.Green;
        Item.maxStack = 9999;
    }
}
