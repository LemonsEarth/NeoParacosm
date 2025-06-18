using Microsoft.Xna.Framework.Input;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Tiles.Depths;
using StructureHelper.API;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.GameContent.RGB;
using Terraria.IO;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems;

public class WorldDataSystem : ModSystem
{
    public static int ResearcherQuestProgress = 0;

    public override void ClearWorld()
    {
        ResearcherQuestProgress = 0;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        ResearcherQuestProgress = tag.GetInt("ResearcherQuestProgress");
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["ResearcherQuestProgress"] = ResearcherQuestProgress;
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(ResearcherQuestProgress);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadInt32();
    }
}
