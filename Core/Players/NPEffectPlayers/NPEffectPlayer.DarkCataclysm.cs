using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.BossSummons;
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
    float DCEffectOpacity = 0f;
    float DCEffectOpacityTimer = 0f;
    public Color DCEffectFogColor = Color.White;
    public Vector2 DCEffectNoFogPosition = Vector2.Zero;
    public float DCEffectNoFogDistance = 0;
    public float DCEffectNoFogDistanceCurrent = 0;
    public float DCEffectMaxFogOpacity = 0.1f;
    public float DCEffectFogOpacity = 0f;
    public bool ShouldReset => !(Player.HeldItem.type == ItemType<AncientCallingHorn>() && Player.ItemAnimationActive);
    public override void ResetEffects()
    {
       
    }

    Vector2 moveT = Vector2.Zero;
    void DCEffects()
    {
        if (ShouldReset)
        {
            DCEffectFogColor = Color.White;
            DCEffectMaxFogOpacity = 0.1f;
            DCEffectNoFogDistance = 0;
        }

        if (ResearcherQuest.DarkCataclysmActive)
        {
            DCEffectFogOpacity = MathHelper.Lerp(DCEffectFogOpacity, DCEffectMaxFogOpacity, 1 / 60f);
            DCEffectOpacity = MathHelper.Lerp(0, 1f, DCEffectOpacityTimer / 60f);
            DCEffectNoFogDistanceCurrent = MathHelper.Lerp(DCEffectNoFogDistanceCurrent, DCEffectNoFogDistance, 1 / 120f);

            if (DCEffectOpacityTimer < 60) DCEffectOpacityTimer++;
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DCEffect").GetShader();
            data.UseImage(ParacosmTextures.NoiseTexture.Value);
            data.Shader.Parameters["time"].SetValue(Timer / 100f);
            data.Shader.Parameters["fogColor"].SetValue(DCEffectFogColor.ToVector4());
            data.UseTargetPosition(DCEffectNoFogPosition);
            data.Shader.Parameters["noFogDistance"].SetValue(DCEffectNoFogDistanceCurrent);
            data.Shader.Parameters["maxFogOpacity"].SetValue(DCEffectFogOpacity);

            Vector2 pVClamped = Vector2.Clamp(Player.velocity, -Vector2.One * 10, Vector2.One * 10);
            moveT = Vector2.Lerp(moveT, pVClamped, 1f / 60f);
            data.Shader.Parameters["playerVelocity"].SetValue(Player.velocity);
            data.Shader.Parameters["moveT"].SetValue(moveT);
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
            DCEffectOpacity = MathHelper.Lerp(0, maxDesaturateValue, DCEffectOpacityTimer / 60f);
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
