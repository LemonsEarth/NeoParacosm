namespace NeoParacosm.Content.Items.Misc;

public class GateKey : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 46;
        Item.value = Item.sellPrice(0, 0, 0, 0);
        Item.rare = ItemRarityID.Yellow;
    }
}