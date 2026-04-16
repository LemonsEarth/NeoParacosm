using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class GreatFireballSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 36;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 6 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire],
                ProjectileType<GreatFireball>(), GetDamage(player), 1f, player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 50;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Fire];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FireballSpell>(), 1);
        recipe.AddIngredient(ItemID.HellstoneBar, 8);
        recipe.AddTile(TileID.Bookcases);
        recipe.Register();
    }
}