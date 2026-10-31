using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class SecretsOfTheShadows : ModItem
{
    readonly float magicCritBoost = 15f;
    readonly float expertiseBoost = 40f;
    readonly int defenseReduction = 15;
    public static readonly int lifeRegenReduction = 5;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicCritBoost, expertiseBoost, defenseReduction, lifeRegenReduction);
    public override void SetDefaults()
    {
        Item.width = 74;
        Item.height = 62;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetCritChance(DamageClass.Magic) += magicCritBoost;
        player.AddGlobalElementalExpertiseBoost(expertiseBoost / 100f);
        player.statDefense -= defenseReduction;
        player.GetModPlayer<SecretsOfTheShadowsPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SecretsOfTheHexes>(), 1);
        recipe.AddIngredient(ItemID.Ectoplasm, 10);
        recipe.AddIngredient(ItemType<KnightsLostSoul>(), 3);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}

public class SecretsOfTheShadowsPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic))
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }
    }

    public override void UpdateBadLifeRegen()
    {
        if (Active)
        {
            Player.lifeRegen -= SecretsOfTheShadows.lifeRegenReduction * 2;
        }
    }
}
