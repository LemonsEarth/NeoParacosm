using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile;
using NeoParacosm.Content.Projectiles.Hostile.Researcher;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Boss;
using NeoParacosm.Core.UI;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

[AutoloadBossHead]
public class Dreadlord : ModNPC
{
    #region Attack Fields and Data
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
            int maxVal = 1;
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

    float attackDuration = 0;
    int[] attackDurations = { 600, 600, 600, 600, 600 };

    public enum Attacks
    {

    }
    #endregion

    #region Constants
    const int INTRO_DURATION = 60;
    #endregion

    #region Body Parts
    DreadlordBodyPart HeadCorrupt = new DreadlordBodyPart();
    DreadlordBodyPart HeadCrimson = new DreadlordBodyPart();
    DreadlordBodyPart Body = new DreadlordBodyPart();
    DreadlordBodyPart FrontLegClose = new DreadlordBodyPart();
    DreadlordBodyPart FrontFootClose = new DreadlordBodyPart();
    DreadlordBodyPart FrontLegFar = new DreadlordBodyPart();
    DreadlordBodyPart FrontFootFar = new DreadlordBodyPart();
    DreadlordBodyPart BackLegClose = new DreadlordBodyPart();
    DreadlordBodyPart BackFootClose = new DreadlordBodyPart();
    DreadlordBodyPart BackLegFar = new DreadlordBodyPart();
    DreadlordBodyPart BackFootFar = new DreadlordBodyPart();

    static Asset<Texture2D> neckTexture;

    #endregion

    int facingDirection = 1;

    Vector2 lookPosition = Vector2.Zero;
    public Player player { get; private set; }


    public override void Load()
    {
        neckTexture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordNeck");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
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
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("Bestiary")),
            });
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override void SetDefaults()
    {
        NPC.width = 800;
        NPC.height = 334;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.Opacity = 1f;
        NPC.lifeMax = 80000;
        NPC.defense = 40;
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
            Music = MusicLoader.GetMusicSlot(Mod, "Common/Assets/Audio/Music/ChaosCognition");
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.NP().SetDamageReductions(
            (DamageClass.Melee, 10f),
            (DamageClass.Ranged, 15f),
            (DamageClass.Magic, 15f),
            (DamageClass.Summon, 10f),
            (DamageClass.SummonMeleeSpeed, 60f)
            );
        HeadCorrupt.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordHeadCorrupt");
        HeadCrimson.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordHeadCorrupt");
        Body.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordBody");
        FrontLegClose.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegFrontClose");
        FrontFootClose.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordFootFrontClose");
        FrontLegFar.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegFrontClose");
        FrontFootFar.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordFootFrontClose");
        BackLegClose.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegBackClose");
        BackFootClose.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordFootBackClose");
        BackLegFar.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordLegBackClose");
        BackFootFar.Texture = Request<Texture2D>("NeoParacosm/Content/NPCs/Bosses/Dreadlord/DreadlordFootBackClose");
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        player = Main.player[NPC.target];
        DespawnCheck();

        if (AITimer < INTRO_DURATION)
        {
            Intro();
            AITimer++;
            return;
        }

        AttackControl();

        AITimer++;
    }

    void AttackControl()
    {
        /*switch (Attack)
        {
            case (int)Attacks.SavBlastDirect:
                SavBlastDirect();
                break;
            case (int)Attacks.RocketSpam:
                RocketSpam();
                break;
            case (int)Attacks.BulletSpam:
                BulletSpam();
                break;
            case (int)Attacks.SavBlastBurst:
                SavBlastBurst();
                break;
            case (int)Attacks.DroneSpam:
                DroneSpam();
                break;

        }*/

        attackDuration--;
        if (attackDuration <= 0)
        {
            SwitchAttacks();
        }
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

    public override bool CheckDead()
    {
        return true;
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
        /*BackFootFar.Draw();
        BackLegFar.Draw();
        FrontFootFar.Draw();
        FrontLegFar.Draw();*/

        

        Body.Draw();
        Vector2 neckBaseToHeadCorrupt = Body.MiscPosition.DirectionTo(HeadCorrupt.Position);
        float neckBaseDistanceToHeadCorrupt = Body.MiscPosition.Distance(HeadCorrupt.Position);
        float distanceLeft = neckBaseDistanceToHeadCorrupt;
        Vector2 drawOrigin = neckTexture.Size() * 0.5f;
        Vector2 drawPos = Body.MiscPosition;
        while (distanceLeft > 0)
        {
            float rotation = drawPos.DirectionTo(drawPos + (neckTexture.Height() * neckBaseToHeadCorrupt)).ToRotation() + MathHelper.PiOver2;   
            drawPos += (neckTexture.Height() / 2f * neckBaseToHeadCorrupt);
            Main.EntitySpriteDraw(neckTexture.Value, drawPos - Main.screenPosition, null, drawColor, rotation, drawOrigin, 1f, SpriteEffects.None);
            distanceLeft -= neckTexture.Height() / 2f;
        }
        HeadCorrupt.Draw();
        //HeadCrimson.Draw();
        FrontLegClose.Draw();
        FrontFootClose.Draw();
        BackLegClose.Draw();
        BackFootClose.Draw();

        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
}
