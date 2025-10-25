using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;

namespace NeoParacosm.Content.Tiles.DeadForest;

public class DeadTree : ModTree
{
    static Asset<Texture2D> treeTexture;
    private Asset<Texture2D> topsTexture;
    private Asset<Texture2D> branchTexture;

    public override void SetStaticDefaults()
    {
        GrowsOnTileId = [TileType<DeadDirtBlock>()];
        treeTexture = Request<Texture2D>("NeoParacosm/Content/Tiles/DeadForest/DeadTree");
        topsTexture = Request<Texture2D>("NeoParacosm/Content/Tiles/DeadForest/DeadTree_Tops");
        branchTexture = Request<Texture2D>("NeoParacosm/Content/Tiles/DeadForest/DeadTree_Branches");
    }

    public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
    {
        UseSpecialGroups = true,
        SpecialGroupMinimalHueValue = 11f / 72f,
        SpecialGroupMaximumHueValue = 0.25f,
        SpecialGroupMinimumSaturationValue = 0.88f,
        SpecialGroupMaximumSaturationValue = 1f
    };

    public override int SaplingGrowthType(ref int style)
    {
        style = 0;
        return TileType<DeadTreeSapling>();
    }

    public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
    {
        
    }

    public override Asset<Texture2D> GetBranchTextures()
    {
        return branchTexture;
    }

    public override Asset<Texture2D> GetTopTextures()
    {
        return topsTexture;
    }

    public override Asset<Texture2D> GetTexture()
    {
        return treeTexture;
    }

    public override int DropWood()
    {
        return ItemID.None;
    }
}
