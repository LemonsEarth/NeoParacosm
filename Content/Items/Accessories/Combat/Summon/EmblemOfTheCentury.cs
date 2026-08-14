using NeoParacosm.Content.Projectiles.Friendly.Summon;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class EmblemOfTheCentury : ModItem
{
    int timer = 0;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 20);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            if (timer % 200 == 0 && player.ownedProjectileCounts[ProjectileType<RedSentryTurret>()] < 6)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.5f, -0.3f) }, player.Center);
                Projectile.NewProjectileDirect(
                    player.GetSource_Accessory(Item),
                    player.Center,
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f)) * Main.rand.NextFloat(4, 8),
                    ProjectileType<RedSentryTurret>(),
                    (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(20),
                    1f,
                    player.whoAmI,
                    ai1: 600
                    );
            }
        }
        timer++;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<EmblemOfTheSentry>());
        recipe.AddIngredient(ItemID.HallowedBar, 12);
        recipe.AddIngredient(ItemID.SoulofFright, 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}