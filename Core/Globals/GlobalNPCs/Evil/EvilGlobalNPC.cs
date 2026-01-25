using NeoParacosm.Content.NPCs.Hostile.Corruption;
using NeoParacosm.Content.NPCs.Hostile.Crimson;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using static Terraria.ID.NPCID;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Evil;

public class EvilGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public static HashSet<int> EvilEnemies { get; private set; } = new HashSet<int>();
    public static HashSet<int> EvilEnemiesBonus { get; private set; } = new HashSet<int>();

    int AITimer = 0;

    static bool EvoActive => ResearcherQuest.Progress >= ResearcherQuest.ProgressState.DownedEvilBoss
                   && ResearcherQuest.Progress < ResearcherQuest.ProgressState.AscendedItem;

    Dictionary<DamageClass, int> ClassAdaptation = new Dictionary<DamageClass, int>
        {
            {DamageClass.Melee, 0},
            {DamageClass.Ranged, 0},
            {DamageClass.Magic, 0},
            {DamageClass.Summon, 0},
            {DamageClass.Generic, 0},
        };

    public override void SetStaticDefaults()
    {
        EvilEnemies = new HashSet<int>()
        {
            FaceMonster, FloatyGross, BloodCrawler, BloodCrawlerWall, Crimera, Crimslime, CrimsonAxe, CrimsonBunny, CrimsonPenguin, CrimsonGoldfish,
            EaterofSouls, DevourerHead, Clinger, Slimer, IchorSticker, Corruptor, CorruptBunny, CorruptPenguin, CorruptGoldfish, CursedHammer, Slimer2,
            DarkMummy, BloodMummy, DesertGhoulCrimson, DesertGhoulCorruption, DesertLamiaDark, SandsharkCorrupt, SandsharkCrimson,
            NPCType<CrimsonInfectionForm>(), NPCType<BaneflyEnemy>(), NPCType<DecayingRat>()
        };

        EvilEnemiesBonus = new HashSet<int>()
        {
            BigMimicCorruption, BigMimicCrimson, NPCType<CrimsonCarrier>(), NPCType<CrimsonSentryForm>(),
            NPCType<CrimsonWalker>(), NPCType<RotPerfumeValve>(), NPCType<Marauder>(), NPCType<CorruptMage>(), NPCType<CorruptWalker>(), NPCType<BaneflyHiveEnemy>()
        };
    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (EvilEnemies.Contains(entity.type) || EvilEnemiesBonus.Contains(entity.type));
    }

    int maxDamageResistance = 50;
    int damageResistancePerHit = 5;
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (!EvoActive || modifiers.DamageType == null) return;
        if (ClassAdaptation.ContainsKey(modifiers.DamageType))
        {
            if (ClassAdaptation[modifiers.DamageType] < maxDamageResistance)
            {
                ClassAdaptation[modifiers.DamageType] += damageResistancePerHit;
            }
            modifiers.FinalDamage *= (100 - ClassAdaptation[modifiers.DamageType]) / 100f;

            foreach (var key in ClassAdaptation.Keys)
            {
                if (key != modifiers.DamageType)
                {
                    if (ClassAdaptation[key] >= damageResistancePerHit) ClassAdaptation[key] -= damageResistancePerHit;
                }
            }
        }
        else
        {
            if (ClassAdaptation[DamageClass.Generic] < maxDamageResistance)
            {
                ClassAdaptation[DamageClass.Generic] += damageResistancePerHit;
            }
            modifiers.FinalDamage *= (100 - ClassAdaptation[DamageClass.Generic]) / 100f;
        }
    }

    public override void PostAI(NPC npc)
    {
        //foreach(KeyValuePair<DamageClass, int> kvp in ClassAdaptation)
        //{
        //    Main.NewText(kvp.Key + ": " + kvp.Value);
        //}
        if (EvoActive && AITimer % 180 == 0 && npc.life != npc.lifeMax)
        {
            if (LemonUtils.NotClient())
            {
                int healValue = npc.lifeMax / 10;
                if (npc.life + healValue > npc.lifeMax)
                {
                    healValue = npc.lifeMax - npc.life;
                }
                if (healValue > 150) healValue = 150;
                npc.life += healValue;
                npc.HealEffect(healValue);
            }
            npc.netUpdate = true;

            foreach (DamageClass key in ClassAdaptation.Keys)
            {
                if (ClassAdaptation[key] >= damageResistancePerHit)
                {
                    ClassAdaptation[key] -= damageResistancePerHit;
                }
            }
        }

        AITimer++;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {

    }

    public override void OnKill(NPC npc)
    {

    }
}
