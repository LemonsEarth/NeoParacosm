using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class BestiaryNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.ModNPC != null && entity.ModNPC.Mod == Mod;
    }

    public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        if (Language.Exists($"Mods.NeoParacosm.NPCs.{npc.ModNPC.Name}.Bestiary"))
        {
            bestiaryEntry.Info.Add(new FlavorTextBestiaryInfoElement($"Mods.NeoParacosm.NPCs.{npc.ModNPC.Name}.Bestiary"));
        }
    }
}
