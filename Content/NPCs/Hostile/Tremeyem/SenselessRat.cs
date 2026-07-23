using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.NPCs.Hostile.Tremeyem;

public class SenselessRat : ModNPC
{
    int AITimer = 0;

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 2;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 88;
        NPC.height = 42;
        NPC.lifeMax = 80;
        NPC.defense = 3;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 50;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 1f;
    }

    public override bool PreAI()
    {
        if (NPC.velocity == Vector2.Zero)
        {
            NPC.velocity.Y -= 5;
        }
        NPC.TargetClosest();
        if (AITimer == 0)
        {

        }
        if (MathF.Abs(NPC.velocity.X) < 10 && NPC.direction == LemonUtils.Sign(NPC.velocity.X, 1)) NPC.velocity.X *= 1.15f;
        NPC.spriteDirection = -NPC.direction;
        if (NPC.collideY && AITimer % 90 == 0)
        {
            NPC.velocity.Y -= 5;

        }
        if (Main.rand.NextBool(20))
        {
            SoundEngine.PlaySound(SoundID.WormDig with { PitchRange = (0.5f, 0.8f), Volume = 0.6f }, NPC.Center);
            SoundEngine.PlaySound(SoundID.WormDig with { PitchRange = (-0.8f, -0.5f), Volume = 0.6f }, NPC.Center);

        }

        return true;
    }



    public override void AI()
    {

        if (AITimer == 0)
        {

        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int frameDur = 6;
        NPC.frameCounter++;
        if (NPC.frameCounter > frameDur)
        {
            NPC.frame.Y += frameHeight;
            NPC.frameCounter = 0;
            if (NPC.frame.Y > 1 * frameHeight)
            {
                NPC.frame.Y = 0;
            }
        }
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            LemonUtils.DustBurst(10, NPC.Center, DustType<FireDust>(), 3, 3, 0.6f, 1f, Color.Black);
        }
        else
        {
            Dust.NewDustDirect(NPC.RandomPos(-8, -8), 2, 2, DustID.Stone);
        }
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        modifiers.Knockback += 1;
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.CountsAsTrueMelee())
        {
            modifiers.Knockback += 1;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
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
