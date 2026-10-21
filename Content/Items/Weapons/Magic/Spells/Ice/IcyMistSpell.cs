using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;

public class IcyMistSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 80;
    public override Vector2 GetTargetVector(Player player)
    {
        return player.Center - Vector2.UnitY * 100;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<IcyMistProj>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(
                Item.GetSource_FromAI(),
                player.Center,
                player.Center.DirectionTo(Main.MouseWorld) * 10,
                ProjectileType<IcyMistProj>(),
                (int)(GetDamage(player)),
                1f,
                player.whoAmI,
                ai0: 200 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 4),
                ai1: player.GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 2));

        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 2;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Ice];
    }
}