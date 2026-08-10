using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class WornChain : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileCut[Type] = false;
        Main.tileMergeDirt[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileRope[Type] = true;
        Main.tileFrameImportant[Type] = false;

        AddMapEntry(new Color(160, 160, 160)); // Slightly darker than ExampleBlock

        DustType = DustID.Iron;
        HitSound = SoundID.Tink;
    }

    public override bool CanDrop(int i, int j)
    {
        return false;
    }
}

public class WornChainItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Chain;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<WornChain>());
        Item.width = 10;
        Item.height = 20;
    }
}
