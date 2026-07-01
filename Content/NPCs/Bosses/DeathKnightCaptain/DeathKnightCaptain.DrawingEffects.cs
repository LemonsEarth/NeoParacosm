using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.NPCs.Bosses.DeathKnightCaptain;

// This boss is spread across multiple files
// This file contains drawing and visual/audio effect logic

public partial class DeathKnightCaptain : ModNPC
{
    int FrameWidth => 90;
    int FrameHeight => 72;

    Point CurrentFrame;

    Point StandingNormal => new Point(0, 0);
    Point Walk1Normal => new Point(0, 1);
    Point Walk2Normal => new Point(0, 2);
    Point ArmUpNormal => new Point(0, 3);
    Point ArmFrontNormal => new Point(0, 4);
    Point ArmUpNormal2 => new Point(0, 5);
    Point ArmFrontNormal2 => new Point(0, 6);

    Point Crouching1 => new Point(1, 0);
    Point Crouching2 => new Point(1, 1);
    Point Crouching3 => new Point(1, 2);
    Point ArmUpCrouching => new Point(1, 3);
    Point ArmFrontCrouching => new Point(1, 4);
    Point Dashing => new Point(1, 5);
    Point ArmFrontDashing => new Point(1, 6);

    Point Tired1 => new Point(0, 7);
    Point Tired2 => new Point(1, 7);

    void SetFrame(Point frame)
    {
        CurrentFrame = frame;
    }

    void PassiveDust()
    {
        Vector2 dustPos = NPC.Center + Vector2.UnitY.RotatedBy(NPC.rotation) * (NPC.height / 2)
            + Vector2.UnitY.RotatedBy(NPC.rotation + MathHelper.PiOver2) * Main.rand.NextFloat(-16, 16);
        Vector2 dustVel = NPC.Center.DirectionTo(dustPos);

        Dust.NewDustPerfect(dustPos, DustType<FireDust>(), dustVel, newColor: Color.Black, Scale: 0.5f).noGravity = true;
        //Dust.NewDustPerfect(dustPos, DustID.GemDiamond, Vector2.UnitY, Scale: 1.5f, newColor: Color.White).noGravity = true;
    }

    public void SpawnDust()
    {
        Dust.NewDustPerfect(NPC.RandomPos(), DustType<FireDust>(), Vector2.Zero, newColor: Color.Black, Scale: 1f).noGravity = true;
        Dust.NewDustPerfect(NPC.RandomPos(), DustType<FireDust>(), Vector2.Zero, newColor: Color.Black, Scale: 0.5f).noGravity = true;
        Dust.NewDustPerfect(NPC.RandomPos(), DustID.GemDiamond, Vector2.Zero, Scale: 2.5f, newColor: Color.White).noGravity = true;
        Dust.NewDustPerfect(NPC.RandomPos(), DustID.GemDiamond, Vector2.Zero, Scale: 1.5f, newColor: Color.White).noGravity = true;
    }

    private void TeleportEffect(int count, float speedX, float speedY)
    {
        LemonUtils.DustBurst(count, NPC.Center, DustType<FireDust>(), speedX, speedY, 0.5f, 1.2f, Color.Black);
        LemonUtils.DustBurst(count, NPC.Center, DustID.GemDiamond, speedX, speedY, 1.5f, 3.2f);
        SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (-0.2f, 0.2f) }, NPC.Center);
    }

    public void LookTowards(Vector2 point)
    {
        Vector2 dirToPoint = NPC.Center.DirectionTo(point);
        NPC.spriteDirection = LemonUtils.Sign(dirToPoint.X, 1);
        NPC.rotation = NPC.spriteDirection == 1 ? dirToPoint.ToRotation() : dirToPoint.ToRotation() + MathHelper.Pi;
    }

    public void GoInvisible()
    {
        NPC.Opacity = 0f;
        NPC.dontTakeDamage = true;
        NPC.ShowNameOnHover = false;
        TeleportEffect(8, 6, 6);
    }

    public void GoVisible()
    {
        NPC.Opacity = 1f;
        NPC.dontTakeDamage = false;
        NPC.ShowNameOnHover = true;
        TeleportEffect(8, 6, 6);
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
    }


    bool doDrawPredictiveLaser = false;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (doDrawPredictiveLaser)
        {
            Vector2 target = player.Center + new Vector2(500 * -AttackCount, player.velocity.Y * 60) * 2;
            LemonUtils.DrawLaser(NPC.Center, target, 0.7f, Color.LightYellow);
        }
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = texture.Frame(2, 8, CurrentFrame.X, CurrentFrame.Y);

        Main.EntitySpriteDraw(
            texture,
            NPC.Center - Main.screenPosition,
            sourceRect,
            Color.White * NPC.Opacity,
            NPC.rotation,
            new Vector2(FrameWidth / 2, FrameHeight / 2),
            NPC.scale,
            LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection)
            );
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
}
