using NeoParacosm.Content.Projectiles.Friendly.Summon;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class EmblemOfTheEnergy : ModItem
{
    int timer = 0;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 20);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            if (timer % 200 == 0 && player.ownedProjectileCounts[ProjectileType<SolarSentryTurret>()] < 6)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.5f, -0.3f) }, player.Center);
                Projectile.NewProjectileDirect(
                    player.GetSource_Accessory(Item),
                    player.Center,
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f)) * Main.rand.NextFloat(4, 8),
                    ProjectileType<SolarSentryTurret>(),
                    (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(40),
                    1f,
                    player.whoAmI,
                    ai1: 720
                    );
            }
        }
        timer++;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<EmblemOfTheCentury>());
        recipe.AddIngredient(ItemID.EyeoftheGolem);
        recipe.AddIngredient(ItemID.LunarTabletFragment, 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}