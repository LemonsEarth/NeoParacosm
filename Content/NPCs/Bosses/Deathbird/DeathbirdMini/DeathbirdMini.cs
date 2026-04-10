using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Content.Items.BossBags;
using NeoParacosm.Content.Items.Placeable.Relics;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using NeoParacosm.Content.NPCs.Hostile.Minions;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.Localization;

namespace NeoParacosm.Content.NPCs.Bosses.Deathbird.DeathbirdMini;

[AutoloadBossHead]
public partial class DeathbirdMini : ModNPC
{
    ref float AITimer => ref NPC.ai[0];
    public float Attack
    {
        get { return NPC.ai[1]; }
        private set
        {
            int diffMod = -1; // One less attack if not in expert
            if (Main.expertMode)
            {
                diffMod = 0;
            }
            int maxVal = 3;
            if (value > maxVal + diffMod || value < 0)
            {
                NPC.ai[1] = 0;
            }
            else
            {
                NPC.ai[1] = value;
            }
        }
    }
    ref float AttackTimer => ref NPC.ai[2];
    ref float AttackCount => ref NPC.ai[3];

    bool phase2Reached = false;
    int phase = 1;
    bool phaseTransition = false;

    float attackDuration = 0;
    int[] attackDurations = { 990, 900, 720, 600 };
    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    public enum Attacks
    {
        HomingDeathflameBalls,
        HoverLingeringFlame,
        LaserFeathers,
        Grab,
    }

