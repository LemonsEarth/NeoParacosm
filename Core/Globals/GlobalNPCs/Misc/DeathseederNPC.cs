using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using static Terraria.ID.NPCID;

namespace NeoParacosm.Core.Globals.GlobalNPCs.Evil;

public class DeathseederNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    int AITimer = 0;
    bool isDeathseederSpawn = false;

    public static HashSet<int> PossibleNPCs { get; private set; } = new HashSet<int>()
    {
        Skeleton
    };



    public override void SetStaticDefaults()
    {

    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        binaryWriter.Write(npc.damage);
        binaryWriter.Write(npc.lifeMax);
        binaryWriter.Write(npc.life);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        npc.damage = binaryReader.ReadInt32();
        npc.lifeMax = binaryReader.ReadInt32();
        npc.life = binaryReader.ReadInt32();
    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (PossibleNPCs.Contains(entity.type));
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (source.Context == "DeathseederSpawn")
        {
            isDeathseederSpawn = true;
        }
    }

    public override void PostAI(NPC npc)
    {
        if (!isDeathseederSpawn)
        {
            return;
        }

        if (AITimer == 0)
        {
            npc.life = npc.lifeMax;
        }

        npc.friendly = true;
        npc.AddBuff(BuffID.Confused, 999);


        if (AITimer % 1000 == 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(npc, npc.position, Vector2.Zero, ProjectileType<InvisibleProjectileFriendly>(), npc.damage, (1 - npc.knockBackResist) * 6, -1, npc.whoAmI, npc.width, npc.height);
            }
        }

        if (AITimer >= 1200)
        {
            npc.active = false;
        }
        AITimer++;
    }

    public override bool? CanFallThroughPlatforms(NPC npc)
    {
        if (isDeathseederSpawn) return false;
        return null;
    }

    public override bool CheckDead(NPC npc)
    {
        if (isDeathseederSpawn)
        {
            SoundEngine.PlaySound(npc.DeathSound, npc.Center);
            npc.HitEffect();
            npc.active = false;
            return false;
        }
        return true;
    }

    public override void OnKill(NPC npc)
    {

    }
}
