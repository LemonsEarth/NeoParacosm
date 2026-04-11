using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class BodyOfThornsBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class BodyOfThornsPlayer : ModPlayer
{
    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (Player.HasBuff(BuffType<BodyOfThornsBuff>()))
        {
            int damageCapped = (int)MathF.Min(hurtInfo.Damage * Player.GetElementalDamageBoost(SpellElement.Nature), npc.lifeMax * 0.05f);
            npc.SimpleStrikeNPC(damageCapped, -hurtInfo.HitDirection);
        }
    }
}
