using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class BeFuelSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 0;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<BeFuelDebuff>(), 20 * 60);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Dark];
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        
    }
}