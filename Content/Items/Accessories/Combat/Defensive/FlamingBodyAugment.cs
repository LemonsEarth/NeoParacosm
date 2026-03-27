using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class FlamingBodyAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FlamingBodyAugmentPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Fireblossom, 5)
            .AddIngredient(ItemID.HellstoneBar, 12)
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 15)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class FlamingBodyAugmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && Main.rand.NextBool(4))
        {
            for (int i = 0; i < 8; i++)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-60, 60))) * 6, ProjectileID.BallofFire, 30, 6f, Player.whoAmI);
            }
        }
    }
}
