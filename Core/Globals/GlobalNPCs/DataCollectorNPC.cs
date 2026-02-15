using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Accessories.Misc;
using NeoParacosm.Content.Items.Placeable.Special.ResearcherQuestTiles;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class DataCollectorNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    int timer = 0;

    public Point16 dataCollectorTEPos { get; set; } = Point16.Zero;
    public Point16 dataCollectorEXTEPos { get; set; } = Point16.Zero;

    public override void OnKill(NPC npc)
    {
        if (npc.SpawnedFromStatue) return;

        if (dataCollectorTEPos != Point16.Zero && TileEntity.TryGet<DataCollectorTileEntity>(dataCollectorTEPos, out DataCollectorTileEntity dataCollector))
        {
            if (EvilGlobalNPC.EvilEnemiesBonus.Contains(npc.type))
            {
                dataCollector.CollectData(3);
            }
            else if (EvilGlobalNPC.EvilEnemies.Contains(npc.type))
            {
                dataCollector.CollectData();
            }
        }

        if (dataCollectorEXTEPos != Point16.Zero && TileEntity.TryGet<DataCollectorEXTileEntity>(dataCollectorEXTEPos, out DataCollectorEXTileEntity dataCollectorEX))
        {
            if (EvilGlobalNPC.EvilEnemiesBonus.Contains(npc.type))
            {
                dataCollectorEX.CollectData(3);
            }
            else if (EvilGlobalNPC.EvilEnemies.Contains(npc.type))
            {
                dataCollectorEX.CollectData();
            }
        }
    }
}
