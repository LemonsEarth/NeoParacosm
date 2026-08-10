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
        Main.tileCut[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoFail[Type] = true;

        TileID.Sets.TileCutIgnore.Regrowth[Type] = true;
        TileID.Sets.IsVine[Type] = true;
        TileID.Sets.ReplaceTileBreakDown[Type] = true;
        TileID.Sets.VineThreads[Type] = true;

        AddMapEntry(new Color(160, 160, 160)); // Slightly darker than ExampleBlock

        DustType = DustID.Iron;
        HitSound = SoundID.Tink;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        // This method is used to make a vine tile draw in the wind. Note that i and j are reversed for this method, this is not a typo.
        Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);

        // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
        return false;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        offsetY = -2;
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
    {
        if (i % 2 == 0)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
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
