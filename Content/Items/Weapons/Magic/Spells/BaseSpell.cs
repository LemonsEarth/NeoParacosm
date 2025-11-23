using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells;

public abstract class BaseSpell : ModItem
{
    /// <summary>
    /// Cooldown applied when spell is cast
    /// </summary>
    public abstract int AttackCooldown { get; }

    /// <summary>
    /// Mana cost for casting spell
    /// </summary>
    public abstract int ManaCost { get; }

    /// <summary>
    /// Vector that controls in what direction the catalyst will be pointing
    /// </summary>
    public abstract Vector2 TargetVector { get; }
    public abstract void ShootBehaviour(Player player);

    public enum SpellElement
    {
        None,
        Fire,
        Magic,
        Ice,
        Earth,
        Lightning,
        Holy,
        Dark
    }

    public HashSet<SpellElement> SpellElements = [];

    public virtual int GetDamage(Player player)
    {
        int itemDamage = Item.damage;
        int finalItemDamage = itemDamage;

        foreach (SpellElement element in SpellElements)
        {
            finalItemDamage += (int)(itemDamage * player.NPCatalystPlayer().ElementalDamageBoosts.GetValueOrDefault(element, 0));
        }

        return player.HeldItem.damage + finalItemDamage;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Default;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.noMelee = true;
        Item.useTime = 30;
        Item.useAnimation = 30;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            player.NPCatalystPlayer().RemoveSpell(this);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.5f, PitchRange = (-0.5f, -0.35f) }, player.Center);
        }
        else
        {
            player.NPCatalystPlayer().AddSpell(this);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.5f, PitchRange = (0.35f, 0.5f) }, player.Center);
        }
        return true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine spellLine = new TooltipLine(Mod, "NeoParacosm:SpellDescription", Language.GetTextValue("Mods.NeoParacosm.Items.SpellTemplate.Description"));
        tooltips.Add(spellLine);

        string elements = Language.GetTextValue("Mods.NeoParacosm.Items.SpellTemplate.Elements");
        foreach (var element in SpellElements)
        {
            elements += $"{Enum.GetName(element)} - ";
        }
        TooltipLine elementsLine = new TooltipLine(Mod, "NeoParacosm:SpellElements", elements);
        tooltips.Add(elementsLine);
    }
}