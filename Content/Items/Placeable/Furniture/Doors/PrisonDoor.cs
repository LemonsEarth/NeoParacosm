using NeoParacosm.Content.Items.Misc;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Furniture.Doors;

public class PrisonDoorClosed : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = false;
        TileID.Sets.NotReallySolid[Type] = true;
        TileID.Sets.DrawsWalls[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        DustType = DustID.Ash;

        // Names
        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

        // Placement
        // In addition to copying from the TileObjectData.Something templates, modders can copy from specific tile types. CopyFrom won't copy subtile data, so style specific properties won't be copied, such as how Obsidian doors are immune to lava.
        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
        TileObjectData.addTile(Type);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override bool RightClick(int i, int j)
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.ConsumeItem(ItemType<GrayKey>()))
            {
                WorldGen.KillTile(i, j);
                return true;
            }
        }

        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 4;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ItemType<GrayKey>();
    }
}

public class PrisonDoor : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<PrisonDoorClosed>());
        Item.width = 14;
        Item.height = 28;
        Item.value = 150;
    }
}
