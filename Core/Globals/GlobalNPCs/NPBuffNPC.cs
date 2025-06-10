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

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(ModContent.BuffType<CrimsonRotDebuff>()))
        {
            DOTDebuff(npc, 24, ref damage);
        }
    }

    void DOTDebuff(NPC npc, int damagePerSecond, ref int damage)
    {
        if (npc.lifeRegen > 0) npc.lifeRegen = 0;
        npc.lifeRegen -= damagePerSecond * 2;
        if (damage < damagePerSecond)
        {
            damage = damagePerSecond;
        }
    }
}
