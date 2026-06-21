using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using System.IO;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class SmallIchorSpirit : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    ref float Speed => ref Projectile.ai[2];

    Vector2 targetPos;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(targetPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        targetPos = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 40;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
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

        Dust.NewDustPerfect(
            Projectile.RandomPos(),
            DustType<FireDust>(),
            -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f),
            Alpha: 150,
            newColor: Color.Gold,
            Scale: Main.rand.NextFloat(0.3f, 0.6f));

        Projectile.StandardAnimation(6, 4);

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
            AttackTimer++;
        }
        else
        {
            PassiveBehavior(owner);
            AttackTimer = 0;
        }

        AITimer++;
    }

    void AttackBehavior(Player player, NPC npc)
    {
        if (AttackTimer == 240)
        {
            Projectile.velocity = Projectile.DirectionTo(npc.Center) * 20;
        }
        if (AttackTimer > 240)
        {
            Projectile.MoveToPos(npc.Center, 0.1f, 0.1f, Speed * 10f, Speed * 10f);
            if (Main.myPlayer == Projectile.owner && AttackTimer % 5 == 0)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Vector2.Zero,
                    ProjectileType<IchorFlamethrowerFriendly>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai0: 20, ai1: 0.95f, ai2: 0f);
            }

            if (AttackTimer > 360)
            {
                AttackTimer = 0;
            }
        }
        else
        {
            Projectile.MoveToPos(npc.Center, 0.1f, 0.1f, Speed, Speed);
        }

    }

    /// <summary>
    /// Once minion on each corner.
    /// Once all corners are filled, fill corners outward.
    /// </summary>
    /// <param name="player"></param>
    void PassiveBehavior(Player player)
    {
        int group = (Projectile.minionPos / 4) + 1;
        float diagonalDistance = 64 * group;
        Vector2 pos = player.Center + Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * Projectile.minionPos) * diagonalDistance;
        Projectile.MoveToPos(pos, 0.3f, 0.3f, 0.2f, 0.2f);
    }

    bool IsPlayerAlive(Player owner)
    {
        if (owner.dead || !owner.active)
        {
            owner.ClearBuff(BuffType<SmallIchorSpiritBuff>());
            return false;
        }

        if (owner.HasBuff(BuffType<SmallIchorSpiritBuff>()))
        {
            Projectile.timeLeft = 2;
        }
        return true;
    }
}

public class SmallIchorSpiritBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ProjectileType<SmallIchorSpirit>()] > 0)
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