    int IntroDuration = 300;
    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];
        SetBodyPartPositions();

        DespawnCheck();
        if (AITimer < IntroDuration)
        {
            Intro();
            AITimer++;
            return;
        }
        if (!phaseTransition) NPC.dontTakeDamage = false;

        if (NPC.life < NPC.lifeMax / 2 && !phase2Reached)
        {
            phaseTransition = true;
            phase2Reached = true;
            AttackTimer = 0;
        }

        if (phaseTransition)
        {
            PhaseTransition();
            AITimer++;
            return;
        }

        switch (Attack)
        {
            case (int)Attacks.HomingDeathflameBalls:
                HomingDeathfireBalls();
                break;
            case (int)Attacks.HoverLingeringFlame:
                HoverLingeringFlame();
                break;
            case (int)Attacks.LaserFeathers:
                LaserFeathers();
                break;
            case (int)Attacks.Grab:
                Grab();
                break;
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }

        AITimer++;
    }

    void SwitchAttacks()
    {
        PlayRoar();
        Terraria.Graphics.Effects.Filters.Scene.Deactivate("NeoParacosm:DeathbirdArenaShader");

        Attack++;
        attackDuration = attackDurations[(int)Attack];

        AttackCount = 0;
        AttackTimer = 0;
        NPC.Opacity = 1f;

        drawClone = false;
        clonePos = Vector2.Zero;
        cloneOpacity = 0f;
        cloneScale = 1f;
    }

    const int HomingDeathfireBallsDuration = 330;
    const int HomingDeathfireBallsMoveTime = 240;
    const int HomingDeathfireBallsFireTime = 150;
    void HomingDeathfireBalls()
    {
        BasicMovementAnimation();
        switch (AttackTimer)
        {
            case HomingDeathfireBallsDuration:
                if (LemonUtils.NotClient())
                {
                    int attemptCount = 0;
                    do
                    {
                        targetPosition = NPC.Center + new Vector2(Main.rand.Next(-250, 250), Main.rand.Next(-250, 250));
                        attemptCount++;
                    }
                    while (attemptCount < 100 && Framing.GetTileSafely(targetPosition.ToTileCoordinates()).HasTile);
                }
                AttackCount++;
                NPC.netUpdate = true;
                break;
            case > HomingDeathfireBallsMoveTime:
                frameDuration = 6;
                NPC.MoveToPos(targetPosition, 0.3f, 0.3f, 0.2f, 0.2f);
                break;
            case > HomingDeathfireBallsFireTime:
                frameDuration = 24;
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y += 0.1f;
                if (AttackTimer % 60 == 0)
                {
                    NPC.velocity.Y -= 5;
                    SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { PitchRange = (0f, 0.2f) }, NPC.Center);
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<SuckyProjectile>(), ai0: 1500, ai1: 50, ai2: 0);
                    }
                }
                break;
            case HomingDeathfireBallsFireTime:
                if (LemonUtils.NotClient())
                {
                    for (int i = 0; i < 3 * LemonUtils.GetDifficulty(); i++)
                    {
                        Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 3);
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), ai0: 120, ai1: NPC.target);
                    }
                }
                break;
            case > HomingDeathfireBallsFireTime - 90:
                NPC.velocity *= 0.98f;
                NPC.velocity.Y += 0.05f;
                frameDuration = 999;
                break;
            case > 0:
                if (attackDuration < 60)
                {
                    // This attack repeats multiple times before attackDuration is up, so we check attackDuration 
                    // to make sure its the last rep
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.01f, 2f);
                }
                frameDuration = 6;
                break;
            case 0:
                AttackTimer = HomingDeathfireBallsDuration;
                return;
        }
        AttackTimer--;
    }

    const int HoverLingeringFlameDuration = 300;
    const int HoverLingeringFlameInterval = 20;
    void HoverLingeringFlame()
    {
        BasicMovementAnimation();
        frameDuration = 6;
        switch (AttackTimer)
        {
            case HoverLingeringFlameDuration:
                TeleportToPos(player.Center - Vector2.UnitY * 300);
                if (LemonUtils.NotClient())
                {
                    for (int i = -6; i <= 6; i++)
                    {
                        Vector2 pos = NPC.Center + Vector2.UnitX * i * 150;
                        LemonUtils.QuickProj(NPC, pos, new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(2, 4)), ProjectileType<LingeringDeathflame>(), ai0: player.whoAmI, ai1: 240);
                    }
                }
                break;
            case > HoverLingeringFlameDuration - 60:
                NPC.velocity = Vector2.Zero;
                break;
            case > 0:
                NPC.MoveToPos(player.Center - Vector2.UnitY * 300, 0.2f, 0.2f, 0.2f, 0.2f);
                if (AttackTimer % HoverLingeringFlameInterval == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.RandomPos(), Vector2.UnitY * 3, ProjectileType<LingeringDeathflame>(), ai0: player.whoAmI, ai1: 210);
                    }
                }
                break;
            case 0:
                AttackTimer = HoverLingeringFlameDuration;
                return;
        }
        AttackTimer--;
    }

    const int LaserFeathersDuration = 240;
    const int LaserFeathersMoveTime = 120;
    void LaserFeathers()
    {
        BasicMovementAnimation();
        switch (AttackTimer)
        {
            case LaserFeathersDuration:
                if (LemonUtils.NotClient())
                {
                    targetPosition = NPC.Center + Main.rand.NextVector2Circular(400, 400);
                }
                NPC.netUpdate = true;
                break;
            case > LaserFeathersMoveTime:
                frameDuration = 12;
                NPC.MoveToPos(targetPosition, 0.2f, 0.2f, 0.1f, 0.1f);
                break;
            case > 0:
                if (attackDuration < 120)
                {
                    DrawClone(player.Center - Vector2.UnitY * 300, 0.005f, 2f);
                }
                NPC.velocity = Vector2.Zero;
                frameDuration = 9999;
                int cd = 20 - ((LemonUtils.GetDifficulty() - 1) * 5);
                if (AttackTimer % cd == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 pos = NPC.Center + new Vector2(16 * i, -8).RotatedBy(AttackCount * MathHelper.ToRadians(50 + AttackCount) * i);
                            Vector2 dir = NPC.Center.DirectionTo(pos);
                            int timeToFire = 90 - (LemonUtils.GetDifficulty() * 5);
                            LemonUtils.QuickProj(NPC, pos, dir * 10, ProjectileType<DeathbirdFeather>(), ai0: timeToFire);
                        }
                    }
                    AttackCount++;
                }
                break;
            case 0:
                AttackTimer = LaserFeathersDuration;
                AttackCount = 0;
                return;
        }

        AttackTimer--;
    }

    int grabProjIdentity = -1;

    const int GrabDuration = 600;
    const int GrabPrepTime = 480;
    const int GrabEndTime = 450;
    Vector2 grabOffset => Vector2.UnitY.RotatedBy(body.rot - MathHelper.PiOver2) * bodyTexture.Height();
    void Grab()
    {
        frameDuration = 6;
        switch (AttackTimer)
        {
            case GrabDuration:
                SetDefaultBodyPartValues();
                TeleportToPos(player.Center - Vector2.UnitY * 300);
                break;
            case > GrabPrepTime:
                targetPosition = player.DirectionTo(NPC.Center);
                NPC.velocity = targetPosition * 2;

                head.rot = Utils.AngleLerp(head.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);
                body.rot = Utils.AngleLerp(body.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);

                leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, body.rot + MathHelper.ToRadians(24), 1 / 40f);
                leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(20), 1 / 40f);

                rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, body.rot + MathHelper.ToRadians(-24), 1 / 40f);
                rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(-20), 1 / 40f);
                break;
            case GrabPrepTime:
                PlayRoar();
                if (LemonUtils.NotClient())
                {
                    grabProjIdentity = LemonUtils.QuickProj(NPC, NPC.Center + grabOffset, Vector2.Zero, ProjectileType<DeathbirdGrab>(), ai0: NPC.whoAmI, ai1: 180).identity;
                }
                NPC.netUpdate = true;
                break;
            case > 300:
                Projectile grabProj = Main.projectile.FirstOrDefault(p => p.identity == grabProjIdentity);
                grabProj.Center = NPC.Center + grabOffset.RotatedBy(MathHelper.PiOver2);
                if (AttackTimer > GrabEndTime)
                {
                    head.rot = Utils.AngleLerp(head.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);
                    body.rot = Utils.AngleLerp(body.rot, targetPosition.ToRotation() + MathHelper.PiOver2, 1 / 20f);

                    leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, -body.rot, 1 / 20f);
                    leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(-60), 1 / 20f);

                    rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, body.rot, 1 / 20f);
                    rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(60), 1 / 20f);
                    NPC.velocity = -targetPosition * 25; // target position = playerToNPC
                }
                else
                {
                    NPC.velocity *= 0.9f;
                    head.rot = Utils.AngleLerp(head.rot, MathHelper.ToRadians(NPC.velocity.X * 6), 1 / 20f);
                    body.rot = Utils.AngleLerp(body.rot, MathHelper.ToRadians(NPC.velocity.X * 3), 1 / 20f);

                    leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
                    leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, body.rot + MathHelper.ToRadians(-60), 1 / 20f);

                    rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, MathHelper.ToRadians(NPC.velocity.X * 10), 1 / 20f);
                    rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, body.rot + MathHelper.ToRadians(60), 1 / 20f);

                    if (AITimer % 45 == 0)
                    {
                        if (LemonUtils.NotClient())
                        {
                            for (int i = 0; i < LemonUtils.GetDifficulty(); i++)
                            {
                                Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2);
                                LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), ai0: 20, ai1: NPC.target);
                            }
                        }
                    }
                }
                break;
            case 300:
                SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.7f, 1f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.4f, 0.9f) }, NPC.Center);
                break;
            case > 0:
                BasicMovementAnimation();
                if (AITimer % 30 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        for (int i = 0; i < LemonUtils.GetDifficulty(); i++)
                        {
                            Vector2 velocity = Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 6);
                            LemonUtils.QuickProj(NPC, NPC.RandomPos(64, 64), velocity, ProjectileType<DeathflameBall>(), ai0: 9999, ai1: NPC.target);
                        }
                    }
                }
                break;
            case 0:
                AttackTimer = GrabDuration;
                return;
        }
        AttackTimer--;
    }

    void DespawnCheck()
    {
        if ((player.dead || !player.active || NPC.Center.Distance(player.MountedCenter) > 2500) && !phaseTransition)
        {
            NPC.active = false;
            NPC.life = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
    }

    void Intro()
    {
        NPC.dontTakeDamage = true;
        NPC.velocity = Vector2.Zero;
        if (AITimer == 0)
        {
            SetDefaultBodyPartValues();
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.2f, 0.5f) }, NPC.Center);
        }

        if (AITimer < IntroDuration - 120)
        {
            NPC.Opacity = 0f;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.Ash, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.Black).noGravity = true;
                Dust.NewDustDirect(NPC.RandomPos(16, 16), 2, 2, DustID.GemDiamond, Scale: Main.rand.NextFloat(1f, 4f), newColor: Color.White).noGravity = true;
            }
        }
        else if (AITimer == IntroDuration - 120)
        {
            NPC.Opacity = 1f;
            for (int i = 0; i < 3; i++)
            {
                LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(2f, 4f));
            }
            PunchCameraModifier mod1 = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 6f, 90, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            PlayRoar();
        }
        attackDuration = attackDurations[(int)Attack];
    }
    void PhaseTransition()
    {
        NPC.dontTakeDamage = true;
        SetDefaultBodyPartValues();
        frameDuration = 6;
        BasicMovementAnimation();
        drawClone = false;

        switch (AttackTimer)
        {
            case 0:
                PlayRoar();
                break;
            case < 120:
                NPC.MoveToPos(NPC.Center + Main.rand.NextVector2Circular(500, 500), 0.1f, 0.1f, 0.2f, 0.2f);
                break;
            case >= 120 and < 240:
                NPC.MoveToPos(NPC.Center - Vector2.UnitY * 500, 0.1f, 0.1f, 0.4f, 0.4f);
                break;
            case >= 240:
                if (LemonUtils.NotClient())
                {
                    Item.NewItem(NPC.GetSource_Death(), NPC.Center, ItemType<RuneOfPeridition>());
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.NeoParacosm.NPCs.DeathbirdMini.FleeMessage"), Color.MediumPurple);
                }
                NPC.active = false;
                DownedBossSystem.downedDeathbirdMini = true;
                return;
        }

        AttackTimer++;
    }

    void TeleportToPos(Vector2 pos)
    {
        LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.Ash, Main.rand.NextFloat(3f, 4f), color: Color.Black);
        NPC.Center = pos;
        LemonUtils.DustCircle(NPC.RandomPos(), 16, 12, DustID.GemDiamond, Main.rand.NextFloat(3f, 4f));
        SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { PitchRange = (0f, 0.2f) }, NPC.Center);
    }
}
