using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class FurnaceSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 100;
    public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<FurnaceHeldProj>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                Vector2.Zero,
                ProjectileType<FurnaceHeldProj>(), GetDamage(player), 1f, player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 80;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Red;
        SpellElements = [SpellElement.Fire];
    }
}