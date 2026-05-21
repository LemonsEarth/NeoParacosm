using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class MagiluminationNecklace : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FulculminationPlayer>().Active = true;
        player.GetModPlayer<MagiluminationNecklacePlayer>().Active = true;
        player.hasMagiluminescence = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<Fulculmination>(), 1);
        recipe.AddIngredient(ItemID.Magiluminescence);
        recipe.AddIngredient(ItemID.PanicNecklace);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class MagiluminationNecklacePlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active)
        {
            Player.AddBuff(BuffID.Panic, 12 * 60);
        }
    }
}
