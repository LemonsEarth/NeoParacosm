using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class GravityFinaleSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 320;
    public override Vector2 GetTargetVector(Player player)
    {
        return Main.MouseWorld;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<GravityField>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            SoundEngine.PlaySound(SoundID.Item7, player.Center);
            SoundEngine.PlaySound(SoundID.Item7 with { PitchRange = (0.4f, 0.6f) }, player.Center);
            SoundEngine.PlaySound(SoundID.Item7 with { PitchRange = (-0.6f, -0.4f) }, player.Center);

            Projectile.NewProjectile(
                Item.GetSource_FromAI(),
                player.Center,
                Vector2.Zero,
                ProjectileType<GravityField>(),
                0,
                1f,
                player.whoAmI,
                ai0: 600 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Pure, 2f),
                ai1: 2f * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f),
                ai2: 8f
                );

        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Yellow;
        SpellElements = [SpellElement.Earth, SpellElement.Pure];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<GravityFieldSpell>(), 1);
        recipe.AddIngredient(ItemID.Ectoplasm, 10);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}