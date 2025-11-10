using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Tiles.Special;

public class DragonRemainsTileEntity : ModTileEntity
{
    int timer = 0;
    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == TileType<DragonRemainsTile>();
    }

    Point16 CenterPos => (Position + new Point16(7, 4));
    float pushForce = 3;
    float distance = 500;

    public override void Update()
    {
        if (timer == 0)
        {
            ResearcherQuest.DragonRemainsTileEntityPos = Position;
        }
        if (ResearcherQuest.Progress <= ResearcherQuest.ProgressState.NotDownedEvilBoss)
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Distance(CenterPos.ToWorldCoordinates()) < distance)
                {
                    player.velocity += CenterPos.ToWorldCoordinates().DirectionTo(player.Center) * pushForce;
                    player.RemoveAllGrapplingHooks();
                }
            }
            if (timer % 600 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    Projectile.NewProjectile(new EntitySource_TileEntity(this, "Shield Pulse"), CenterPos.ToWorldCoordinates(), Vector2.Zero, ProjectileType<DragonRemainsPulseShield>(), 0, 0, Main.myPlayer);
                }
            }
        }

        if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.DownedPlantera)
        {
            WorldGen.KillTile(Position.X, Position.Y, false, false, true);
        }
        timer++;
    }
}
