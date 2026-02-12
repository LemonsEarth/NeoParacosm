using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.NPCs.Bosses.EmperorColdsteel;

public class EmperorColdsteelBody : ModNPC
{
    float AITimer = 0;

    int FollowingNPC
    {
        get { return (int)NPC.ai[0]; }
    }

    int HeadNPC
    {
        get { return (int)NPC.ai[1]; }
    }

    int SegmentNum
    {
        get { return (int)NPC.ai[2]; }
    }

    int AttackTimer = 0;
    int AttackCount = 0;
    float RandNum = 0;

    EmperorColdsteelHead head;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.width = 128;
        NPC.height = 128;
        NPC.Opacity = 1;
        NPC.lifeMax = 400000;
        NPC.defense = 40;
        NPC.damage = 100;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 30000;
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

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.damage = (int)(NPC.damage * balance * 0.5f);
        NPC.lifeMax = (int)(NPC.lifeMax * balance * 0.5f);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(NPC.Opacity);
        writer.Write(RandNum);
        writer.Write(AttackCount);
        writer.Write(AttackTimer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NPC.Opacity = reader.ReadSingle();
        RandNum = reader.ReadSingle();
        AttackCount = reader.ReadInt32();
        AttackTimer = reader.ReadInt32();
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void AI()
    {
        NPC followingNPC = Main.npc[FollowingNPC];
        NPC headNPC = Main.npc[HeadNPC];
        Lighting.AddLight(NPC.Center, 0.8f, 0.8f, 1f);
        if (followingNPC is null || !followingNPC.active || followingNPC.friendly || followingNPC.townNPC || followingNPC.lifeMax <= 5)
        {
            NPC.active = false;
            NPC.life = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
        head = (EmperorColdsteelHead)headNPC.ModNPC;

        NPC.Opacity = head.NPC.Opacity;
        FollowNextSegment(followingNPC);
        NPC.spriteDirection = followingNPC.spriteDirection;

        AITimer++;
    }

    void FollowNextSegment(NPC followingNPC)
    {
        Vector2 toFollowing = NPC.DirectionTo(followingNPC.Center);
        NPC.rotation = toFollowing.ToRotation() + MathHelper.PiOver2;
        float distance = NPC.height;
        Vector2 posOffset = -toFollowing * distance;
        NPC.Center = followingNPC.Center + posOffset;
        NPC.velocity = Vector2.Zero;
    }

    public void SwitchAttacks(int attack)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            AttackTimer = 0;
            AttackCount = 0;
            RandNum = 0;
        }
        NPC.netUpdate = true;
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {

    }

    public override bool CheckActive()
    {
        return false;
    }

    public override bool CheckDead()
    {
        return true;
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }
}