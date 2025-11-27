using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class RestoreManaSpell : BaseSpell
{
    public override int AttackCooldown => 10;
    public override int ManaCost => 0;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        float boostAdjusted = 1 + (player.GetElementalExpertiseBoost(SpellElement.Dark) - 1) * 3;
        player.RestoreMana((int)(30 * MathF.Max(boostAdjusted, 0)));
        player.statLife -= 7;
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