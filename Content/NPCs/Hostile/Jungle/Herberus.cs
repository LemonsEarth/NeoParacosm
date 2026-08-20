using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.NPCs.Hostile.Jungle;

public class Herberus : ModNPC
{
    int AITimer = 0;
    bool stationary = false;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 48;
        NPC.lifeMax = 300;
        NPC.defense = 8;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 1000;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.8f;
    }

    public override bool PreAI()
    {
        if (AITimer == 0)
        {

        }

        NPC.TargetClosest();
        NPC.spriteDirection = NPC.direction;

        return true;
    }

    public override void AI()
    {
        //Main.NewText(NPC.spriteDirection);
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                for (int i = 1; i <= 3; i++)
                {
                    NPC npc = NPC.NewNPCDirect(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        NPCType<HerberusHead>()
                        );

                    if (npc.ModNPC is HerberusHead head)
                    {
                        head.BodyWhoAmI = NPC.whoAmI;
                        head.HeadNum = i;
                    }
                }
            }
        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        if (stationary)
        {
            NPC.frame.Y = 0;
            return;
        }

        if (NPC.velocity.Y != 0)
        {
            NPC.frame.Y = 3 * frameHeight;
            return;
        }

        int frameDur = 6;
        NPC.frameCounter++;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 3 * frameHeight)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        LemonUtils.DustBurst(10, NPC.Center, DustID.JungleGrass, 3, 3, 0.6f, 1f);
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {

    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

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


