using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;

namespace NeoParacosm.Content.NPCs.Friendly.Special;

public class FriendlySkeleton : FriendlyPatrolNPC
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 26;
        NPC.height = 34;
        NPC.lifeMax = 500;
        NPC.defense = 3;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 0;
        NPC.knockBackResist = 0.6f;
        SetMovementValues(
            movementSpeed: 2f,
            chaseSpeed: 8f,
            acceleration: 0.05f,
            jumpHeight: 6f,
            fallThroughBottomMargin: 8f);
        SetPatrolValues(
            patrolDistance: 160,
            maxHomePositionDistance: 160,
            switchHomePositionInterval: 300,
            switchPatrolPositionInterval: 180);
        SetAggroValues(
            aggroRange: 400,
            maxTargetDistance: 800);
        SetInvestigatingValues(
            investigatingDuration: 300
            );
        SetAnimationValues(
            walkingStartFrame: 3,
            walkingMaxFrame: 4,
            walkingFrameDuration: 12,
            jumpingFrame: 1,
            fallingFrame: 2,
            idleFrame: 0);
        SetCombatValues(
            knockback: 5f,
            npcHitCooldown: 60,
            bossHitCooldown: 60);
    }

    public override void AI()
    {
        base.AI();
    }

    public override void Despawn()
    {
        base.Despawn();
        LemonUtils.DustBurst(10, NPC.Center, DustType<FireDust>(), 2, 2, 0.6f, 1f, Color.Black);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(10, NPC.Center, DustType<FireDust>(), 2, 2, 0.6f, 1f, Color.Black);
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
        }
    }
}
