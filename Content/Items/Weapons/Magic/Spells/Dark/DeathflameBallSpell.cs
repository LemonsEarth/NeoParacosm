using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class DeathflameBallSpell : BaseSpell
{
    public override int AttackCooldown => 90;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            float dirMul = 6 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire] * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Dark];
            float duration = 120 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 4);
            float height = 2 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2);
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * dirMul,
                ProjectileType<LingeringDeathflameFriendly>(), GetDamage(player), 1f, player.whoAmI, 1, duration, height);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 36;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Dark, SpellElement.Fire];
    }
}