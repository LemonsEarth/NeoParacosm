using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems;
using Newtonsoft.Json.Linq;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Tiles.Special;

public class DataCollectorTileEntity : ModTileEntity
{
    int timer = 0;

    public int dataCollected { get; private set; } = 0;

    int lastMessageSentID = -1;

    bool droppedReward = false;

    public void CollectData(int amount = 1)
    {
        if (lastMessageSentID != -1) Main.popupText[lastMessageSentID].active = false;
        dataCollected += amount;
        AdvancedPopupRequest t = new AdvancedPopupRequest();
        t.Color = Color.White;
        t.DurationInFrames = 60;
        t.Velocity = -Vector2.UnitY * 2;
        t.Text = dataCollected.ToString();
        Vector2 pos = (CenterPos - new Point16(0, 3)).ToWorldCoordinates();
        lastMessageSentID = PopupText.NewText(t, pos);

        if (dataCollected >= 10 && !droppedReward)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item.NewItem(new EntitySource_TileEntity(this, "Reached data goal"), CenterPos.ToWorldCoordinates(), ItemID.BloodButcherer, 1);
            }
            droppedReward = true;
        }
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

        if (timer % 30 == 0)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.Distance(Position.ToWorldCoordinates()) < distance)
                {
                    npc.NPBuffNPC().dataCollectorTEPos = Position;
                }
            }

        }

        foreach (var player in Main.ActivePlayers)
        {
            if (player.Distance(Position.ToWorldCoordinates()) < distance && (player.ZoneCorrupt || player.ZoneCrimson))
            {
                player.AddBuff(ModContent.BuffType<ProvokedPresenceDebuff>(), 2);
            }
        }

        for (int i = 0; i < 16; i++)
        {
            Vector2 pos = CenterPos.ToWorldCoordinates() + (Vector2.UnitY * distance).RotatedBy(i * MathHelper.ToRadians(360f / 16f) + MathHelper.ToRadians(timer));
            Dust.NewDustPerfect(pos, DustID.GemDiamond).noGravity = true;
        }
        timer++;
    }
}
