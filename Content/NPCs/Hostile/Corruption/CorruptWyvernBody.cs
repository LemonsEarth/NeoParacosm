using NeoParacosm.Content.Projectiles.Hostile.Evil;

namespace NeoParacosm.Content.NPCs.Hostile.Corruption;

public class CorruptWyvernBody : ModNPC
{
    float AITimer = 0;

    int FollowingNPC
    {
        get { return (int)NPC.ai[1]; }
    }

    int FollowerNPC
    {
        get { return (int)NPC.ai[0]; }
    }

    int HeadNPC
    {
        get { return (int)NPC.ai[2]; }
    }

    int SegmentNum
    {
        get { return (int)NPC.ai[3]; }
    }

    int AttackTimer = 0;
    int AttackCount = 0;
    float RandNum = 0;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = false;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }

    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.width = 40;
        NPC.height = 40;
        NPC.Opacity = 1;
        NPC.lifeMax = 20000;
        NPC.defense = 25;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit18;
        NPC.DeathSound = SoundID.DD2_BetsyDeath;
        NPC.value = 10000;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.noGravity = true;
        NPC.npcSlots = 1;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(4, NPC.Center, DustID.Corruption, 4f, 4f, 2f, 3.5f);
            LemonUtils.DustBurst(4, NPC.Center, DustID.CursedTorch, 4f, 4f, 2f, 3.5f);
        }
    }

    public override void AI()
    {
        NPC followingNPC = Main.npc[FollowingNPC];

        if (followingNPC is null || !followingNPC.active || followingNPC.friendly || followingNPC.townNPC || followingNPC.lifeMax <= 5)
        {
            NPC.life = 0;
            NPC.active = false;
            LemonUtils.DustBurst(4, NPC.Center, DustID.Corruption, 4f, 4f, 2f, 3.5f);
            LemonUtils.DustBurst(4, NPC.Center, DustID.CursedTorch, 4f, 4f, 2f, 3.5f);
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            return;
        }
        if (AITimer == 0)
        {
            NPC.frame.Y = Main.rand.Next(0, 4) * 40;
        }
        FollowNextSegment(followingNPC);
        NPC.target = followingNPC.target;

        if (NPC.HasValidTarget && LemonUtils.NotClient() && Main.rand.NextBool(500))
        {
            Vector2 dir = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15, 15)));
            LemonUtils.QuickProj(NPC, NPC.RandomPos(), dir * Main.rand.NextFloat(2, 6), ProjectileType<CursedDecayFire>());
        }

        NPC.spriteDirection = followingNPC.spriteDirection;
        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        if (AITimer == 0)
        {
            NPC.frame.Y = Main.rand.Next(0, 4) * frameHeight;
        }
    }

    public override bool CheckActive()
    {
        return false;
    }

    void FollowNextSegment(NPC followingNPC)
    {
        Vector2 toFollowing = followingNPC.Center - NPC.Center;
        NPC.rotation = toFollowing.ToRotation();
        float distance = (toFollowing.Length() - (NPC.height - 8)) / toFollowing.Length();

        Vector2 pos = toFollowing * distance;
        NPC.velocity = Vector2.Zero;
        NPC.position += pos;
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