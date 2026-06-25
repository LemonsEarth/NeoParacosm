using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class RoarOfDreadlordSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 60;
    public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<RoarOfDreadlordProjectile>()] == 0;
    }

    public override void SpellAction(Player player)
    {

        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                Vector2.Zero,
                ProjectileType<RoarOfDreadlordProjectile>(),
                GetDamage(player),
                20f,
                player.whoAmI,
                player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 1f) * 180
                );
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 40;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Fire, SpellElement.Dark];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.SpellTome, 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 10);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}