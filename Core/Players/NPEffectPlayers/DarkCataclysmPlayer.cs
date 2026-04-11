using NeoParacosm.Content.Items.BossSummons;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using static NeoParacosm.Core.Systems.Data.DarkCataclysmSystem;

namespace NeoParacosm.Core.Players.NPEffectPlayers;

public partial class DarkCataclysmPlayer : ModPlayer
{
    int Timer = 0;
    public override void ResetEffects()
    {

    }

    public override void PostUpdateMiscEffects()
    {
        DCEffects();
        Timer++;
    }

    void DCEffects()
    {
        if (Player.HeldItem.type == ItemType<AncientCallingHorn>() && Player.ItemAnimationActive)
        {
            AncientCallingHornInUse = true;
            Player.NPPlayer().NoMusic = true;
        }
        if (DarkCataclysmActive)
        {
            DCEffectFogOpacity = MathHelper.Lerp(DCEffectFogOpacity, DCEffectMaxFogOpacity, 1 / 60f);
            DCEffectOpacity = MathHelper.Lerp(0, 0.4f, DCEffectOpacityTimer / 60f);
            DCEffectNoFogDistanceCurrent = MathHelper.Lerp(DCEffectNoFogDistanceCurrent, DCEffectNoFogDistance, 1 / 120f);

            if (DCEffectOpacityTimer < 60) DCEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DCEffect").GetShader();
            data.UseImage(ParacosmTextures.NoiseTexture.Value);
            data.Shader.Parameters["time"].SetValue(DCEffectFogSpeed * Timer / 100f);
            data.Shader.Parameters["fogColor"].SetValue(DCEffectFogColor.ToVector4());
            data.UseTargetPosition(DCEffectNoFogPosition);
            data.Shader.Parameters["noFogDistance"].SetValue(DCEffectNoFogDistanceCurrent);
            data.Shader.Parameters["maxFogOpacity"].SetValue(DCEffectMaxFogOpacity);
            data.Shader.Parameters["fogColorMultiplier"].SetValue(DCEffectFogColorMultiplier);
            data.Shader.Parameters["worldSize"].SetValue(new Vector2(Main.maxTilesX * 16f, Main.maxTilesY * 16f));
            data.Shader.Parameters["screenPos"].SetValue(Main.screenPosition);
            data.UseProgress(DCEffectOpacity);
            int worldWidth = Main.maxTilesX * 16;
            int worldHeight = Main.maxTilesY * 16;
            int sectionWidth = worldWidth / 16;
            int sectionHeight = worldHeight / 16;
            int sectionX = (int)MathF.Floor(Player.Center.X / sectionWidth);
            int sectionY = (int)MathF.Floor(Player.Center.Y / sectionHeight);
            int biomeDecider = (sectionX + sectionY) % 2;
            if (biomeDecider == 0)
            {
                Player.ZoneCorrupt = true;
            }
            else
            {
                Player.ZoneCrimson = true;
            }

            Lighting.GlobalBrightness = 0.9f;
            if (!SkyManager.Instance["NeoParacosm:DCSky"].IsActive())
            {
                SkyManager.Instance.Activate("NeoParacosm:DCSky");
            }

            SkyManager.Instance["NeoParacosm:DCSky"].Opacity = DCEffectOpacity;
        }
        else
        {
            DCEffectOpacity = MathHelper.Lerp(0, 0.6f, DCEffectOpacityTimer / 60f);
            SkyManager.Instance["NeoParacosm:DCSky"].Opacity = DCEffectOpacity;
            if (DCEffectOpacityTimer > 0) DCEffectOpacityTimer--;
            if (DCEffectOpacityTimer <= 0)
            {
                Filters.Scene.Deactivate("NeoParacosm:DCEffect");
                SkyManager.Instance.Deactivate("NeoParacosm:DCSky");
            }
            else
            {
                Filters.Scene["NeoParacosm:DCEffect"].GetShader().UseProgress(DCEffectOpacity);
                SkyManager.Instance["NeoParacosm:DCSky"].Opacity = DCEffectOpacity;
            }
        }
    }
}
