namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class GoldenHeartBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.statDefense += 5;
        player.endurance += 5f / 100f;
        player.GetDamage(DamageClass.Generic) += 10f / 100f;
        player.GetCritChance(DamageClass.Generic) += 5f;
        for (int i = 0; i < player.buffTime.Length; i++)
        {
            if (player.buffType[i] != Type)
            {
                if (player.buffTime[i] > 12 && !Main.debuff[player.buffType[i]])
                {
                    player.buffTime[i] -= 10;
                }
            }
            else
            {

            }
        }
    }
}
