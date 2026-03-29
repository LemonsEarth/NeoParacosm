using Terraria.Audio;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class HauntedBodyAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<HauntedBodyAugmentPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BoneHelm)
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 15)
            .AddIngredient(ItemID.Bone, 30)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class HauntedBodyAugmentPlayer : ModPlayer
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
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectile(
                    Player.GetSource_FromThis(), 
                    Player.Center, 
                    Vector2.UnitY.RotatedByRandom(6.28f) * 3, 
                    ProjectileID.InsanityShadowFriendly, 
                    60, 6f, 
                    Player.whoAmI, 
                    ai0: 1); // ai0 > 0 mutes it for some reason
            }
        }
    }
}
