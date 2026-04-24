using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.Localization;

namespace NeoParacosm.Common.Utils;

/// <summary>
/// Contains a lot of utillities and global usings
/// </summary>
public static partial class LemonUtils
{
    public static void DrawAscendedWeaponGlowInWorld(Item item, int originalItemID, float rotation, float scale, SpriteBatch spriteBatch, Color color)
    {
        Main.instance.LoadItem(originalItemID);
        Texture2D origTexture = TextureAssets.Item[originalItemID].Value;
        Texture2D glowTexture = TextureAssets.Item[item.type].Value;
        Vector2 drawPos = item.Center - Main.screenPosition;
        spriteBatch.Draw(origTexture, drawPos, null, Color.White, rotation, origTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        spriteBatch.Draw(glowTexture, drawPos, null, Color.White, rotation, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public static void DrawAscendedWeaponGlowInInventory(Item item, int originalItemID, Vector2 position, float scale, Rectangle frame, SpriteBatch spriteBatch, Color color)
    {
        Main.instance.LoadItem(originalItemID);
        Texture2D origTexture = TextureAssets.Item[originalItemID].Value;
        Texture2D glowTexture = TextureAssets.Item[item.type].Value;
        spriteBatch.Draw(origTexture, position, null, Color.White, 0f, origTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["color"].SetValue(color.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, shader.Shader, Main.UIScaleMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        spriteBatch.Draw(glowTexture, position, null, Color.White, 0f, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
    }

    public static string GetSpellBonusTooltip(SpellElement element, SpellBoostType boostType)
    {
        return Language.GetTextValue($"Mods.NeoParacosm.Items.SpellBonus.{boostType}.{element}");
    }

    public static TooltipLine QuickArmorSpellBoostTooltipLine(string itemName, SpellElement element, SpellBoostType boostType)
    {
        return new TooltipLine(NeoParacosm.Instance, $"NeoParacosm:{itemName}SpellBoost", GetSpellBonusTooltip(element, boostType));
    }

    public static string GetLocKey(this Item item)
    {
        return $"Mods.NeoParacosm.NPCs.{item.ModItem.Name}";
    }
}
