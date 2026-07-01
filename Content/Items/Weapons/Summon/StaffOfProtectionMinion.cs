namespace NeoParacosm.Content.Items.Weapons.Summon;

public class StaffOfProtectionMinion : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    ref float Speed => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.25f;
    }

    public override void SetDefaults()
    {
        Projectile.width = 42;
        Projectile.height = 42;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 3;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
        Projectile.extraUpdates = 0;
    }

    public override bool MinionContactDamage()
    {
        return false;
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
        Projectile.rotation = MathHelper.ToRadians(AITimer) - MathHelper.PiOver4;
        Vector2 pos = owner.Center - Vector2.UnitY.RotatedBy(MathHelper.ToRadians(AITimer)) * 64;
        Projectile.Center = pos;
        AITimer++;
    }

    bool IsPlayerAlive(Player owner)
    {
        if (owner.dead || !owner.active)
        {
            owner.ClearBuff(BuffType<StaffOfProtectionBuff>());
            return false;
        }

        if (owner.HasBuff(BuffType<StaffOfProtectionBuff>()))
        {
            Projectile.timeLeft = 2;
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity * 0.5f, Projectile.scale * 2);
        Projectile.DrawAfterimages(Color.White, 0.5f);
        Projectile.DrawProjectile(Color.White);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}

public class StaffOfProtectionBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (player.ownedProjectileCounts[ProjectileType<StaffOfProtectionMinion>()] > 0)
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

public class StaffOfProtectionBuffPlayer : ModPlayer
{
    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<StaffOfProtectionBuff>()))
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner == Player.whoAmI && projectile.type == ProjectileType<StaffOfProtectionMinion>())
                {
                    if (projectile.Distance(proj.Center) < 60)
                    {
                        modifiers.FinalDamage *= 0.9f;
                        return;
                    }
                }
            }
        }
    }
}
