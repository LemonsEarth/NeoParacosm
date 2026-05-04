using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class LightningTridentSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Vector2 vel = player.DirectionTo(TargetVector) * 20;
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                vel,
                ProjectileType<LightningTrident>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 40;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 8);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Lightning];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<LightningSpearSpell>(), 1);
        recipe.AddIngredient(ItemID.Trident, 1);
        recipe.AddIngredient(ItemID.HellstoneBar, 10);
        recipe.AddTile(TileID.Bookcases);
        recipe.Register();
    }
}