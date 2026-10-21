using NeoParacosm.Core.Systems.Particles;
using NeoParacosm.Core.Systems.Particles.Renderers;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class EnemySlowed : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
}

public class EnemySlowedNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.boss) return true;
        if (npc.HasBuff(BuffType<EnemySlowed>()))
        {
            npc.velocity *= 2f;
        }
        return true;
    }

    public override void PostAI(NPC npc)
    {
        if (npc.boss) return;
        if (npc.HasBuff(BuffType<EnemySlowed>()))
        {
            ParticleSystem.SpawnParticle<AfterDustParticleRendererGlowy>(
                    ParticleID.Glowy,
                    npc.RandomPos(8, 8),
                    Vector2.UnitY.RotatedBy(Main.rand.NextRotation()),
                    Main.rand.NextFromList(Color.LightBlue, Color.Blue, Color.Cyan) * 0.5f,
                    1f,
                    Main.rand.NextFloat(1f, 1.2f),
                    data0: 60,
                    data1: 5f,
                    data2: 30f,
                    data3: Main.rand.NextFloat(0.95f, 0.98f)
                    );

            npc.velocity *= 0.5f;
        }
    }
}
