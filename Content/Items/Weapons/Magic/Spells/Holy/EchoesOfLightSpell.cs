using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class EchoesOfLightSpell : BaseSpell
{
    public override int AttackCooldown => 24;
    public override int ManaCost => 22;
    public override Vector2 GetTargetVector(Player player)
    {
        return Main.MouseWorld;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<LightRingProjectile>()] < 20;
    }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SFX.CrystalSerpent with { PitchRange = (0.3f, 0.5f) }, player.Center);
        for (int i = -2; i <= 2; i++)
        {
            Projectile.NewProjectileDirect(
                player.GetSource_FromThis(),
                player.Center,
                player.Center.DirectionTo(Main.MouseWorld).RotatedBy(i * MathHelper.Pi / 16f) * 30,
                ProjectileType<LightRingProjectile>(),
                GetDamage(player),
                6f,
                player.whoAmI,
                ai0: 60,
                ai1: player.GetElementalExpertiseBoostMultiplied(SpellElement.Holy, 4)
                );
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 40;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.Pink;
        SpellElements = [SpellElement.Holy];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<LightRingSpell>(), 1);
        recipe.AddIngredient(ItemID.SoulofLight, 12);
        recipe.AddIngredient(ItemID.CrystalShard, 10);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}