using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Special.Spawners.Goblins;

public class GoblinScoutSpawnerTile : SpawnerTile
{
    public override SpawnerTileEntity TileEntityType => GetInstance<GoblinScoutSpawnerTileEntity>();
}

public class GoblinScoutSpawnerTileItem : SpawnerTileItem
{
    public override int TileType => TileType<GoblinScoutSpawnerTile>();
}

public class GoblinScoutSpawnerTileEntity : SpawnerTileEntity
{
    public override int NPCType => NPCID.GoblinScout;
    public override int TileType => TileType<GoblinScoutSpawnerTile>();
    public override int ActivationDistance => 300;
    public override int DespawnDistance => 600;

    public override bool IsDisabled()
    {
        return DisabledTimer > 0;
    }

    public override void OnNPCDeath()
    {
        DisableSpawner();
    }
}
