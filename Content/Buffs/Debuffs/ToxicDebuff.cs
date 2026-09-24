using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class ToxicDebuff : ModBuff
{
    public static int SecondsToDetonateNPC = 60;
    public static int SecondsToDetonatePlayer = 60;
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
    }

    public static void AddToNPC(NPC npc, int duration)
    {
        int index = npc.FindBuffIndex(BuffType<ToxicDebuff>());
        if (index >= 0)
        {
            int buffTime = npc.buffTime[index];
            npc.AddBuff(BuffType<ToxicDebuff>(), buffTime + duration);
        }
        else
        {
            npc.AddBuff(BuffType<ToxicDebuff>(), duration);
        }
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        //Main.NewText(npc.buffTime[buffIndex]);
        int dustCD = 5 - (int)(npc.buffTime[buffIndex] / 1000f);
        if (NPPlayer.Timer % dustCD == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                    ParticleID.Glowy,
                    npc.RandomPos(4, 4),
                    Main.rand.NextVector2Circular(1, 1),
                    Main.rand.NextFromList(new Color(0, 30, 0, 255) * 1f),
                    0f,
                    scale: 1f,
                    data0: 60,
                    data1: 10,
                    data2: 10,
                    data3: 0.98f);
            }
        }

        if (npc.buffTime[buffIndex] >= SecondsToDetonateNPC * 60)
        {
            npc.SimpleStrikeNPC(200, 1);
            for (int i = 0; i < 20; i++)
            {
                ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                    ParticleID.Glowy,
                    npc.RandomPos(4, 4),
                    Main.rand.NextVector2Circular(3, 3),
                    Main.rand.NextFromList(new Color(0, 30, 0, 255) * 1f),
                    0f,
                    scale: 3f,
                    data0: 60,
                    data1: 10,
                    data2: 10,
                    data3: 0.98f);
            }
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }

    public override void Update(Player player, ref int buffIndex)
    {
        for (int i = 0; i < 1; i++)
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                ParticleID.Glowy,
                player.RandomPos(16, 16),
                -Vector2.UnitY * Main.rand.NextFloat(1, 3),
                Main.rand.NextFromList(new Color(0, 30, 0, 255) * 1f),
                0f,
                scale: 1f,
                data0: 60,
                data1: 10,
                data2: 10,
                data3: 0.95f);
        }

        if (player.buffTime[buffIndex] >= SecondsToDetonatePlayer * 60)
        {
            PlayerDeathReason pdr = PlayerDeathReason.ByCustomReason(this.GetLocalization("DeathReason").ToNetworkText(player.name));
            player.ClearBuff(BuffType<ToxicDebuff>());
            int damage = 75 * LemonUtils.GetDifficulty();
            player.Hurt(pdr, damage, 1, dodgeable: false, armorPenetration: 9999);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(player.Center, DustType<CircleDust>(), new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10)), Scale: Main.rand.NextFloat(0.8f, 1.2f), newColor: new Color(0f, Main.rand.NextFloat(0.2f, 0.6f), 0f, 1f)).noGravity = true;
            }
        }
    }
}

public class ToxicDebuffNPC : GlobalNPC
{
    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (npc.HasBuff(BuffType<ToxicDebuff>()))
        {
            npc.DOTDebuff(8, ref damage);
        }
    }
}

public class ToxicDebuffPlayer : ModPlayer
{
    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff(BuffType<ToxicDebuff>()))
        {
            Player.DOTDebuff(3);
        }
    }

    public override void PostUpdateRunSpeeds()
    {

    }
}
