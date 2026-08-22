using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;

public class GrassBladestormSpell : BaseSpell
{
    public override int AttackCooldown => 180;
    public override int ManaCost => 50;
    public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (0.2f, 0.3f) }, player.Center);
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (-0.3f, -0.1f) }, player.Center);

        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 2 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature],
                ProjectileType<GrassBladestorm>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai0: 210 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature],
                ai1: 10
                );
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<LeafTornadoSpell>(), 1);
        recipe.AddIngredient(ItemID.HallowedBar, 10);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 6;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 10);
        Item.rare = ItemRarityID.Pink;
        SpellElements = [SpellElement.Nature];
    }
}
