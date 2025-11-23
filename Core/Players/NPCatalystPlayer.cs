using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Systems.Misc;
using System.Collections.Generic;
using System.Linq;
using Terraria;
namespace NeoParacosm.Core.Players;

public class NPCatalystPlayer : ModPlayer
{
    public Dictionary<BaseSpell.SpellElement, float> ElementalDamageBoosts = new Dictionary<BaseSpell.SpellElement, float>();
    public Dictionary<BaseSpell.SpellElement, float> ElementalSpeedBoosts = new Dictionary<BaseSpell.SpellElement, float>();

    public List<BaseSpell> EquippedSpells { get; private set; } = new List<BaseSpell>();
    public BaseSpell SelectedSpell => (SelectedSpellIndex < 0) ? null : EquippedSpells[SelectedSpellIndex];
    public int SelectedSpellIndex { get; private set; } = -1;

    public void AddSpell(BaseSpell spell)
    {
        int foundSpell = EquippedSpells.IndexOf(EquippedSpells.Find(sp => sp.Type == spell.Type));
        if (foundSpell == -1)
        {
            EquippedSpells.Add(spell);
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
            ElementalSpeedBoosts[spellDamageType] = 1f;
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
