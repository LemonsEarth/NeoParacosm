using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.NPCs.Friendly.Special;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class BranchOfLifeSentry : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    ref float Attack => ref Projectile.ai[2];
    const int BUFF_DISTANCE = 400;
    NPC closestEnemy;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 116;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = true;
        Projectile.timeLeft = Projectile.SentryLifeTime;
        Projectile.friendly = false;
        Projectile.sentry = true;
    }

    public override void AI()
    {
        closestEnemy = LemonUtils.GetClosestNPC(Projectile.Center, BUFF_DISTANCE + 200);
        if (AITimer % 30 == 0)
        {
            foreach (Player player in Main.player)
            {
                if (Vector2.Distance(Projectile.Center, player.Center) < BUFF_DISTANCE)
                {
                    player.AddBuff(BuffType<BranchedOfLifedBuff>(), 60);
                }
            }
        }

        if (closestEnemy != null && closestEnemy.active && AITimer > 60)
        {
            switch (Attack)
            {
                case 0:
                    PoisonBloomAttack();
                    break;
                case 1:
                    ShiverthornAttack();
                    break;
                case 2:
                    WaterfallerAttack();
                    break;
                case 3:
                    BlinkrootAttack();
                    break;
                case 4:
                    ShadowFlowerAttack();
                    break;
                case 5:
                    FireBloomAttack();
                    break;
                case 6:
                    MoonBurstAttack();
                    break;
                case 7:
                    DeathSeederAttack();
                    break;
            }
            AttackTimer++;
        }


        Projectile.velocity.Y = 10f;
        if (AttackTimer == 181)
        {
            AttackTimer = 0;
            Attack++;
            if (Attack >= 8)
            {
                Attack = 0;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Color color = Color.White;
                switch (Attack)
                {
                    case 0:
                        color = Color.LimeGreen;
                        break;
                    case 1:
                        color = Color.SkyBlue;
                        break;
                    case 2:
                        color = Color.DodgerBlue;
                        break;
                    case 3:
                        color = Color.Gold;
                        break;
                    case 4:
                        color = Color.MediumPurple;
                        break;
                    case 5:
                        color = Color.Red;
                        break;
                    case 6:
                        color = Color.Cyan;
                        break;
                    case 7:
                        color = Color.White;
                        break;
                }
                LemonUtils.QuickPulse(Projectile, Projectile.Center, 1f, 5f, 5f, color);
            }
        }

        Projectile.StandardAnimation(12, 4);
        Lighting.AddLight(Projectile.Center, 5, 2, 2);
        AITimer++;
    }

    void DeathSeederAttack()
    {
        if (AttackTimer == 0 && Main.myPlayer == Projectile.owner)
        {
            NPC npc = NPC.NewNPCDirect(Projectile.GetSource_FromAI("BranchOfLifeSpawn"), Projectile.Top, NPCType<FriendlyGuardian>(),
                ai1: Projectile.damage / 77f, ai2: Projectile.damage / 77f, ai3: Projectile.owner);
        }
    }

    void FireBloomAttack()
    {
        int attackCD = AttackTimer switch
        {
            <= 90 => 45,
            <= 150 => 30,
            _ => 10
        };

        if (AttackTimer % attackCD == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Torch, 0, -5, Scale: Main.rand.NextFloat(2, 4f)).noGravity = true;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        -Vector2.UnitY.RotatedBy(
                            Main.rand.NextBool().ToDirectionInt() * MathHelper.ToRadians(Main.rand.NextFloat(AttackTimer / 45f, AttackTimer / 30f))) * 
                            Main.rand.NextFloat(AttackTimer / 20f, AttackTimer / 15f),
                        ProjectileID.BallofFire,
                        knockback: 6f
                        );
                }
            }
        }
    }

    void ShiverthornAttack()
    {
        if (AttackTimer <= 60 || AttackTimer >= 120)
        {
            if (AttackTimer % 20 == 0 && Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Projectile.DirectionTo(closestEnemy.Center) * 8,
                    ProjectileType<IceProjectile>()
                    );
            }
        }
    }

    void MoonBurstAttack()
    {
        if (AttackTimer == 90)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = -8; i < 8; i++)
                {
                    Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(50, 100) * i, -800);
                    LemonUtils.QuickProj(
                        Projectile,
                        pos,
                        Vector2.UnitY * Main.rand.NextFloat(5, 8),
                        ProjectileType<MoonBurstProjectile>(),
                        ai1: 1,
                        ai2: 1f
                        );
                }
            }
        }
    }

    void PoisonBloomAttack()
    {
        if (AttackTimer % 45 == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float randSpeed = Main.rand.NextFloat(4, 6);
                for (int i = 0; i < 12; i++)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 12)) * randSpeed,
                        ProjectileType<PoisonBloomPetal>()
                        );
                }
            }
        }
    }

    void ShadowFlowerAttack()
    {
        if (AttackTimer % 60 == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = -4; i <= 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(closestEnemy.Center).RotatedBy(MathHelper.ToRadians(9) * i) * 7, ProjectileType<ShadowBolt>(), Projectile.damage, 1f, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.5f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }
    }

    void WaterfallerAttack()
    {
        if (AttackTimer % 60 == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-40, 40), -500 + Main.rand.NextFloat(-40, 40));
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), closestEnemy.Center + offset, Vector2.UnitY * 2, ProjectileType<WaterfallDrop>(), Projectile.damage, 1f, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.Item21 with { Volume = 0.5f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }
    }

    void BlinkrootAttack()
    {
        if (AttackTimer % 90 == 0)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && Vector2.Distance(Projectile.Center, npc.Center) < BUFF_DISTANCE + 200)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 offset = new Vector2(Main.rand.NextFloat(-20, 20), Main.rand.NextFloat(-20, 20));
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center + offset, Vector2.Zero, ProjectileType<BranchOfLifeProj>(), Projectile.damage, 1f, Projectile.owner);
                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.3f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<PulseEffect>(), ai0: 1, ai1: 8, ai2: 2);
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y = 0;
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }
}
