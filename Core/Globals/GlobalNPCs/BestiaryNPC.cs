using NeoParacosm.Content.Items.Placeable.Special.ResearcherQuestTiles;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class BestiaryNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.ModNPC.Mod == Mod;
    }

    public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        if (Language.Exists($"Mods.NeoParacosm.NPCs.{npc.TypeName}.Bestiary"))
        {
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement($"Mods.NeoParacosm.NPCs.{npc.TypeName}.Bestiary"));
        }
    }
}
