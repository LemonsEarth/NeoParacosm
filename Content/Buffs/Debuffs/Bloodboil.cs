using NeoParacosm.Content.Items.Weapons.Magic.Spells;
namespace NeoParacosm.Content.Buffs.Debuffs;

public class Bloodboil : ModBuff
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

public class BloodboilPlayer : ModPlayer
{
    int timer = 0;
    public override void PostUpdateBuffs()
    {
        if (Player.HasBuff<Bloodboil>())
        {
            if (timer < 300)
            {
                timer++;
            }

            float value = MathHelper.Lerp(1, 0.5f, timer / 300f);
            Player.statLifeMax2 = (int)(Player.statLifeMax2 * value);
        }
        else
        {
            timer = 0;
        }
    }
}
