using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class ArmoredBodyAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 0, 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ArmoredBodyAugmentPlayer>().Active = true;
        if (player.HasBuff(BuffID.Ironskin) && player.HasBuff(BuffID.Endurance))
        {
            player.noKnockback = true;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 15)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class ArmoredBodyAugmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && Main.rand.NextBool(8))
        {
            Player.AddBuff(BuffID.Ironskin, 60 * 30);
            Player.AddBuff(BuffID.Endurance, 60 * 30);
        }
    }
}
