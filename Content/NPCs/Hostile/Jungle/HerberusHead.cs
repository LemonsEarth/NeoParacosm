using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Jungle;
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
        NPC.DontDropAnything();
        NPC.HideFromBestiary();
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
        NPC.ShowNameOnHover = false;
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
        //NPC.Center = Vector2.Lerp(NPC.Center, GetDefaultPosition(), 1 / 4f);
        NPC.MoveToPos(GetDefaultPosition(), 0.5f, 0.4f, 0.3f, 0.4f);
        if (AITimer % (90 + HeadNum * 30) == 0)
        {
            NPC.velocity.Y -= 8;
        }
        NPC.target = bodyNPC.target;
        if (!NPC.HasValidTarget)
        {
            NPC.rotation = 0f;
            AITimer++;
            return;
        }
        Player player = NPC.GetTarget();
        Vector2 toPlayer = NPC.Center.DirectionTo(player.Center);
        float targetRot = toPlayer.ToRotation() + MathHelper.Pi;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 1 / 20f);

        if (Mode == 0)
        {
            EyeAttackMode(player, toPlayer);
        }
        else
        {
            MouthAttackMode(player, toPlayer);
        }

        AITimer++;
    }

    void EyeAttackMode(Player player, Vector2 toPlayer)
    {
        if (bodyNPC.GetLifePercent() > 0.6f)
        {
            float range = (300 + LemonUtils.GetDifficulty() * 150);
            bool rangeCond = NPC.DistanceSQ(player.Center) < range * range;
            if (rangeCond && AITimer % (120 + HeadNum * 15) == 0)
            {
                SoundEngine.PlaySound(SoundID.Item46 with { PitchRange = (0.4f, 0.6f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (0.2f, 0.4f), Volume = 0.5f }, NPC.Center);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        toPlayer * Main.rand.NextFloat(7, 10),
                        ProjectileType<HerberusSpore>(),
                        ai0: NPC.target
                        );
                }
            }
        }
        else
        {
            float range = (350 + LemonUtils.GetDifficulty() * 200);
            bool rangeCond = NPC.DistanceSQ(player.Center) < range * range;
            if (rangeCond && AITimer % (60 + HeadNum * 15) == 0)
            {
                SoundEngine.PlaySound(SoundID.Item46 with { PitchRange = (0.4f, 0.6f) }, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (0.1f, 0.3f) }, NPC.Center);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        toPlayer * Main.rand.NextFloat(9, 12),
                        ProjectileType<HerberusSporeFire>(),
                        ai0: NPC.target
                        );
                }
            }
        }
    }

    void MouthAttackMode(Player player, Vector2 toPlayer)
    {
        if (bodyNPC.GetLifePercent() > 0.6f)
        {
            float range = (300 + LemonUtils.GetDifficulty() * 150);
            bool rangeCond = NPC.DistanceSQ(player.Center) < range * range;
            if (rangeCond && AITimer % (180 + HeadNum * 30) == 0)
            {
                for (int i = -4; i <= 4; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (-0.5f, -0.1f) }, NPC.Center);
                    SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (0.2f, 0.4f), Volume = 0.5f }, NPC.Center);
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        toPlayer.RotatedBy(i * MathHelper.Pi / 8f) * Main.rand.NextFloat(30, 40),
                        ProjectileType<HerberusSporeBig>(),
                        ai0: 10
                        );
                    }
                }
            }
        }
        else
        {
            float range = (350 + LemonUtils.GetDifficulty() * 150);
            bool rangeCond = NPC.DistanceSQ(player.Center) < range * range;
            if (rangeCond && AITimer % (150 + HeadNum * 30) == 0)
            {
                for (int i = -4; i <= 4; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (-0.5f, -0.1f) }, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { PitchRange = (-0.2f, 0.1f), Volume = 0.7f, MaxInstances = 3 }, NPC.Center);
                    //SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (-0.4f, -0.2f), Volume = 0.5f }, NPC.Center);
                    if (LemonUtils.NotClient())
                    {
                        LemonUtils.QuickProj(
                        NPC,
                        NPC.Center,
                        toPlayer.RotatedBy(i * MathHelper.Pi / 8f) * Main.rand.NextFloat(40, 50),
                        ProjectileType<HerberusSporeBigFire>(),
                        ai0: 30
                        );
                    }
                }
            }
        }
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


