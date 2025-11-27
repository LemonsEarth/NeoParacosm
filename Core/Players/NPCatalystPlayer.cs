using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems.Misc;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;
namespace NeoParacosm.Core.Players;

public class NPCatalystPlayer : ModPlayer
{
    /// <summary>
    /// Damage boosts for each spell element type.
    /// Each is set to 0 at the start of every Update.
    /// Value is a percentage, so 1f would be a 100% damage increase.
    /// </summary>
    public Dictionary<BaseSpell.SpellElement, float> ElementalDamageBoosts = new Dictionary<BaseSpell.SpellElement, float>();

    /// <summary>
    /// Speed, Projectile amount, knockback and other misc boosts for each spell element type.
    /// Expertise affects each spell differently, if at all.
    /// Each is set to 1 at the start of every Update.
    /// Value is a percentage, so 2f could be a 100% speed increase.
    /// </summary>
    public Dictionary<BaseSpell.SpellElement, float> ElementalExpertiseBoosts = new Dictionary<BaseSpell.SpellElement, float>();

    public const int BASE_SPELL_SLOTS = 3;
    public int maxSpellSlots = 3;

    public override void Initialize()
    {
        maxSpellSlots = BASE_SPELL_SLOTS;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(maxSpellSlots)] = maxSpellSlots;
    }

    public override void LoadData(TagCompound tag)
    {
        maxSpellSlots = tag.GetInt(nameof(maxSpellSlots));
    }

    public List<BaseSpell> EquippedSpells { get; private set; } = new List<BaseSpell>();
    public BaseSpell SelectedSpell => (SelectedSpellIndex < 0) ? null : EquippedSpells[SelectedSpellIndex];
    public int SelectedSpellIndex { get; private set; } = -1;

    public void AddSpell(BaseSpell spell)
    {
        int foundSpell = EquippedSpells.IndexOf(EquippedSpells.Find(sp => sp.Type == spell.Type));
        if (foundSpell == -1)
        {
            if (EquippedSpells.Count < maxSpellSlots)
            {
                EquippedSpells.Add(spell);
            }
            else
            {
                RemoveSpell(EquippedSpells[EquippedSpells.Count - 1]);
                EquippedSpells.Add(spell);
            }
            SelectedSpellIndex = EquippedSpells.Count - 1;
        }
        else
        {
            EquippedSpells[foundSpell] = spell;
            SelectedSpellIndex = foundSpell;
        }
    }

    public void RemoveSpell(BaseSpell spell)
    {
        EquippedSpells.RemoveAll(sp => sp.Type == spell.Type);
        if (EquippedSpells.Count == 0)
        {
            SelectedSpellIndex = -1;
        }
        else
        {
            SelectedSpellIndex = 0;
        }
    }

    public void CycleSpells(int direction = 1)
    {
        if (EquippedSpells.Count == 0)
        {
            SelectedSpellIndex = -1;
            return;
        }
        SelectedSpellIndex += direction;
        if (SelectedSpellIndex >= EquippedSpells.Count)
        {
            SelectedSpellIndex = 0;
        }
        else if (SelectedSpellIndex < 0)
        {
            SelectedSpellIndex = EquippedSpells.Count - 1;
        }
        if (Main.myPlayer == Player.whoAmI)
        {
            Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<SpellSwapProjectile>(), 0, 0, Player.whoAmI);
        }
    }

    public override void SetStaticDefaults()
    {

    }

    BaseSpell.SpellElement[] elementsArray;
    public override void ResetEffects()
    {
        elementsArray ??= (BaseSpell.SpellElement[])Enum.GetValues(typeof(BaseSpell.SpellElement));
        foreach (BaseSpell.SpellElement spellDamageType in elementsArray)
        {
            ElementalDamageBoosts[spellDamageType] = 0;
        }

        foreach (BaseSpell.SpellElement spellDamageType in elementsArray)
        {
            ElementalExpertiseBoosts[spellDamageType] = 1f;
        }
    }

    public override void PostUpdate()
    {
        if (KeybindSystem.CycleSpellsForward.JustReleased)
        {
            CycleSpells(1);
        }
        else if (KeybindSystem.CycleSpellsBackward.JustReleased)
        {
            CycleSpells(-1);
        }
    }
}
