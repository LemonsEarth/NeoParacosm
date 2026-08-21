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
        spriteBatch.Draw(glowTexture, drawPos, null, color, rotation, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(origTexture, drawPos, null, Color.White, rotation, origTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
    }

    public static void DrawAscendedWeaponGlowInInventory(Item item, int originalItemID, Vector2 position, float scale, Rectangle frame, SpriteBatch spriteBatch, Color color)
    {
        Main.instance.LoadItem(originalItemID);
        Texture2D origTexture = TextureAssets.Item[originalItemID].Value;
        Texture2D glowTexture = TextureAssets.Item[item.type].Value;
        spriteBatch.Draw(glowTexture, position, null, color, 0f, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(origTexture, position, null, Color.White, 0f, origTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
    }

    public static void DrawDreadlordWeaponGlowInInventory(int itemType, Asset<Texture2D> glowTexture, Vector2 position, float scale, SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAssets.Item[itemType].Value;
        float colorT = (MathF.Sin((float)Main.timeForVisualEffects / 20f) + 1) * 0.5f;
        Color glowColor = Color.Lerp(Color.Gold, Color.Purple, colorT);
        spriteBatch.Draw(glowTexture.Value, position, null, glowColor, 0f, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, position, null, Color.White, 0f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);
    }

    public static void DrawDreadlordWeaponGlowInWorld(Item item, Asset<Texture2D> glowTexture, float rotation, float scale, SpriteBatch spriteBatch)
    {
        float colorT = (MathF.Sin((float)Main.timeForVisualEffects / 20f) + 1) * 0.5f;
        Color glowColor = Color.Lerp(Color.Gold, Color.Purple, colorT);
        Texture2D origTexture = TextureAssets.Item[item.type].Value;
        Vector2 drawPos = item.Center - Main.screenPosition;
        spriteBatch.Draw(glowTexture.Value, drawPos, null, glowColor, 0f, glowTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(origTexture, drawPos, null, Color.White, 0f, origTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
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
