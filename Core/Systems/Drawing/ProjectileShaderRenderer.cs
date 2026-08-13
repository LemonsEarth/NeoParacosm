using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Players;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Core.Systems.Drawing;

public class ProjectileShaderRenderer : ModSystem
{
    public Dictionary<MiscShaderData, List<IShaderProjectile>> ProjectilesByEffect { get; private set; } = new();
    public Dictionary<MiscShaderData, ProjectileRenderState> RenderStatesByEffect { get; private set; } = new();

    public static ProjectileShaderRenderer Instance => GetInstance<ProjectileShaderRenderer>();

    public static MiscShaderData GetMiscShader(string name)
    {
        return GameShaders.Misc[$"NeoParacosm:{name}"];
    }

    void LoadMiscShader(string name, string path, BlendState blendState)
    {
        Asset<Effect> shader = Mod.Assets.Request<Effect>(path);
        string key = $"NeoParacosm:{name}";
        GameShaders.Misc[key] = new MiscShaderData(shader, name);
        ProjectilesByEffect.Add(GameShaders.Misc[key], new List<IShaderProjectile>());

        ProjectileRenderState projRenderState = new(GameShaders.Misc[key], blendState);
        RenderStatesByEffect.Add(GameShaders.Misc[key], projRenderState);
    }

    public override void Load()
    {
        LoadMiscShader("ShieldPulseShader", "Common/Assets/Shaders/Projectiles/ShieldPulseShader", BlendState.Additive);
        LoadMiscShader("GasShader", "Common/Assets/Shaders/Projectiles/GasShader", BlendState.Additive);
        LoadMiscShader("GravityForceShader", "Common/Assets/Shaders/Projectiles/GravityForceShader", BlendState.Additive);
        LoadMiscShader("FireShader", "Common/Assets/Shaders/Projectiles/FireShader", BlendState.AlphaBlend);
        LoadMiscShader("LaserShader", "Common/Assets/Shaders/Projectiles/LaserShader", BlendState.AlphaBlend);
        LoadMiscShader("DreadlordLaserShader", "Common/Assets/Shaders/Projectiles/DreadlordLaserShader", BlendState.AlphaBlend);
        LoadMiscShader("LightningShader", "Common/Assets/Shaders/Projectiles/LightningShader", BlendState.AlphaBlend);
        LoadMiscShader("BigLightningShader", "Common/Assets/Shaders/Projectiles/BigLightningShader", BlendState.AlphaBlend);
        LoadMiscShader("SphereShader", "Common/Assets/Shaders/Projectiles/SphereShader", BlendState.AlphaBlend);
        LoadMiscShader("AscendedWeaponGlow", "Common/Assets/Shaders/Items/AscendedWeaponGlow", BlendState.Additive);
        LoadMiscShader("DeathbirdWingShader", "Common/Assets/Shaders/NPCs/DeathbirdWingShader", BlendState.Additive);

        On_Main.DrawProjectiles += On_Main_DrawProjectiles;
    }

    private void DrawProjectilesByEffect()
    {
        foreach (var effect in ProjectilesByEffect.Keys)
        {
            if (ProjectilesByEffect[effect].Count == 0) continue;
            ProjectileRenderState renderState = RenderStatesByEffect[effect];

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, renderState.blendState, Main.DefaultSamplerState, default, Main.Rasterizer, effect.Shader, Main.GameViewMatrix.TransformationMatrix);
            effect.Apply();
            foreach (var proj in ProjectilesByEffect[effect])
            {
                proj.DrawProjectile();
            }
            Main.spriteBatch.End();
        }

        foreach (var effect in ProjectilesByEffect.Keys)
        {
            ProjectilesByEffect[effect].Clear();
        }
    }

    private void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
    {
        DrawProjectilesByEffect();
        orig(self);
    }

    public void Queue(ModProjectile projectile)
    {
        if (projectile is IShaderProjectile shaderProjectile)
        {
            MiscShaderData effect = shaderProjectile.ShaderData;
            ProjectilesByEffect[effect].Add(shaderProjectile);
        }
        else
        {
            Mod.Logger.Warn($"Attempted to queue projectile {projectile.Name} into ProjectileShaderRenderer, but it does not implement IShaderProjectile!");
        }
    }
}

public struct ProjectileRenderState
{
    public MiscShaderData effect;
    public BlendState blendState;

    public ProjectileRenderState(MiscShaderData effect, BlendState blendState)
    {
        this.effect = effect;
        this.blendState = blendState;
    }
}
