using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.EffectProjectiles;
using NeoParacosm.Core.Systems.Misc;
using NeoParacosm.Core.UI.Spells;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
namespace NeoParacosm.Core.Players;

public class NPCatalystPlayer : ModPlayer
{
    /// <summary>
    /// Damage boosts for each spell element type.
    /// Each is set to 1f at the start of every Update.
    /// Value is a percentage, so 1f would be a 100% damage increase.
    /// </summary>
    public Dictionary<SpellElement, float> ElementalDamageBoosts = new Dictionary<SpellElement, float>();

    /// <summary>
    /// Speed, Projectile amount, knockback and other misc boosts for each spell element type.
    /// Expertise affects each spell differently, if at all.
    /// Each is set to 1 at the start of every Update.
    /// Value is a percentage, so 2f could be a 100% speed increase.
    /// </summary>
    public Dictionary<SpellElement, float> ElementalExpertiseBoosts = new Dictionary<SpellElement, float>();

    public Dictionary<int, bool> CatalystBoostActive = new Dictionary<int, bool>();

    public const int BASE_SPELL_SLOTS = 3;
    public int maxSpellSlots = 3;

    public override void Initialize()
    {
        maxSpellSlots = BASE_SPELL_SLOTS;
        EquippedSpells = new BaseSpell[3];
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(maxSpellSlots)] = maxSpellSlots;
        Item[] equippedSpellsItems = new Item[EquippedSpells.Length];
        for (int i = 0; i < equippedSpellsItems.Length; i++)
        {
            equippedSpellsItems[i] = EquippedSpells[i] == null ? new Item() : EquippedSpells[i].Item;
        }
        tag[nameof(EquippedSpells)] = equippedSpellsItems;
    }

    public override void LoadData(TagCompound tag)
    {
        maxSpellSlots = tag.GetInt(nameof(maxSpellSlots));
        Item[] equippedSpellsItems = tag.Get<Item[]>(nameof(EquippedSpells));
        for (int i = 0; i < equippedSpellsItems.Length; i++)
        {
            EquippedSpells[i] = equippedSpellsItems[i].ModItem as BaseSpell;
        }
    }

    public BaseSpell[] EquippedSpells { get; private set; } = new BaseSpell[3];
    public BaseSpell SelectedSpell => EquippedSpells[SelectedSpellIndex];
    public int SelectedSpellIndex { get; private set; } = 0;

    public void SetSpell(int index, BaseSpell spell)
    {
        EquippedSpells[index] = spell;
    }

    public void CycleSpells(int direction = 1)
    {
        if (SelectedSpellIndex == -1)
        {
            return;
        }

        do
        {
            SelectedSpellIndex += direction;
            if (SelectedSpellIndex >= EquippedSpells.Length)
            {
                SelectedSpellIndex = 0;
            }
            else if (SelectedSpellIndex < 0)
            {
                SelectedSpellIndex = EquippedSpells.Length - 1;
            }
        }
        while (EquippedSpells[SelectedSpellIndex] == null);

        if (Main.myPlayer == Player.whoAmI)
        {
            Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<SpellSwapProjectile>(), 0, 0, Player.whoAmI);
        }
    }

    SpellElement[] elementsArray;
    public override void ResetEffects()
    {
        elementsArray ??= (SpellElement[])Enum.GetValues(typeof(SpellElement));
        foreach (SpellElement spellDamageType in elementsArray)
        {
            ElementalDamageBoosts[spellDamageType] = 1f;
            ElementalExpertiseBoosts[spellDamageType] = 1f;
        }

        foreach (int catalystType in CatalystBoostActive.Keys)
        {
            CatalystBoostActive[catalystType] = false;
        }
    }

    public override void PostUpdate()
    {
        /*foreach(var spell in EquippedSpells)
        {
            spell.NewText();
        }*/
        SpellUISystem system = GetInstance<SpellUISystem>();
        if (Main.playerInventory)
        {
            if (system.userInterface.CurrentState == null)
            {
                system.ShowUI();
            }
        }
        else
        {
            if (system.userInterface.CurrentState != null)
            {
                system.HideUI();
            }
        }

        if (KeybindSystem.CycleSpellsForward.JustReleased)
        {
            CycleSpells(1);
        }
        else if (KeybindSystem.CycleSpellsBackward.JustReleased)
        {
            CycleSpells(-1);
        }

        foreach (var spellElement in ElementalExpertiseBoosts.Keys)
        {
            ElementalExpertiseBoosts[spellElement] *= (1 - Player.manaSickReduction);
        }

        foreach (var spellElement in ElementalDamageBoosts.Keys)
        {
            ElementalDamageBoosts[spellElement] *= (1 - Player.manaSickReduction);
        }
    }
}
