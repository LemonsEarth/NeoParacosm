using NeoParacosm.Content.NPCs.Hostile.Crimson;
using System.Collections.Generic;
using static Terraria.ID.NPCID;
using static Terraria.ModLoader.ModContent;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static class NPCLists
{
    public static HashSet<int> EvilEnemies { get; private set; } = new HashSet<int>()
    {
        FaceMonster, FloatyGross, BloodCrawler, BloodCrawlerWall, Crimera, Crimslime, CrimsonAxe, CrimsonBunny, CrimsonPenguin, CrimsonGoldfish,
        EaterofSouls, DevourerHead, Clinger, Slimer, IchorSticker,
        DarkMummy, BloodMummy, DesertGhoulCrimson, DesertGhoulCorruption, DesertLamiaDark, SandsharkCorrupt, SandsharkCrimson,
         NPCType<CrimsonInfectionForm>(),
    };

    public static HashSet<int> EvilEnemiesBonus { get; private set; } = new HashSet<int>()
    {
        BigMimicCorruption, BigMimicCrimson, NPCType<CrimsonCarrier>(), NPCType<CrimsonSentryForm>(), NPCType<CrimsonWalker>(), NPCType<RotPerfumeValve>()
    };
}
