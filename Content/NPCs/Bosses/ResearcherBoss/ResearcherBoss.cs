using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.BossBags;
using NeoParacosm.Content.Items.Placeable.Relics;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using NeoParacosm.Content.NPCs.Hostile.Minions;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Content.Projectiles.Hostile.Researcher;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.ResearcherBoss;

[AutoloadBossHead]
public class ResearcherBoss : ModNPC
{
    static Asset<Texture2D> gunTexture;

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
            int maxVal = phase == 1 ? 1 : 0;
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
    int[] attackDurations = { 600, 600, 600 };
    public Player player { get; private set; }
    Vector2 targetPosition = Vector2.Zero;

    float gunRotation = 0;



    public enum Attacks
    {
        SavBlastDirect,
        RocketSpam,
        SavBlastBurst,
        RocketSpam2
    }

    public override void Load()
    {
        gunTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/ResearcherBoss/ResearcherBoss_Gun");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { };
        /*{
            PortraitScale = 0.2f,
            PortraitPositionYOverride = -150
        };*/
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    public override void SetDefaults()
    {
        NPC.width = 80;
        NPC.height = 50;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1;
        NPC.lifeMax = 24000;
        NPC.defense = 32;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 300000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 10;
        NPC.SpawnWithHigherTime(30);

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/Gravefield");
        }
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    int IntroDuration = 60;
    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];

        DespawnCheck();
        NPCGunDirectionAndRotation();
        Dust.NewDustPerfect(NPC.Center + Vector2.UnitX * (Main.rand.NextFloat(8, 16) * -NPC.direction), DustID.IceTorch, Vector2.UnitY * Main.rand.NextFloat(1, 3));
        if (AITimer < IntroDuration)
        {
            Intro();
            AITimer++;
            return;
        }
        if (phaseTransition || AITimer < IntroDuration || Collision.SolidTiles(NPC.position, NPC.width, NPC.height))
        {
            NPC.dontTakeDamage = true;
        }
        else
        {
            NPC.dontTakeDamage = false;
        }


        targetPosition = player.Center;


        if (phase == 1)
        {
            switch (Attack)
            {
                case (int)Attacks.SavBlastDirect:
                    SavBlastDirect();
                    break;
                case (int)Attacks.RocketSpam:
                    RocketSpam();
                    break;
            }
        }

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }

        AITimer++;
    }

    Vector2 GetRandomPos(int width, int height)
    {
        int attemptCount = 1000;
        if (LemonUtils.NotClient())
        {
            while (attemptCount > 0)
            {
                int i = (int)(player.Center.X / 16) + Main.rand.Next(-width, width);
                int j = (int)(player.Center.Y / 16) + Main.rand.Next(-height, height);
                Vector2 worldVector = new Vector2(i, j).ToWorldCoordinates();
                if (!WorldGen.InWorld(i, j))
                {
                    attemptCount--;
                    continue;
                }

                if (Collision.SolidTiles(worldVector, 48, 48))
                {
                    attemptCount--;
                    continue;
                }

                if (!Collision.CanHitLine(worldVector, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    attemptCount--;
                    continue;
                }

                randomPos = worldVector;
                break;
            }
        }
        NPC.netUpdate = true;
        return randomPos;
    }

    Vector2 randomPos = Vector2.Zero;
    const float SAV_BLAST_DIRECT_DURATION = 120;
    void SavBlastDirect()
    {
        targetPosition = player.Center;
        switch (AttackTimer)
        {
            case SAV_BLAST_DIRECT_DURATION:
                GetRandomPos(40, 40);  
                break;
            case > 0:
                if (randomPos != Vector2.Zero)
                {
                    NPC.MoveToPos(randomPos, 0.3f, 0.3f, 0.1f, 0.1f);
                }
                else
                {
                    randomPos = targetPosition;
                }

                if (AITimer % 60 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, NPC.DirectionTo(targetPosition) * 12, ProjectileType<SavBlast>());
                    }
                }
                break;
            case 0:
                AttackTimer = SAV_BLAST_DIRECT_DURATION;
                return;
        }


        AttackTimer--;
    }

    const float ROCKET_SPAM_DURATION = 300;
    void RocketSpam()
    {
        targetPosition = player.Center;
        switch (AttackTimer)
        {
            case ROCKET_SPAM_DURATION:
                GetRandomPos(40, 40);
                break;
            case > ROCKET_SPAM_DURATION - 90:
                if (randomPos != Vector2.Zero)
                {
                    NPC.MoveToPos(randomPos, 0.4f, 0.4f, 0.15f, 0.15f);
                }
                else
                {
                    randomPos = targetPosition;
                }

                if (AITimer % 90 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, NPC.DirectionTo(targetPosition).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 12, ProjectileType<SavBlast>());
                    }
                }

                if (AITimer % 20 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, 
                            NPC.DirectionTo(targetPosition).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 2, 
                            ProjectileType<SavMissile>(), ai1: 120);
                    }
                }
                break;
            case > ROCKET_SPAM_DURATION - 180:
                if (AITimer % 30 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, NPC.DirectionTo(targetPosition) * 14, ProjectileType<SavBlast>());
                    }
                }
                break;
            case > 0:
                if (randomPos != Vector2.Zero)
                {
                    NPC.MoveToPos(randomPos, 0.4f, 0.4f, 0.15f, 0.15f);
                }
                else
                {
                    randomPos = targetPosition;
                }

                if (AITimer % 90 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center, NPC.DirectionTo(targetPosition).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 12, ProjectileType<SavBlast>());
                    }
                }

                if (AITimer % 25 == 0)
                {
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(NPC, NPC.Center,
                            NPC.DirectionTo(targetPosition).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 5,
                            ProjectileType<SavMissile>(), ai1: 60);
                    }
                }
                break;
            case 0:
                AttackTimer = ROCKET_SPAM_DURATION;
                return;
        }
        AttackTimer--;
    }

    void NPCGunDirectionAndRotation()
    {
        Vector2 directionToTarget = NPC.DirectionTo(targetPosition);

        NPC.direction = MathF.Sign(directionToTarget.X) == 0 ? 1 : MathF.Sign(directionToTarget.X);

        if (NPC.direction == 1)
        {
            gunRotation = directionToTarget.ToRotation();
        }
        else
        {
            gunRotation = directionToTarget.ToRotation() + MathHelper.Pi;
        }
        NPC.spriteDirection = NPC.direction;
    }

    void SwitchAttacks()
    {
        Attack++;
        attackDuration = attackDurations[(int)Attack];

        AttackCount = 0;
        AttackTimer = 0;
        NPC.Opacity = 1f;
    }

    void DespawnCheck()
    {
        if (player.dead || !player.active || NPC.Center.Distance(player.MountedCenter) > 5000)
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


        attackDuration = attackDurations[(int)Attack];
    }



    public override void OnKill()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Main.EntitySpriteDraw(gunTexture.Value, NPC.Center - Main.screenPosition, null, Color.White, gunRotation, gunTexture.Size() * 0.5f, NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection));
    }
}
