global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
global using System;
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
        FaceMonster, FloatyGross, BloodCrawler, BloodCrawlerWall, Crimera, Crimslime, CrimsonAxe, CrimsonBunny, CrimsonPenguin, CrimsonGoldfish, BigMimicCrimson,
        EaterofSouls, DevourerHead, Clinger, Slimer, BigMimicCorruption, 
        DarkMummy, BloodMummy, DesertGhoulCrimson, DesertGhoulCorruption, DesertLamiaDark, SandsharkCorrupt, SandsharkCrimson
    };
}
