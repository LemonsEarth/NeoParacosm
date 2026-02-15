namespace NeoParacosm.Content.Buffs.Debuffs;

public class ShroomedDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        Dust.NewDustDirect(npc.RandomPos(), 2, 2, DustID.GlowingMushroom);
    }
}

public class ShroomedNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override void PostAI(NPC npc)
    {
        if (npc.HasBuff(BuffType<ShroomedDebuff>()))
        {
            foreach (var otherNPC in Main.ActiveNPCs)
            {
                if (npc.whoAmI != otherNPC.whoAmI && npc.DistanceSQ(otherNPC.Center) < 10000)
                {
                    otherNPC.AddBuff(BuffType<ShroomedDebuff>(), 180);
                }
            }

            if (LemonUtils.NotClient() && Main.rand.NextBool(200))
            {
                Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.RandomPos(), Vector2.Zero, ProjectileID.Mushroom, 5, 0f);
            }
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(BuffType<ShroomedDebuff>()))
        {
            float dps = 8;
            if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.Creeper || npc.realLife != -1)
            {
                dps = 1;
            }
            npc.DOTDebuff(dps, ref damage);
        }
    }
}
