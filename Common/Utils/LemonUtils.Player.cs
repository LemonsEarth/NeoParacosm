using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;

namespace NeoParacosm.Common.Utils;

public static partial class LemonUtils
{
    /// <summary>
    /// Checks if the player is not null, active, alive etc.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool IsAlive(this Player player)
    {
        return (player != null || player.active || !player.dead || !player.ghost);
    }

    public static NPCatalystPlayer NPCatalystPlayer(this Player player)
    {
        return player.GetModPlayer<NPCatalystPlayer>();
    }

    public static NPPlayer NPPlayer(this Player player)
    {
        return player.GetModPlayer<NPPlayer>();
    }

    /// <summary>
    /// Add to the player's elemental damage boost for the given element.
    /// Adding 0.2f would make the boost 1.2f.
    /// </summary>
    public static void AddElementalDamageBoost(this Player player, SpellElement element, float value)
    {
        player.NPCatalystPlayer().ElementalDamageBoosts[element] += value;
    }

    /// <summary>
    /// Add to the player's elemental expertise boost for the given element.
    /// Adding 0.2f would make the boost 1.2f.
    /// </summary>
    public static void AddElementalExpertiseBoost(this Player player, SpellElement element, float value)
    {
        player.NPCatalystPlayer().ElementalExpertiseBoosts[element] += value;
    }

    /// <summary>
    /// Returns the player's elemental damage boost for the given element.
    /// 1f means 100%.
    /// </summary>
    public static float GetElementalDamageBoost(this Player player, SpellElement element)
    {
        return player.NPCatalystPlayer().ElementalDamageBoosts[element];
    }

    /// <summary>
    /// Returns the player's elemental expertise boost for the given element.
    /// 1f means 100%.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static float GetElementalExpertiseBoost(this Player player, SpellElement element)
    {
        return player.NPCatalystPlayer().ElementalExpertiseBoosts[element];
    }

    /// <summary>
    /// Returns the player's elemental expertise boost for the given element, with the decimal portion of the boost multiplied by mul.
    /// So for mul = 2 and expertise = 1.22, this would return 1.44.
    /// Useful for making stats scale better with expertise without actually changing the expertise itself.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="element"></param>
    /// <param name="mul"></param>
    /// <returns></returns>
    public static float GetElementalExpertiseBoostMultiplied(this Player player, SpellElement element, float mul)
    {
        float dec = MathF.Max(player.NPCatalystPlayer().ElementalExpertiseBoosts[element] - 1, 0);
        dec *= mul;
        return 1 + dec;
    }

    /// <summary>
    /// Get player life as a float 0f-1f, where 1f means full hp.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static float GetLifePercent(this Player player)
    {
        return (float)player.statLife / player.statLifeMax2;
    }

    public static bool HasAnyFireDebuff(this Player player)
    {
        return player.HasBuff(BuffID.OnFire) || player.HasBuff(BuffID.Burning) || player.HasBuff(BuffID.OnFire3)
            || player.HasBuff(BuffID.Frostburn) || player.HasBuff(BuffID.Frostburn2) || player.HasBuff(BuffID.ShadowFlame);
    }

    public static bool HasAnyPoisonDebuff(this Player player)
    {
        return player.HasBuff(BuffID.Poisoned) || player.HasBuff(BuffID.Venom);
    }

    /// <summary>
    /// Gets random position on the player's hitbox.
    /// Increase fluffX and fluffY to expand the range.
    /// Useful for spawning dusts on the player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="fluffX"></param>
    /// <param name="fluffY"></param>
    /// <returns></returns>
    public static Vector2 RandomPos(this Player player, float fluffX = 0, float fluffY = 0)
    {
        Vector2 pos = player.position + new Vector2(Main.rand.NextFloat(-fluffX, player.width + fluffX), Main.rand.NextFloat(-fluffY, player.height + fluffY));
        return pos;
    }

    /// <summary>
    /// Checks if the player is grounded.
    /// Might not be fully reliable.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool IsGrounded(this Player player)
    {
        Tile tileBelow = Main.tile[(int)(player.Center.X / 16), (int)(player.Center.Y / 16) + 2];
        return (Main.tileSolid[tileBelow.TileType] || Main.tileSolid[tileBelow.TileType]) && player.velocity.Y == 0;
    }

    /// <summary>
    /// Restores mana with respect to cap.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="value"></param>
    public static void RestoreMana(this Player player, int value)
    {
        if (player.statMana + value > player.statManaMax2)
        {
            value = player.statManaMax2 - player.statMana;
        }
        player.statMana += value;
        player.ManaEffect(value);
    }

    /// <summary>
    /// Basic damage over time debuff effect.
    /// Use in ModPlayer.UpdateBadLifeRegen().
    /// </summary>
    /// <param name="player"></param>
    /// <param name="damagePerSecond"></param>
    public static void DOTDebuff(this Player player, int damagePerSecond)
    {
        if (player.lifeRegen > 0) player.lifeRegen = 0;
        player.lifeRegenTime = 0;
        player.lifeRegen -= damagePerSecond * 2;
    }
}
