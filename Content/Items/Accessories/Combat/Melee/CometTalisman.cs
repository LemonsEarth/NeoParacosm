using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class CometTalisman : ModItem
{
    readonly float critBoost = 60f;
    readonly float critDamageBoost = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost, critDamageBoost);
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.velocity.Y > 7)
        {
            player.GetCritChance(DamageClass.Melee) += critBoost;
            player.GetModPlayer<CometTalismanPlayer>().Active = true;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MeteoriteBar, 12)
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 8)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class CometTalismanPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active)
        {
            modifiers.CritDamage += 20f / 100f;
        }
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsTrueMelee())
        {
            modifiers.CritDamage += 20f / 100f;
        }
    }
}
