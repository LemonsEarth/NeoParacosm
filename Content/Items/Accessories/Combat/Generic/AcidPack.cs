using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class AcidPack : ModItem
{
    readonly float dmgBoost = 10f;
    readonly float critBoost = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(dmgBoost, critBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.HasAnyPoisonDebuff())
        {
            player.GetDamage(DamageClass.Generic) += dmgBoost / 100f;
            player.GetCritChance(DamageClass.Generic) += critBoost;
        }
        player.GetModPlayer<PoisonousStingerPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<PoisonousStinger>(), 1);
        recipe.AddIngredient(ItemType<PoisonOrb>(), 1);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}
