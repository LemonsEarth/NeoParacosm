namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class FireAuraBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        
    }
}
