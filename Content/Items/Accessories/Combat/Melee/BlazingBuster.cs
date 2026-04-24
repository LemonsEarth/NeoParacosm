using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class BlazingBuster : ModItem
{
    readonly float damageBoost = 15f;
    readonly float attackSpeedBoost = 22f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, attackSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 60;
        Item.height = 52;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 80);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetDamage(DamageClass.Melee) += damageBoost / 100f;
        player.GetAttackSpeed(DamageClass.Melee) += attackSpeedBoost / 100f;
        player.pickSpeed -= 0.15f;
        player.tileSpeed += 0.15f;
        player.kbGlove = true;
        player.autoReuseGlove = true;
        player.meleeScaleGlove = true;
        player.GetModPlayer<BlazingBusterPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MechanicalArm>(), 1);
        recipe.AddIngredient(ItemID.FireGauntlet, 1);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class BlazingBusterPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (Active && !Player.ItemTimeIsZero)
        {
            Player.DOTDebuff(5);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}
