using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class BlightedEuphoria : ModItem
{
    public static float DMGBoost = 10f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DMGBoost);
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 4);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BlightedEuphoriaPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<AccursedEuphoria>(), 1);
        recipe.AddIngredient(ItemType<GoldenEuphoria>(), 1);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}

public class BlightedEuphoriaPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && target.HasBuff(BuffID.Ichor))
        {
            modifiers.FinalDamage *= 1 + (BlightedEuphoria.DMGBoost / 100f);
        }

        if (Active && target.HasBuff(BuffID.CursedInferno))
        {
            modifiers.FinalDamage *= 1 + (BlightedEuphoria.DMGBoost / 100f);
        }
    }
}
