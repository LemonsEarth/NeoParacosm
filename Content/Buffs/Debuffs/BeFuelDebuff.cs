using NeoParacosm.Content.Items.Weapons.Magic.Spells;
namespace NeoParacosm.Content.Buffs.Debuffs;

public class BeFuelDebuff : ModBuff
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
        player.GetDamage(DamageClass.Generic) += 17f / 100f;
        player.AddElementalDamageBoost(BaseSpell.SpellElement.Fire, 10f/100f);
        player.AddElementalExpertiseBoost(BaseSpell.SpellElement.Fire, 10f/100f);
    }
}
