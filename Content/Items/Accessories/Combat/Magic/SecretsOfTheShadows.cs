using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class SecretsOfTheShadows : ModItem
{
    readonly float magicDamageBoost = 15f;
    readonly float magicCritBoost = 15f;
    readonly float expertiseBoost = 10f;
    int defenseReduction = 10;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, magicCritBoost, expertiseBoost, defenseReduction);
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
        float missingLifePercent = 1f - player.GetLifePercent();
        player.AddGlobalElementalExpertiseBoost(missingLifePercent + expertiseBoost / 100f);
        player.statDefense -= defenseReduction;
        player.GetModPlayer<SecretsOfTheShadowsPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SecretsOfTheHexes>(), 1);
        recipe.AddIngredient(ItemID.SorcererEmblem, 1);
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
}
