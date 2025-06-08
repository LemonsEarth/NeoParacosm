using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;

namespace NeoParacosm.Core.Globals.GlobalNPCs;

public class NPBuffNPC : GlobalNPC
{
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (npc.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            modifiers.Defense.Flat -= (10 - (npc.life / npc.lifeMax * 10));
        }
    }
}
