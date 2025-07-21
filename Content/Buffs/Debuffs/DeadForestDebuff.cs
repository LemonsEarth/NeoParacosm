using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Core.Systems;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class DeadForestDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {

    }

    public override void Update(Player player, ref int buffIndex)
    {
        
    }
}
