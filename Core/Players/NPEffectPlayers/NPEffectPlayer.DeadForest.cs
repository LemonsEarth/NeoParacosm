using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Core.Systems;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Runtime.InteropServices;
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
