using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Special.Spawners.Goblins;

public class GoblinScoutWatchtowerSpawnerTile : SpawnerTile
{
    public override SpawnerTileEntity TileEntityType => GetInstance<GoblinScoutWatchtowerSpawnerTileEntity>();
}

public class GoblinScoutWatchtowerSpawnerTileItem : SpawnerTileItem
{
    public override int TileType => TileType<GoblinScoutWatchtowerSpawnerTile>();
}

public class GoblinScoutWatchtowerSpawnerTileEntity : SpawnerTileEntity
{
    public override int NPCType => NPCID.GoblinScout;
    public override int TileType => TileType<GoblinScoutWatchtowerSpawnerTile>();
    public override int ActivationDistance => 500;
    public override int DespawnDistance => 1200;

    public override bool NeedsLineOfSight => true;

    public override bool IsDisabled()
    {
        return DisabledTimer > 0;
    }

    public override void OnNPCDeath()
    {
        DisableSpawner();
    }
}
