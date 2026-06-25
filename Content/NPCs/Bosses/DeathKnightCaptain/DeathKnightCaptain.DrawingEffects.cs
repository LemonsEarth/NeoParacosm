using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

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

    void SetFrame(Point frame)
    {
        CurrentFrame = frame;
    }

    private void AuraBurst(int count, Vector2 speed)
    {
        for (int i = 0; i < count; i++)
        {
            Dust.NewDustDirect(NPC.RandomPos(200, 100), 2, 2, DustID.GemTopaz, speed.X, speed.Y, Scale: Main.rand.NextFloat(2f, 3f)).noGravity = true;
        }
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = texture.Frame(2, 7, CurrentFrame.X, CurrentFrame.Y);

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
