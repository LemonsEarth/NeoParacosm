using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Core.Globals.GlobalNPCs;
using NeoParacosm.Core.Systems.Data;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Placeable.Special.Spawners;

public abstract class SpawnerTileEntity : ModTileEntity
{
    public abstract int NPCType { get; }
    public abstract int TileType { get; }
    public abstract int ActivationDistance { get; }
    public abstract int DespawnDistance { get; }
    public abstract bool IsDisabled();
    public abstract void OnNPCDeath();

    public virtual bool NeedsLineOfSight => false;

    public bool NPCWasForceDespawned { get; private set; } = false;
    public NPC SpawnedNPC { get; private set; }

    public int DisabledTimer { get; set; } = 0;
    public int DisabledDuration { get; set; } = 600;

    public void DisableSpawner()
    {
        DisabledTimer = DisabledDuration;
    }

    /// <summary>
    /// World position of the tile center
    /// </summary>
    public Vector2 Center => (Position + new Point16(1, 1)).ToWorldCoordinates();

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == TileType;
    }

    public void SpawnNPC(int targetPlayer)
    {
        if (LemonUtils.NotClient())
        {
            SpawnedNPC = NPC.NewNPCDirect(
                new EntitySource_TileEntity(this, "NeoParacosm:SpawnerNPC"),
                Center,
                NPCType,
                target: targetPlayer
                );
        }
        NPCWasForceDespawned = false;
    }

    /// <summary>
    /// Checks if there is any player close to the tile (ActivationDistance). If there is, spawns the NPC.
    /// </summary>
    public void TrySpawningNPC()
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (!player.IsAlive()) continue;
            float distanceSQToCenter = player.Center.DistanceSQ(Center);
            float activationDistanceSQ = ActivationDistance * ActivationDistance;

            if (distanceSQToCenter < activationDistanceSQ)
            {
                bool hasLineOfSight = true;
                if (NeedsLineOfSight)
                {
                    if (!Collision.CanHit(Position.ToWorldCoordinates(), 32, 32, player.position, player.width, player.height))
                    {
                        hasLineOfSight = false;
                    }
                }

                if (hasLineOfSight)
                {
                    SpawnNPC(player.whoAmI);
                    break;
                }
            }
        }
    }

    void ForceDespawnNPC()
    {
        SpawnedNPC.active = false;
        NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC.whoAmI);
        SpawnedNPC = null;
        NPCWasForceDespawned = true;
    }

    /// <summary>
    /// Checks if there is any player close to the tile (DespawnDistance). If there isn't, despawns the NPC.
    /// </summary>
    public void TryDespawningNPC()
    {
        bool somePlayerIsClose = false;
        foreach (var player in Main.ActivePlayers)
        {
            if (!player.IsAlive()) continue;
            if (player.DistanceSQ(Center) < DespawnDistance * DespawnDistance)
            {
                somePlayerIsClose = true;
                break;
            }
        }

        if (!somePlayerIsClose)
        {
            ForceDespawnNPC();
        }
    }

    public bool NPCWasKilled()
    {
        return (!SpawnedNPC.active || !SpawnedNPC.IsAlive()) && !NPCWasForceDespawned;
    }

    public override void Update()
    {
        if (IsDisabled())
        {
            if (DisabledTimer > 0)
            {
                if (DisabledTimer % 60 == 0)
                {
                    AdvancedPopupRequest req = new AdvancedPopupRequest
                    {
                        Text = (DisabledTimer / 60).ToString(),
                        Color = Color.White,
                        DurationInFrames = 30,
                        Velocity = Vector2.Zero
                    };
                    PopupText.NewText(req, Position.ToWorldCoordinates() + new Vector2(8, -32));
                }
                DisabledTimer--;
            }
            return;
        }

        Rectangle rect = new Rectangle(Position.X * 16, Position.Y * 16, 32, 32);
        Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(rect), DustID.GemTopaz, -Vector2.UnitY).noGravity = true;

        if (SpawnedNPC != null && NPCWasKilled())
        {
            OnNPCDeath();
            SpawnedNPC = null;
            return;
        }

        if (SpawnedNPC == null)
        {
            TrySpawningNPC();
        }
        else
        {
            TryDespawningNPC();
        }

        //LemonUtils.DustRing(Center, ActivationDistance, 32, DustID.GemTopaz, 3);
        //LemonUtils.DustRing(Center, DespawnDistance, 32, DustID.GemRuby, 6);
    }
}
