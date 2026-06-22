using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.DeathKnightCaptain;

// This boss is spread across multiple files
// This file contains drawing and visual/audio effect logic

public partial class DeathKnightCaptain : ModNPC
{
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

        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

    }
}
