using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class HolyLightningTorrentSpell : BaseSpell
{
    public override int AttackCooldown => 20;
    public override int ManaCost => 25;
    public override Vector2 GetTargetVector(Player player) { return player.Center - Vector2.UnitY; }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(),
                player.Top,
                Vector2.Zero,
                ProjectileType<HolyLightningTorrentSpellHeldProj>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 180;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Yellow;
        SpellElements = [SpellElement.Lightning, SpellElement.Holy];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<LightningTridentSpell>(), 1);
        recipe.AddIngredient(ItemType<KnightsLostSoul>(), 8);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}