using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using NeoParacosm.Core.Systems;

namespace NeoParacosm.Content.Tiles.Special;

public class DragonRemainsTileEntity : ModTileEntity
{
    int timer = 0;
    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<DragonRemainsTile>();
    }

    Point16 CenterPos => (Position + new Point16(7, 4));
    float pushForce = 3;
    float distance = 500;

    public override void Update()
    {
        if (timer == 0)
        {
            WorldGenSystem.DragonRemainsTileEntityPos = Position;
        }
        if (!NPC.downedBoss2)
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Distance(CenterPos.ToWorldCoordinates()) < distance)
                {
                    player.velocity += CenterPos.ToWorldCoordinates().DirectionTo(player.Center) * pushForce;
                    player.RemoveAllGrapplingHooks();
                }
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = CenterPos.ToWorldCoordinates() + (Vector2.UnitY * distance).RotatedBy(i * (MathHelper.ToRadians(360f / 32f)));
                Dust.NewDust(pos, 2, 2, DustID.HallowedTorch);
            }
        }
        timer++;
    }
}
