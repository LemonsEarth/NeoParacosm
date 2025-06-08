using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using Terraria.Graphics.Effects;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    public override void ResetEffects()
    {
        
    }

    public override void PostUpdate()
    {
        //LemonUtils.DebugPlayerCenter(Player);
        //Main.NewText("World Surface: " + (int)Main.worldSurface);
        
    }

    public override void PostUpdateMiscEffects()
    {
        if (Player.InModBiome<DepthsHigh>())
        {
            Filters.Scene.Activate("NeoParacosm:ScreenTintShader").GetShader().UseColor(new Color(102, 148, 255));
            Filters.Scene["NeoParacosm:ScreenTintShader"].GetShader().UseProgress(1);
        }
        else
        {
            Filters.Scene.Deactivate("NeoParacosm:ScreenTintShader");
        }
    }
}
