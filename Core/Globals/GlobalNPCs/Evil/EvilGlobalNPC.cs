using NeoParacosm.Content.NPCs.Hostile.Crimson;
using NeoParacosm.Content.NPCs.Hostile.Special;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using static Terraria.ID.NPCID;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Evil;

public class EvilGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public static HashSet<int> EvilEnemies { get; private set; } = new HashSet<int>()
        {
            FaceMonster, FloatyGross, BloodCrawler, BloodCrawlerWall, Crimera, Crimslime, CrimsonAxe, CrimsonBunny, CrimsonPenguin, CrimsonGoldfish,
            EaterofSouls, DevourerHead, Clinger, Slimer, IchorSticker,
            DarkMummy, BloodMummy, DesertGhoulCrimson, DesertGhoulCorruption, DesertLamiaDark, SandsharkCorrupt, SandsharkCrimson,
            ModContent.NPCType<CrimsonInfectionForm>(),
        };

    public static HashSet<int> EvilEnemiesBonus { get; private set; } = new HashSet<int>()
        {
            BigMimicCorruption, BigMimicCrimson, ModContent.NPCType<CrimsonCarrier>(), ModContent.NPCType<CrimsonSentryForm>(), ModContent.NPCType<CrimsonWalker>(), ModContent.NPCType<RotPerfumeValve>(), ModContent.NPCType<Marauder>()
        };

    int AITimer = 0;

    bool EvoActive => WorldDataSystem.ResearcherQuestProgress >= WorldDataSystem.ResearcherQuestProgressState.DownedEvilBoss
                   && WorldDataSystem.ResearcherQuestProgress < WorldDataSystem.ResearcherQuestProgressState.AscendedItem;

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

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (EvilEnemies.Contains(entity.type) || EvilEnemiesBonus.Contains(entity.type));
    }

    int maxDamageResistance = 50;
    int damageResistancePerHit = 5;
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (!EvoActive) return;
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
            if (Main.netMode != NetmodeID.MultiplayerClient)
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
