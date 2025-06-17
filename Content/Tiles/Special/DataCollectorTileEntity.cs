using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems;
using Newtonsoft.Json.Linq;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Tiles.Special;

public class DataCollectorTileEntity : ModTileEntity
{
    int timer = 0;

    int dataCollected = 0;

    public void CollectData()
    {
        dataCollected++;
        AdvancedPopupRequest t = new AdvancedPopupRequest();
        t.Color = Color.White;
        t.DurationInFrames = 60;
        t.Velocity = -Vector2.UnitY * 2;
        t.Text = dataCollected.ToString();
        Vector2 pos = (CenterPos - new Point16(0, 2)).ToWorldCoordinates();
        PopupText.NewText(t, pos);
    }

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<DataCollectorTile>();
    }

    Point16 CenterPos => (Position + new Point16(2, 2));
    float distance = 500;

    public override void Update()
    {
        if (timer == 0)
        {
            WorldGenSystem.DragonRemainsTileEntityPos = Position;
        }

        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.Distance(Position.ToWorldCoordinates()) < distance)
            {
                npc.NPBuffNPC().dataCollectorTEPos = Position;
            }
        }

        Main.NewText(dataCollected);

        for (int i = 0; i < 16; i++)
        {
            Vector2 pos = CenterPos.ToWorldCoordinates() + (Vector2.UnitY * distance).RotatedBy(i * MathHelper.ToRadians(360f / 16f) + MathHelper.ToRadians(timer));
            Dust.NewDustPerfect(pos, DustID.GemDiamond).noGravity = true;
        }
        timer++;
    }
}
