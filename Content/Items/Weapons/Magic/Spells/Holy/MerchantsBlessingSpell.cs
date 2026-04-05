using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class MerchantsBlessingSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<MerchantsBlessingBuff>(), (int)(30 * 60 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Holy, 4)));
        SoundEngine.PlaySound(SoundID.CoinPickup with { PitchRange = (0.1f, 0.3f) }, player.Center);
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