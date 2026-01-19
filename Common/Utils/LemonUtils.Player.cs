using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;

namespace NeoParacosm.Common.Utils;

public static partial class LemonUtils
{
    public static bool Alive(this Player player)
    {
        return (player != null || player.active || !player.dead || !player.ghost);
    }

    public static NPCatalystPlayer NPCatalystPlayer(this Player player)
    {
        return player.GetModPlayer<NPCatalystPlayer>();
    }

    public static NPAcessoryPlayer NPAccessoryPlayer(this Player player)
    {
        return player.GetModPlayer<NPAcessoryPlayer>();
    }

    public static NPPlayer NPPlayer(this Player player)
    {
        return player.GetModPlayer<NPPlayer>();
    }

    public static NPBuffPlayer NPBuffPlayer(this Player player)
    {
        return player.GetModPlayer<NPBuffPlayer>();
    }

    public static NPArmorPlayer NPArmorPlayer(this Player player)
    {
        return player.GetModPlayer<NPArmorPlayer>();
    }

    public static void AddElementalDamageBoost(this Player player, BaseSpell.SpellElement element, float value)
    {
        player.NPCatalystPlayer().ElementalDamageBoosts[element] += value;
    }

    public static void AddElementalExpertiseBoost(this Player player, BaseSpell.SpellElement element, float value)
    {
        player.NPCatalystPlayer().ElementalExpertiseBoosts[element] += value;
    }

    public static float GetElementalDamageBoost(this Player player, BaseSpell.SpellElement element)
    {
        return player.NPCatalystPlayer().ElementalDamageBoosts[element];
    }

    public static float GetElementalExpertiseBoost(this Player player, BaseSpell.SpellElement element)
    {
        return player.NPCatalystPlayer().ElementalExpertiseBoosts[element];
    }

    public static float GetElementalExpertiseBoostMultiplied(this Player player, BaseSpell.SpellElement element, float mul)
    {
        float dec = MathF.Max(player.NPCatalystPlayer().ElementalExpertiseBoosts[element] - 1, 0);
        dec *= 2;
        return 1 + dec;
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

    public static Vector2 RandomPos(this Player player, float fluffX = 0, float fluffY = 0)
    {
        Vector2 pos = player.position + new Vector2(Main.rand.NextFloat(-fluffX, player.width + fluffX), Main.rand.NextFloat(-fluffY, player.height + fluffY));
        return pos;
    }

    public static bool IsGrounded(this Player player)
    {
        Tile tileBelow = Main.tile[(int)(player.Center.X / 16), (int)(player.Center.Y / 16) + 2];
        return (Main.tileSolid[tileBelow.TileType] || Main.tileSolid[tileBelow.TileType]) && player.velocity.Y == 0;
    }

    public static void RestoreMana(this Player player, int value)
    {
        if (player.statMana + value > player.statManaMax2)
        {
            value = player.statManaMax2 - player.statMana;     
        }
        player.statMana += value;
        player.ManaEffect(value);
    }
}
