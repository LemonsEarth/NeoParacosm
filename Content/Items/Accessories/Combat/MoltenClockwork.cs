using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class MoltenClockwork : ModItem
{
    readonly float attackSpeedBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(attackSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetAttackSpeed(DamageClass.Ranged) += attackSpeedBoost / 100f;
        player.GetModPlayer<MoltenClockworkPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class MoltenClockworkPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Active)
        {
            return;
        }
        if (proj.CountsAsClass(DamageClass.Ranged))
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
    }
}
