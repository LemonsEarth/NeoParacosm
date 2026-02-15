namespace NeoParacosm.Content.Buffs.Debuffs;

public class LightsBaneDebuff : ModBuff
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

public class LightsBaneDebuff2 : ModBuff
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

public class LightsBaneNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    int timer = 0;
    public override void PostAI(NPC npc)
    {
        if (npc.HasBuff(BuffType<LightsBaneDebuff>()))
        {
            if (timer % 5 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.RandomPos(16, 16), Main.rand.NextVector2Unit(), ProjectileID.LightsBane, 30, 0f, Main.myPlayer, Main.rand.NextFloat(0.75f, 1.25f));
                }
            }
            timer++;
        }
        else if (npc.HasBuff(BuffType<LightsBaneDebuff2>()))
        {
            int attackInterval = 30;
            if (timer >= 45) attackInterval = 10;
            if (timer >= 75) attackInterval = 3;
            if (timer % attackInterval == 0)
            {
                if (LemonUtils.NotClient())
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.RandomPos(16, 16), Main.rand.NextVector2Unit(), ProjectileID.LightsBane, 60, 0f, Main.myPlayer, Main.rand.NextFloat(0.75f, 1.25f));
                }
            }
            timer++;
        }
        else
        {
            timer = 0;
        }
    }
}
