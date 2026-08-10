using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class MagicOilOfFrostburn : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 38;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MagicOilOfFrostburnPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MagicOilOfFire>(), 1);
        recipe.AddIngredient(ItemID.IceBlock, 50);
        recipe.AddIngredient(ItemID.Bone, 25);
        recipe.AddTile(TileID.AlchemyTable);
        recipe.Register();
    }
}

public class MagicOilOfFrostburnPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Melee) && Main.rand.NextBool(4))
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
