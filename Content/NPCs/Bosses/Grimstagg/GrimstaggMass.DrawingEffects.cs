using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.Grimstagg;

// This boss is spread across multiple files
// This file contains drawing and visual/audio effect logic

public partial class GrimstaggMass : ModNPC
{
    Asset<Texture2D> Tex => TextureAssets.Npc[Type];
    const int MAX_FRAMES = 8;
    float BallWidth => 80f;

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

    #region Drawing

    Vector2 GetBallPosition(int i, int j)
    {
        int iSign = LemonUtils.Sign(i, 1);
        Vector2 startPos = MassTarget + Vector2.UnitY * 64;
        Vector2 direction = startPos.DirectionTo(NPC.Center).RotatedBy(i * (Pi / 24f));
        //direction = direction.RotatedBy(Pi / 8f * -NPC.spriteDirection); // offsetting

        float length = (j + 0.25f) * (BallWidth * 0.5f);

        Vector2 bezierPointA = startPos;
        Vector2 bezierPointB = bezierPointA + direction * length;

        Vector2 bezierControlPoint = bezierPointA + direction * length * 0.8f;

        float normalLength = 150 * MathF.Min(RowCount / 15f, 2f);
        Vector2 controlPointNormal = direction.RotatedBy(-iSign * PiOver2) * normalLength;
        bezierControlPoint += controlPointNormal;

        float bezierT = j / 14f;
        Vector2 drawPos = LemonUtils.BezierCurve(bezierPointA, bezierPointB, bezierControlPoint, bezierT);
        /*if (i == -7)
        {
            Dust.NewDustPerfect(drawPos, DustID.GemDiamond, Vector2.Zero).noGravity = true;
        }*/
        drawPos += Main.rand.NextVector2Circular((j + 1) * 0.5f, (j + 1) * 0.5f);
        return drawPos;
    }

    int ColumnCount => 14;
    int RowCount => (int)((NPC.Distance(MassTarget) / BallWidth) * 3f);

    void DrawBalls(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        int columnCount = ColumnCount;
        int rowCount = RowCount;
        for (int i = -columnCount / 2; i <= columnCount / 2; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                Vector2 drawPos = GetBallPosition(i, j);
                int frameValue = (int)MathF.Abs((i + j) % MAX_FRAMES);
                Rectangle frame = Tex.Frame(1, MAX_FRAMES, 0, frameValue);
                Color color = Color.Lerp(Color.White, Color.Black, j / (rowCount - 9f));
                float rotationDeg = (i * j * j) % 360;
                float rotation = ToRadians(rotationDeg);
                float scaleMul = 1 + (j / 30f);
                sb.Draw(
                    Tex.Value,
                    drawPos - screenPos,
                    frame,
                    color,
                    rotationDeg,
                    frame.Size() * 0.5f,
                    NPC.scale * scaleMul,
                    SpriteEffects.None,
                    0
                    );
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBalls(spriteBatch, screenPos, drawColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
    #endregion
}
