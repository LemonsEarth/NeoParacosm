using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class SigilOfTheCreatureSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<SigilOfTheCreature>()] < 3;
    }

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        Projectile.NewProjectileDirect(
            player.GetSource_FromThis(),
            Main.MouseWorld,
            Vector2.Zero,
            ProjectileType<SigilOfTheCreature>(),
            GetDamage(player),
            3f,
            player.whoAmI,
            ai1: player.GetElementalExpertiseBoostMultiplied(SpellElement.Holy, 1)
            );
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 10;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Holy];
    }
}