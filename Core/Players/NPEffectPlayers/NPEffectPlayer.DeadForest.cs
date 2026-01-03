using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Core.Players.NPEffectPlayers;

public partial class NPEffectPlayer : ModPlayer
{
    float desaturateEffectOpacity = 0f;
    float desaturateEffectOpacityTimer = 0f;
    float maxDesaturateValue = 0.6f;

    void DeadForestEffects()
    {
        if (Player.InModBiome<DeadForestBiome>())
        {
            desaturateEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, desaturateEffectOpacityTimer / 60f);
            if (desaturateEffectOpacityTimer < 60) desaturateEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DesaturateShader").GetShader();
            data.UseProgress(desaturateEffectOpacity);

            Player.AddBuff(BuffType<DeadForestDebuff>(), 2);
        }
        else
        {
            desaturateEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, desaturateEffectOpacityTimer / 60f);
            if (desaturateEffectOpacityTimer > 0) desaturateEffectOpacityTimer--;
            if (desaturateEffectOpacityTimer <= 0)
            {
                Filters.Scene.Deactivate("NeoParacosm:DesaturateShader");
            }
            else
            {
                Filters.Scene["NeoParacosm:DesaturateShader"].GetShader().UseProgress(desaturateEffectOpacity);
            }
        }
    }
}
