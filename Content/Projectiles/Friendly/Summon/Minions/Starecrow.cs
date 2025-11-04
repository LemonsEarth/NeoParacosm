using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Content.Projectiles.Hostile;
using Terraria.Utilities.Terraria.Utilities;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Minions;

public class Starecrow : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    float attackTimer = 0;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 72;
        Projectile.height = 72;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 1f;
        Projectile.penetrate = -1;
        Projectile.frameCounter = 9;
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

    NPC closestNPC;
    Vector2 randomPos = Vector2.Zero;
    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (IsPlayerAlive(owner) == false)
        {
            return;
        }
        Projectile.frameCounter++;
        if (Projectile.frameCounter == 20)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 0;
            }
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
        if (currentTarget == null)
        {
            if (AITimer % 300 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    randomPos = Main.rand.NextVector2Circular(300, 300);
                }
            }
            Vector2 tPos = owner.Center;
            if (randomPos != Vector2.Zero)
            {
                tPos = owner.Center + randomPos;
            }
            Projectile.velocity = Projectile.Center.DirectionTo(tPos) * (Projectile.Center.Distance(tPos) / 24f);
            Projectile.spriteDirection = MathF.Sign(Projectile.DirectionTo(owner.Center).X) != 0 ? -MathF.Sign(Projectile.DirectionTo(owner.Center).X) : 1;
            float bonusRot = Projectile.spriteDirection == 1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.Center.DirectionTo(owner.Center).ToRotation() + bonusRot;
        }
        else
        {
            if (AITimer % 60 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY * 5, ProjectileType<LingeringDeathflameFriendly>(), ai0: 0, ai1: 90, ai2: 1.2f);
                }
            }
            if (AITimer % 90 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity += Projectile.DirectionTo(currentTarget.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 10;
                }
                Projectile.netUpdate = true;
            }
            if (AITimer % 300 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity += -Projectile.DirectionTo(currentTarget.Center) * 15;
                }
                Projectile.netUpdate = true;
            }
            Projectile.MoveToPos(currentTarget.Center, 0.1f, 0.1f, 0.3f, 0.3f);
            Projectile.spriteDirection = MathF.Sign(Projectile.DirectionTo(currentTarget.Center).X) != 0 ? -MathF.Sign(Projectile.DirectionTo(currentTarget.Center).X) : 1;
            float bonusRot = Projectile.spriteDirection == 1 ? MathHelper.Pi : 0;
            Projectile.rotation = Projectile.Center.DirectionTo(currentTarget.Center).ToRotation() + bonusRot;
        }

        AITimer++;
    }

    bool IsPlayerAlive(Player owner)
    {
        if (owner.dead || !owner.active)
        {
            owner.ClearBuff(BuffType<StarecrowBuff>());
            return false;
        }

        if (owner.HasBuff(BuffType<StarecrowBuff>()))
        {
            Projectile.timeLeft = 2;
        }
        return true;
    }
}

public class StarecrowBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ProjectileType<Starecrow>()] > 0)
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
