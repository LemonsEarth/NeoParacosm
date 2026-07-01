using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class ArcaneSkull : ModItem
{
    public static float DamageBoost { get; private set; } = 10f;
    public static int LifeRegenBoost { get; private set; } = 3;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost, LifeRegenBoost);
    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ArcaneStonePlayer>().Active = true;
        player.GetModPlayer<ArcaneSkullPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<ArcaneStone>(), 1);
        recipe.AddIngredient(ItemID.Bone, 50);
        recipe.AddTile(TileID.BoneWelder);
        recipe.Register();
    }
}

public class ArcaneSkullBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetDamage(DamageClass.Generic) += ArcaneSkull.DamageBoost / 100f;
        player.lifeRegen += ArcaneSkull.LifeRegenBoost;
    }
}

public class ArcaneSkullPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}

public class ArcaneSkullMinionProjectile : GlobalProjectile
{
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        if (projectile.minion && projectile.GetOwner().GetModPlayer<ArcaneSkullPlayer>().Active)
        {
            projectile.GetOwner().AddBuff(BuffType<ArcaneSkullBuff>(), 600);
        }
    }
}
