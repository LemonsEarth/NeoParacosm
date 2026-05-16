using NeoParacosm.Content.Items.Materials;
using System.Collections.Generic;

namespace NeoParacosm.Content.Items.Placeable.Tiles.Ice;

public class FrozenFossilBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        //Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Item50;
        MineResist = 2;
        MinPick = 55;

        DustType = DustID.Ice;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        AddMapEntry(new Color(70, 70, 200));
    }

    public override void RandomUpdate(int i, int j)
    {

    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }
}

public class FrozenFossilItem : ModItem
{
    public static List<int> Items;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.DesertFossil;
        Item.ResearchUnlockCount = 1000;
        ItemID.Sets.ExtractinatorMode[Type] = Type;

        Items = new List<int>()
        {
            ItemID.LeadOre, ItemID.SilverOre, ItemID.PlatinumOre,
            ItemID.Amethyst, ItemID.Sapphire, ItemID.Diamond,
            ItemType<FrigidFossil>()
        };
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<FrozenFossilBlock>());
        Item.width = 16;
        Item.height = 16;
    }

    public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
    {
        if (Main.rand.NextBool(8))
        {
            resultType = Main.rand.NextFromList(ItemID.Amethyst, ItemID.Sapphire, ItemID.Diamond);
        }
        else if (Main.rand.NextBool(16))
        {
            resultType = ItemType<FrigidFossil>();
        }
        else
        {
            resultType = Main.rand.NextFromList(ItemID.LeadOre, ItemID.SilverOre, ItemID.PlatinumOre);
        }
        resultStack = Main.rand.Next(1, 3);
    }

    public override void AddRecipes()
    {

    }
}
