using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class MagicOilOfBlight : ModItem
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
        player.GetModPlayer<MagicOilOfBlightPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MagicOilOfFrostburn>(), 1);
        recipe.AddIngredient(ItemID.SoulofNight, 12);
        recipe.AddIngredient(ItemID.CursedFlame, 15);
        recipe.AddIngredient(ItemID.Ichor, 15);
        recipe.AddTile(TileID.AlchemyTable);
        recipe.Register();
    }
}

public class MagicOilOfBlightPlayer : ModPlayer
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
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(BuffID.CursedInferno, 180);
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Melee))
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Ichor, 180);
            }

            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }
    }
}
