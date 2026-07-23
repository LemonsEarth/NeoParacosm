using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.NPCs.Hostile.Tremeyem;

public class FacelessSpearman : ModNPC
{
    int AITimer = 0;
    bool stabbing = false;
    int stabbingTimer = 0;
    int stabbingPrepDuration = 0;
    int stabbingStabDuration = 0;

    int stabbingCooldownTimer = 0;
    int stabbingCooldown = 60;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(stabbingPrepDuration);
        writer.Write(stabbingStabDuration);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        stabbingPrepDuration = reader.ReadInt32();
        stabbingStabDuration = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 7;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 3;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 56;
        NPC.lifeMax = 100;
        NPC.defense = 3;
        NPC.damage = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 50;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        //AIType = NPCID.DesertBeast;
        NPC.knockBackResist = 0.6f;
    }

    public override bool PreAI()
    {
        if (AITimer == 0)
        {
            if (LemonUtils.NotClient())
            {
                if (stabbingPrepDuration == 0) stabbingPrepDuration = Main.rand.Next(90, 120);
                if (stabbingStabDuration == 0) stabbingStabDuration = Main.rand.Next(60, 90);
            }
            NPC.netUpdate = true;
        }
        NPC.TargetClosest();

        if (!NPC.HasValidTarget)
        {
            NPC.velocity.X = 0;
            return false;
        }

        Player player = NPC.GetTarget();
        Vector2 toPlayer = player.Center - NPC.Center;
        if (stabbingCooldownTimer == 0)
        {
            TrySetStabbing(player, toPlayer);
        }

        if (stabbing)
        {
            StabbingBehavior(player, toPlayer);
            AITimer++;
            return false;
        }

        if (stabbingCooldownTimer > 0)
        {
            stabbingCooldownTimer--;
        }

        return true;
    }

    void TrySetStabbing(Player player, Vector2 toPlayer)
    {
        float horizontalDistance = MathF.Abs(toPlayer.X);
        float verticalDistance = MathF.Abs(toPlayer.Y);

        bool verticalCheck = verticalDistance < NPC.height * 0.5f;
        bool horizontalCheck = horizontalDistance < 300;
        bool canHitLine = Collision.CanHitLine(NPC.Center, 2, 2, player.Center, 2, 2);

        if (verticalCheck && horizontalCheck && canHitLine)
        {
            stabbing = true;
        }
    }

    void StabbingBehavior(Player player, Vector2 toPlayer)
    {
        if (stabbingTimer < stabbingPrepDuration) // Prepare dash
        {
            if (stabbingTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-2f, -1f), Volume = 0.5f }, NPC.Center);
            }
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);
            NPC.velocity = Vector2.Zero;
        }
        else if (stabbingTimer == stabbingPrepDuration) // Dash
        {
            SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (0.3f, 0.6f), Volume = 0.75f }, NPC.Center);
            NPC.knockBackResist = 0f;
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);
            NPC.velocity = new Vector2(LemonUtils.Sign(toPlayer.X, 1) * 30, 0);
        }
        else
        {
            NPC.velocity.Y = 0;
            NPC.velocity.X *= 0.97f;
        }

        if (stabbingTimer > stabbingPrepDuration + stabbingStabDuration)
        {
            NPC.knockBackResist = 0.6f;
            stabbing = false;
            stabbingTimer = 0;
            stabbingCooldownTimer = stabbingCooldown;
        }

        stabbingTimer++;
    }

    public override void AI()
    {
        if (NPC.HasValidTarget)
        {
            Vector2 toPlayer = NPC.GetTarget().Center - NPC.Center;
            NPC.spriteDirection = -LemonUtils.Sign(toPlayer.X, 1);
        }
        if (AITimer == 0)
        {

        }

        AITimer++;
    }

    public override void FindFrame(int frameHeight)
    {
        int walkingMaxFrame = 3;
        int walkingFrameDuration = 18;
        int jumpingFrame = 3;
        int stabbingPrepFrame = 5;
        int stabbingStabFrame = 6;

        if (stabbing)
        {
            if (stabbingTimer < stabbingPrepDuration)
            {
                NPC.frame.Y = stabbingPrepFrame * frameHeight;
            }
            else
            {
                NPC.frame.Y = stabbingStabFrame * frameHeight;
            }
        }
        else
        {
            if (NPC.collideY)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > walkingFrameDuration)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0;
                    if (NPC.frame.Y > walkingMaxFrame * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.frame.Y = jumpingFrame * frameHeight;
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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (stabbing)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle sourceRect = texture.Frame(1, Main.npcFrameCount[Type], 0, NPC.frame.Y / 56);
            Vector2 drawOrigin = sourceRect.Size() * 0.5f;
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (NPC.oldPos[k] - Main.screenPosition) + new Vector2(NPC.width / 2f, NPC.height / 2f);
                Color color = (Color.Black * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length)) * 1f;
                Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, NPC.rotation, drawOrigin, NPC.scale, LemonUtils.SpriteDirectionToSpriteEffects(-NPC.spriteDirection), 0);
            }
        }
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
