using NeoParacosm.Content.NPCs.Friendly.Special;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Machines;

// This file contains the tile, tile entity and item
public class GoblinHut : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        MinPick = 0;
        MineResist = 6f;
        DustType = DustID.Hay;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];

        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 2, 0);
        TileObjectData.newTile.HookPostPlaceMyPlayer = GetInstance<GoblinHutTileEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(110, 80, 0));
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


    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        GetInstance<GoblinHutTileEntity>().Kill(i, j);
    }
}

public class GoblinHutTileEntity : ModTileEntity
{
    int timer = 0;

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == TileType<GoblinHut>();
    }

    Point16 CenterPos => Position + new Point16(2, 3);

    public override void Update()
    {
        if (timer % 600 == 0)
        {
            int thisTECount = 0;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCType<FriendlyGoblin>() && npc.ai[1] == ID)
                {
                    thisTECount++;
                }
            }

            if (thisTECount < 5 && NPC.CountNPCS(NPCType<FriendlyGoblin>()) < 20)
            {
                NPC npc = NPC.NewNPCDirect(
                     new EntitySource_TileEntity(this, "NeoParacosm:GoblinHutSpawn"),
                    CenterPos.ToWorldCoordinates(),
                    NPCType<FriendlyGoblin>(),
                    ai0: 3000, ai1: ID);

            }
        }

        timer++;
    }
}

public class GoblinHutItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<GoblinHut>());
        Item.width = 64;
        Item.height = 80;
        Item.rare = ItemRarityID.Orange;
    }
}
