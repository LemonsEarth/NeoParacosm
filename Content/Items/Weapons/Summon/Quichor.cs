using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using System.IO;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class Quichor : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    ref float Speed => ref Projectile.ai[2];

    Vector2 dashStartPos;
    Vector2 targetPos;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(dashStartPos);
        writer.WriteVector2(targetPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        dashStartPos = reader.ReadVector2();
        targetPos = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.25f;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
    }

    public override bool MinionContactDamage()
    {
        return true;
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override void AI()
    {
        Player owner = Projectile.GetOwner();
        if (IsPlayerAlive(owner) == false)
        {
            Projectile.Kill();
            return;
        }

        if (AITimer == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Speed = Main.rand.NextFloat(0.01f, 0.04f);
            }
            Projectile.netUpdate = true;
        }

        NPC currentTarget = null;
        if (owner.HasMinionAttackTargetNPC && Main.npc[owner.MinionAttackTargetNPC].Distance(owner.Center) < 1000)
        {
            currentTarget = Main.npc[owner.MinionAttackTargetNPC];
        }
        else
        {
            currentTarget = LemonUtils.GetClosestNPC(owner.Center, 1000);
        }

        if (currentTarget != null)
        {
            AttackBehavior(owner, currentTarget);
        }
        else
        {
            PassiveBehavior(owner);
            AttackTimer = 0;
        }
        Projectile.extraUpdates = 3;
        AITimer++;
    }

    void AttackBehavior(Player player, NPC npc)
    {
        if (AttackTimer == 0)
        {
            dashStartPos = Projectile.Center;
            if (Main.myPlayer == Projectile.owner)
            {
                targetPos = npc.RandomPos();
            }
            Projectile.netUpdate = true;
        }

        Vector2 startToTarget = dashStartPos.DirectionTo(targetPos);

        Dust.NewDustPerfect(
            Projectile.RandomPos(),
            DustID.GemTopaz,
            startToTarget * 5,
            newColor: Color.Gold,
            Scale: Main.rand.NextFloat(0.6f, 1f)).noGravity = true;

        Projectile.rotation = startToTarget.ToRotation();
        float targetHitboxDiagonal = new Vector2(npc.width, npc.height).Length();
        Vector2 realTargetPos = dashStartPos + startToTarget * (dashStartPos.Distance(targetPos) + targetHitboxDiagonal * 2);
        Projectile.Center = Vector2.Lerp(dashStartPos, realTargetPos, AttackTimer / 20f);
        AttackTimer++;
        if (AttackTimer > 20f)
        {
            AttackTimer = 0f;
        }
    }

    /// <summary>
    /// Once minion on each corner.
    /// Once all corners are filled, fill corners outward.
    /// </summary>
    /// <param name="player"></param>
    void PassiveBehavior(Player player)
    {
        Projectile.rotation = MathHelper.ToRadians(AITimer * 3);
        int group = (Projectile.minionPos / 4) + 1;
        float diagonalDistance = 64 * group;
        Vector2 pos = player.Center + Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * Projectile.minionPos) * diagonalDistance;
        Projectile.MoveToPos(pos, 0.1f, 0.1f, 0.1f, 0.1f);
    }

    bool IsPlayerAlive(Player owner)
    {
        if (owner.dead || !owner.active)
        {
            owner.ClearBuff(BuffType<QuichorBuff>());
            return false;
        }

        if (owner.HasBuff(BuffType<QuichorBuff>()))
        {
            Projectile.timeLeft = 2;
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(Color.White, 1f);
        Projectile.DrawProjectile(Color.White);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.Ichor, 90);
    }
}

public class QuichorBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ProjectileType<Quichor>()] > 0)
        {
            player.buffTime[buffIndex] = 18000;
        }
        else
        {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
