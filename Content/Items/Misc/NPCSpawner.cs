using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Content.Items.Misc;

public class NPCSpawner : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.value = Item.buyPrice(0, 0, 0, 0);
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.rare = ItemRarityID.Red;
    }

    Vector2 MouseTile => Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates();

    public override void HoldItem(Player player)
    {
        Dust.NewDustPerfect(
            MouseTile,
            DustID.GemDiamond,
            Vector2.Zero,
            Scale: 2f
            ).noGravity = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
        {
            return null;
        }
        Main.NewText("Placed NPC spawner");
        Vector2 mousePos = MouseTile;
        var ply = player.GetModPlayer<NPCSpawnerPlayer>();
        NPCSpawnerSystem system = GetInstance<NPCSpawnerSystem>();
        NPCSpawnPoint spawnPoint = new NPCSpawnPoint(ply.CurrentNPCType, mousePos, ply.AdditionalData);


        if (system.NPCSpawnPoints.ContainsKey(mousePos))
        {
            system.NPCSpawnPoints[mousePos] = spawnPoint;
        }
        else
        {
            system.NPCSpawnPoints.Add(mousePos, spawnPoint);
        }


        return true;
    }
}

public class NPCSpawnerPlayer : ModPlayer
{
    public int CurrentNPCType { get; set; } = -1;
    public float[] AdditionalData { get; set; } = new float[10];

    public bool ShowAllSpawnPoints { get; set; } = false;

    public void Reset()
    {
        CurrentNPCType = -1;
        AdditionalData = new float[10];
    }

    public override void PostUpdate()
    {
        if (ShowAllSpawnPoints)
        {
            NPCSpawnerSystem system = GetInstance<NPCSpawnerSystem>();
            foreach (var spawnPoint in system.NPCSpawnPoints.Keys)
            {
                if (spawnPoint.DistanceSQ(Player.Center) < 800 * 800)
                {
                    Vector2 pos = spawnPoint.ToTileCoordinates().ToWorldCoordinates();
                    Dust.NewDustPerfect(
                      pos,
                      DustID.GemDiamond,
                      Vector2.Zero,
                      Scale: 2f
                      ).noGravity = true;
                }
            }
        }
    }
}

public struct NPCSpawnPoint
{
    public int id = -1;
    public Vector2 position = Vector2.Zero;
    public float[] additionalData = new float[10];

    public NPCSpawnPoint(int _id, Vector2 _pos, params float[] args)
    {
        id = _id;
        position = _pos;
        for (int i = 0; i < args.Length; i++)
        {
            additionalData[i] = args[i];
        }
    }

    public NPCSpawnPoint(int _id, Vector2 _pos)
    {
        id = _id;
        position = _pos;
    }
}

public class NPCSpawnPointSerializer : TagSerializer<NPCSpawnPoint, TagCompound>
{
    public override NPCSpawnPoint Deserialize(TagCompound tag)
    {
        int id = tag.GetInt("id");
        Vector2 position = tag.Get<Vector2>("position");
        float[] additionalData = tag.Get<float[]>("additionalData");
        return new NPCSpawnPoint(id, position, additionalData);
    }

    public override TagCompound Serialize(NPCSpawnPoint value)
    {
        TagCompound tag = new TagCompound
        {
            ["id"] = value.id,
            ["position"] = value.position,
            ["additionalData"] = value.additionalData
        };

        return tag;
    }
}

public class NPCSpawnerSystem : ModSystem
{
    public Dictionary<Vector2, NPCSpawnPoint> NPCSpawnPoints = new Dictionary<Vector2, NPCSpawnPoint>();
    public override void ClearWorld()
    {
        NPCSpawnPoints.Clear();
    }

    public override void Load()
    {
        On_Main.DrawNPCs += On_Main_DrawNPCs;
    }

    private void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        orig(self, behindTiles);
        if (Main.LocalPlayer.GetModPlayer<NPCSpawnerPlayer>().ShowAllSpawnPoints)
        {
            foreach (var spawnPoint in NPCSpawnPoints)
            {
                int npcType = spawnPoint.Value.id;
                if (npcType < 0)
                {
                    continue;
                }
                Vector2 position = spawnPoint.Key;
                Rectangle frame = TextureAssets.Npc[npcType].Frame(1, Main.npcFrameCount[npcType], 0, 0);
                Main.EntitySpriteDraw(
                    TextureAssets.Npc[npcType].Value,
                    position - Main.screenPosition,
                    frame,
                    Color.White * 0.25f,
                    0f,
                    frame.Size() * 0.5f,
                    1f,
                    SpriteEffects.None
                    );
            }
        }
    }

    public static void ActivateNPCSpawners()
    {
        if (!LemonUtils.NotClient()) return;
        NPCSpawnerSystem system = GetInstance<NPCSpawnerSystem>();
        foreach (var npcSpawner in system.NPCSpawnPoints.Values)
        {
            if (npcSpawner.id < 0) continue;
            NPC.NewNPCDirect(
                new EntitySource_Misc($"NPC Spawner at {npcSpawner.position}"),
                npcSpawner.position,
                npcSpawner.id,
                0,
                ai0: npcSpawner.additionalData[0],
                ai1: npcSpawner.additionalData[1],
                ai2: npcSpawner.additionalData[2],
                ai3: npcSpawner.additionalData[4]
                );
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        var keys = tag.GetList<Vector2>("NPCSpawnPointsKeys");
        var values = tag.GetList<NPCSpawnPoint>("NPCSpawnPointsValues");
        for (int i = 0; i < keys.Count; i++)
        {
            NPCSpawnPoints[keys[i]] = values[i];
        }
    }

    public override void SaveWorldData(TagCompound tag)
    {
        foreach (var key in NPCSpawnPoints.Where(kvp => kvp.Value.id == -1)
                                          .Select(kvp => kvp.Key)
                                          .ToList())
        {
            NPCSpawnPoints.Remove(key);
        }
        tag["NPCSpawnPointsKeys"] = NPCSpawnPoints.Keys.ToList();
        tag["NPCSpawnPointsValues"] = NPCSpawnPoints.Values.ToList();
    }
}
