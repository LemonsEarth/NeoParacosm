namespace NeoParacosm.Content.Buffs.Debuffs;

public class SnowgraveDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {

    }

    public override void Update(Player player, ref int buffIndex)
    {

    }
}
