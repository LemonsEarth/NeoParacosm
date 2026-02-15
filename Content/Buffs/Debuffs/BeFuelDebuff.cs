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
        player.AddElementalDamageBoost(BaseSpell.SpellElement.Fire, 10f / 100f);
        player.AddElementalExpertiseBoost(BaseSpell.SpellElement.Fire, 10f / 100f);
    }
}

public class BeFuelPlayer : ModPlayer
{
    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<BeFuelDebuff>()))
        {
            Dust.NewDustDirect(Player.RandomPos(8, 8), 2, 2, DustID.OrangeStainedGlass, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-4, -0.5f)).noGravity = true;
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<BeFuelDebuff>()))
        {
            Player.DOTDebuff(15);
        }
    }
}
