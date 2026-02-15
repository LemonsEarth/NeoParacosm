using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RoundShield : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 0, 80);
        Item.rare = ItemRarityID.White;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RoundShieldPlayer>().roundShield = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 15);
        recipe.AddRecipeGroup(RecipeGroupID.IronBar, 6);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}

public class RoundShieldPlayer : ModPlayer
{
    public bool roundShield { get; set; } = false;

    public override void ResetEffects()
    {
        roundShield = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (roundShield && !Player.HasBuff(BuffType<KnockbackCooldown>()))
        {
            Player.AddBuff(BuffType<KnockbackCooldown>(), 1800);
        }
    }

    public override void PostUpdateEquips()
    {
        if (roundShield && !Player.HasBuff(BuffType<KnockbackCooldown>())) // On Hurt code is in Main
        {
            Player.noKnockback = true;
        }
    }
}

public class KnockbackCooldown : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}