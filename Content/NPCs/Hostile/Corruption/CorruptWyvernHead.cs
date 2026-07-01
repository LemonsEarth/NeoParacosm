using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class CorruptWyvernHead : ModNPC
{
    int BodyType => NPCType<CorruptWyvernBody>();
    int TailType => NPCType<CorruptWyvernTail>();
    const int MAX_SEGMENT_COUNT = 20;
    int SegmentCount = 0;

    ref float AITimer => ref NPC.ai[0];

    public override void SetStaticDefaults()
    {
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = false;
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { };
        /*{
            PortraitScale = 0.2f,
            PortraitPositionYOverride = -150
        };*/
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            });
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        bool inCorruptSpace = spawnInfo.Player.ZoneCorrupt && spawnInfo.Player.ZoneSkyHeight;

        if (inCorruptSpace)
        {
            return DarkCataclysmSystem.DarkCataclysmActive ? 0.2f : 0.05f;
        }
        return 0;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(8, NPC.Center, DustID.Corruption, 4f, 4f, 2f, 3.5f);
            LemonUtils.DustBurst(8, NPC.Center, DustID.CursedTorch, 4f, 4f, 2f, 3.5f);
        }
    }

    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.width = 80;
        NPC.height = 40;
        NPC.Opacity = 1;
        NPC.lifeMax = 20000;
        NPC.defense = 15;
        NPC.damage = 80;
        NPC.HitSound = SoundID.NPCHit18;
        NPC.DeathSound = SoundID.DD2_BetsyDeath;
        NPC.value = 50000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 2;
    }

    public override void AI()
    {
        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(false);
        }
        if (AITimer == 0)
        {
            NPC.velocity = Vector2.UnitY * 5;
        }

        NPC.CheckAndDespawn();
        if (!NPC.HasValidTarget)
        {
            MoveToPos(Vector2.Zero, 30f, 25f);

        }
        Player player = Main.player[NPC.target];
        MoveToPos(player.Center, 30f, 15f);
        if (!NPC.velocity.HasNaNs())
        {
            NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
        }
        NPC.rotation = NPC.spriteDirection == -1 ? NPC.velocity.ToRotation() : NPC.velocity.ToRotation() + MathHelper.Pi;
        if (AITimer == 0)
        {
            SpawnSegments();
            PlayRoar(1.5f);
        }
        AITimer++;
    }

    void MoveToPos(Vector2 pos, float turningSpeedDegreesDenominator = 60f, float moveSpeed = 8f)
    {
        Vector2 dirToPos = NPC.DirectionTo(pos);
        float angleBetween = MathHelper.ToDegrees(LemonUtils.AngleBetween(NPC.velocity, dirToPos));
        //Main.NewText(angleBetween);
        if (MathF.Abs(angleBetween) > MathHelper.ToRadians(5))
        {
            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(angleBetween / turningSpeedDegreesDenominator)) * moveSpeed;
        }
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return true;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {

    }

    void SpawnSegments()
    {
        int latestNPC = NPC.whoAmI;
        while (SegmentCount < MAX_SEGMENT_COUNT - 2) // Body segments, excluding head and tail
        {
            latestNPC = SpawnSegment(BodyType, latestNPC);
            CorruptWyvernBody bodySegment = (CorruptWyvernBody)Main.npc[latestNPC].ModNPC;
            SegmentCount++;
        }

        SpawnSegment(TailType, latestNPC);
    }

    int SpawnSegment(int type, int latestNPC)
    {
        int oldestNPC = latestNPC;
        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC, NPC.whoAmI, SegmentCount);

        Main.npc[oldestNPC].ai[0] = latestNPC;
        Main.npc[latestNPC].realLife = NPC.whoAmI;
        return latestNPC;
    }


    void PlayRoar(float volume = 1f)
    {
        //SoundEngine.PlaySound(SoundID.DD2_BetsyDeath with { Volume = volume, PitchRange = (-0.4f, 0.8f) }, NPC.Center);
        //SoundEngine.PlaySound(SoundID.Roar with { Volume = volume, PitchRange = (-1f, -0.8f) }, NPC.Center);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    /*public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (phase == 1)
        {
            return true;
        }

        Asset<Texture2D> textureAsset = ModContent.Request<Texture2D>("Paracosm/Assets/Textures/Boss/NebulaMasterTrail");
        Texture2D texture = textureAsset.Value;

        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, NPC.height * 0.5f);
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (NPC.spriteDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        for (int k = NPC.oldPos.Length - 1; k >= 0; k--)
        {
            Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;
            Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
            float scale = 1f;
            if (NPC.oldPos.Length - k > 0)
            {
                float posMod = 1f / (NPC.oldPos.Length - k);
                scale = ((float)Math.Sin(MathHelper.ToRadians(AITimer)) + 1) * 0.5f + posMod;
            }
            Main.EntitySpriteDraw(texture, drawPos, null, color, NPC.rotation, drawOrigin, scale, spriteEffects, 0);
        }
        return true;
    }*/
}