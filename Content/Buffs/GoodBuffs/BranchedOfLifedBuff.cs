namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class BranchedOfLifedBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        //player.GetModPlayer<ParacosmPlayer>().branchedOfLifed = true;
    }
}

public class BranchedOfLifedPlayer : ModPlayer
{
    int bofBuffTimer = 0;

    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<BranchedOfLifedBuff>()))
        {
            Player.GetDamage(DamageClass.Generic) += 12f / 100f;
            Player.GetCritChance(DamageClass.Generic) += 8f / 100f;
            Player.manaCost *= 0.8f;
            Player.lifeRegen += 2;

            if (bofBuffTimer <= 0)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    int buffID = Main.rand.Next(0, 5) switch
                    {
                        0 => BuffID.WellFed3,
                        1 => BuffID.Endurance,
                        2 => BuffID.Lifeforce,
                        3 => BuffID.Wrath,
                        4 => BuffID.Rage,
                        _ => BuffID.Ironskin
                    };
                    Player.AddBuff(buffID, 1200);
                }
                bofBuffTimer = 300;

            }
        }
        if (bofBuffTimer > 0)
        {
            bofBuffTimer--;
        }
    }
}
