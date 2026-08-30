using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.Grimstagg;

// This boss is spread across multiple files
// This file contains drawing and visual/audio effect logic

public partial class Grimstagg : ModNPC
{
    void PlayRoar(float bonusPitch = 0f)
    {
        SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f + bonusPitch, MaxInstances = 5 }, NPC.Center);
        SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -1f + bonusPitch, MaxInstances = 5 }, NPC.Center);
        SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { Pitch = bonusPitch, Volume = 0.5f, MaxInstances = 5 }, NPC.Center);
    }

    void PlayRoarAtPlayer(float bonusPitch = 0f)
    {
        SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f + bonusPitch, MaxInstances = 5 });
        SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -1f + bonusPitch, MaxInstances = 5 });
        SoundEngine.PlaySound(ParacosmSFX.DragonRoar with { Pitch = bonusPitch, Volume = 0.5f, MaxInstances = 5 }, NPC.Center);
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsMoonMoon.Add(index);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
        if (NPC.life <= 0)
        {
            /*LemonUtils.DustBurst(20, NPC.Center, DustID.GemTopaz, 10, 10, 1.5f, 3.5f);
            LemonUtils.DustBurst(20, NPC.Center, DustType<FireDust>(), 20, 20, 1f, 2f, Color.Lime);
            LemonUtils.DustBurst(20, NPC.Center, DustType<FireDust>(), 20, 20, 1f, 2f, Color.Gold);
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickPulse(NPC, NPC.Center, 4, 30, 5, Color.Gold);
                LemonUtils.QuickPulse(NPC, NPC.Center, 3, 30, 5, Color.Gold);
                LemonUtils.QuickPulse(NPC, NPC.Center, 2, 30, 5, Color.Gold);
            }*/
        }
    }
    void LerpScale(float targetScale, float time)
    {
        // rounding stuff
        if (targetScale == 1f && MathF.Abs(targetScale - NPC.scale) < 0.02f)
        {
            NPC.scale = 1f;
        }
        NPC.scale = MathHelper.Lerp(NPC.scale, targetScale, time);
    }

    #region Drawing

    enum BodyFrame
    {
        Left = 0, Front = 1, Right = 2
    }
    BodyFrame CurrentBodyFrame { get; set; } = BodyFrame.Left;
    void DrawBody(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        Rectangle frame = BodyTex.Frame(1, 3, 0, (int)CurrentBodyFrame);
        sb.Draw(
            BodyTex.Value,
            NPC.Center - screenPos,
            frame,
            drawColor,
            NPC.rotation,
            frame.Size() * 0.5f,
            NPC.scale,
            LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection),
            0
            );
    }

    enum HeadFrame
    {
        Left = 0, Front = 1, Right = 2
    }
    HeadFrame CurrentHeadFrame { get; set; } = HeadFrame.Left;
    void DrawHead(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        Rectangle frame = HeadTex.Frame(1, 3, 0, (int)CurrentHeadFrame);
        Vector2 headOffset = new Vector2(-NPC.spriteDirection * 45, -45).RotatedBy(NPC.rotation);
        Vector2 headPos = NPC.Center + headOffset;
        sb.Draw(
            HeadTex.Value,
            headPos - screenPos,
            frame,
            drawColor,
            headPos.DirectionTo(headLookAtPos).ToRotation(),
            frame.Size() * 0.5f,
            NPC.scale,
            LemonUtils.SpriteDirectionToSpriteEffects(NPC.spriteDirection),
            0
            );
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBody(spriteBatch, screenPos, drawColor);
        DrawHead(spriteBatch, screenPos, drawColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
    #endregion
}
