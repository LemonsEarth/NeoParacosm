global using Microsoft.Xna.Framework;
global using System;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static void DrawAscendedWeaponGlowInWorld(Item item, float rotation, float scale, int timer, SpriteBatch spriteBatch, Color color)
    {
        Texture2D texture = TextureAssets.Item[item.type].Value;
        Vector2 drawPos = item.Center - Main.screenPosition;
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(timer);
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        spriteBatch.Draw(texture, drawPos, null, Color.White, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public static void DrawAscendedWeaponGlowInInventory(Item item, Vector2 position, float scale, int timer, Rectangle frame, SpriteBatch spriteBatch, Color color)
    {
        Texture2D texture = TextureAssets.Item[item.type].Value;
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(timer);
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.UIScaleMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        spriteBatch.Draw(texture, position, frame, Color.White, 0f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.UIScaleMatrix);
    }
}
