using Terraria.DataStructures;

namespace NeoParacosm.Content.NPCs.Friendly.Special;

public class FriendlyGoblinMinion : FriendlyPatrolNPC
{
    ref float PlayerOwner => ref NPC.ai[2];

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        NPC.damage = (int)Main.player[(int)PlayerOwner].GetTotalDamage(DamageClass.Summon).ApplyTo(NPC.damage);
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 26;
        NPC.height = 34;
        NPC.lifeMax = 100;
        NPC.defense = 3;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 0;
        NPC.knockBackResist = 0.6f;
        SetMovementValues(
            movementSpeed: 6f,
            chaseSpeed: 15f,
            acceleration: 0.06f,
            jumpHeight: 10f,
            fallThroughBottomMargin: 8f);
        SetPatrolValues(
            patrolDistance: 300,
            maxHomePositionDistance: 400,
            switchHomePositionInterval: 180,
            switchPatrolPositionInterval: 120);
        SetAggroValues(
            aggroRange: 400,
            maxTargetDistance: 600);
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
            knockback: 7f,
            npcHitCooldown: 40,
            bossHitCooldown: 40);
    }

    public override void AI()
    {
        base.AI();
    }

    public override void Despawn()
    {
        base.Despawn();
        LemonUtils.DustBurst(10, NPC.Center, DustID.Grass, 2, 2, 0.6f, 1f);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(10, NPC.Center, DustID.Grass, 2, 2, 0.6f, 1f);
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Grass);
        }
    }
}
