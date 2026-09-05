using NeoParacosm.Content.NPCs.Friendly.Special;
using NeoParacosm.Core.UI.Expeditions;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Special.Expeditions;

// This file contains the tile, tile entity and item
public class ExpeditionBoard : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        MinPick = 0;
        MineResist = 6f;
        DustType = DustID.GemSapphire;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.Width = 5;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];

        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 5, 0);
        TileObjectData.addTile(Type);

        AddMapEntry(Color.DarkSlateBlue);
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.8f;
        g = 0.8f;
        b = 0.8f;
    }

    public override bool RightClick(int i, int j)
    {
        ExpeditionUISystem system = GetInstance<ExpeditionUISystem>();
        if (system.userInterface.CurrentState == null)
        {
            system.ShowUI();
        }
        else
        {
            system.HideUI();
        }
        return true;
    }
}

public class ExpeditionBoardItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<ExpeditionBoard>());
        Item.width = 80;
        Item.height = 80;
        Item.rare = ItemRarityID.Green;
    }
}
