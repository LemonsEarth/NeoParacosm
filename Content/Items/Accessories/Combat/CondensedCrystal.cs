using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class CondensedCrystal : ModItem
{
    readonly float damageBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost);
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
        player.GetDamage(DamageClass.Magic) += damageBoost / 100f;
        player.GetModPlayer<CondensedCrystalPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FallenStar, 6)
            .AddIngredient(ItemID.Diamond, 8)
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 12)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class CondensedCrystalPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic))
        {
            Main.NewText((int)Player.GetCritChance(DamageClass.Magic));
            if ((int)Player.GetCritChance(DamageClass.Magic) > 0 && Main.rand.NextBool(100 / (int)Player.GetCritChance(DamageClass.Magic))) // if crit
            {
                Player.AddBuff(BuffID.ManaRegeneration, 300);
                Player.AddBuff(BuffID.MagicPower, 300);
            }
            modifiers.DisableCrit();
        }
    }
}
