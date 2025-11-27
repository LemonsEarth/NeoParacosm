using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class GoldenHeartSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 0;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<GoldenHeartBuff>(), (int)(30 * 60 * player.GetElementalExpertiseBoost(SpellElement.Holy)));
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Holy];
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        
    }
}