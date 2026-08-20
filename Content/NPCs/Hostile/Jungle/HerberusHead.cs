using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.NPCs.Hostile.Jungle;

public class HerberusHead : ModNPC
{
    int AITimer = 0;
    public int Mode = 0;
    public int BodyWhoAmI = 0;
    public int HeadNum = 0;

    NPC bodyNPC;

    static Asset<Texture2D> vineTexture;

    public override void SetStaticDefaults()
    {
        vineTexture = Request<Texture2D>(Texture + "Vine");
        Main.npcFrameCount[NPC.type] = 2;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 36;
        NPC.height = 36;
        NPC.lifeMax = 300;
        NPC.defense = 8;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.8f;
        NPC.dontTakeDamage = true;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
    }

    Vector2 GetDefaultPosition()
    {
        if (HeadNum == 1)
        {
            return bodyNPC.Center + new Vector2(48 * bodyNPC.spriteDirection, -48);
        }
        else if (HeadNum == 2)
        {
            return bodyNPC.Center + new Vector2(64 * bodyNPC.spriteDirection, -24);
        }
        else
        {
            return bodyNPC.Center + new Vector2(48 * bodyNPC.spriteDirection, 0);
        }
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                Mode = Main.rand.Next(0, 2);

            }
            NPC.netUpdate = true;
            bodyNPC = Main.npc[BodyWhoAmI];
        }

        if (!bodyNPC.active || bodyNPC.type != NPCType<Herberus>() || bodyNPC.life == 0)
        {
            NPC.active = false;
            return;
        }
        NPC.Center = Vector2.Lerp(NPC.Center, GetDefaultPosition(), 1 / 4f);

        NPC.target = bodyNPC.target;
        if (!NPC.HasValidTarget)
        {
            NPC.rotation = 0f;
            AITimer++;
            return;
        }
        Player player = NPC.GetTarget();
        float targetRot = NPC.Center.DirectionTo(player.Center).ToRotation() + MathHelper.Pi;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 1 / 20f);

        AITimer++;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = Mode * frameHeight;
    }


    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (AITimer < 3)
        {
            return true;
        }
        Vector2 startDrawPos = bodyNPC.Center + new Vector2(30 * bodyNPC.spriteDirection, -4);
        Vector2 endDrawPos = NPC.Center;
        Vector2 startToEnd = endDrawPos - startDrawPos;
        Vector2 startToEndDir = startToEnd.SafeNormalize(Vector2.Zero);
        Vector2 drawPos = startDrawPos;
        float rot = startToEnd.ToRotation();

        int segmentCountToDraw = (int)(startToEnd.Length() / 10) + 1;
        int segmentsDrawn = 0;
        while (segmentsDrawn < segmentCountToDraw)
        {
            Rectangle frame = vineTexture.Frame(1, 4, 0, segmentsDrawn % 4);
            Main.EntitySpriteDraw(vineTexture.Value, drawPos - screenPos, frame, drawColor, rot, frame.Size() * 0.5f, NPC.scale, SpriteEffects.None);
            drawPos += startToEndDir * 10;
            segmentsDrawn++;
        }
        
        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemType<EclipseGreatshield>(), 10, minimumDropped: 1, maximumDropped: 1));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return NPC.ShouldFallThroughPlatforms(8);
    }
}


