using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class SecretsOfTheHexes : ModItem
{
    readonly float magicCritBoost = 12f;
    readonly float darkDamageBoost = 15f;
    readonly float darkExpertiseBoost = 15f;
    readonly int defenseReduction = 8;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicCritBoost, darkDamageBoost, darkExpertiseBoost, defenseReduction);
    public override void SetDefaults()
    {
        Item.width = 70;
        Item.height = 54;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetCritChance(DamageClass.Magic) += magicCritBoost;
        player.AddElementalDamageBoost(SpellElement.Dark, darkDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Dark, darkExpertiseBoost / 100f);
        player.statDefense -= defenseReduction;
        player.GetModPlayer<SecretsOfTheHexesPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SecretsOfTheGoblins>(), 1);
        recipe.AddIngredient(ItemID.DarkShard, 2);
        recipe.AddIngredient(ItemID.SoulofNight, 8);
        recipe.AddRecipeGroup(AnyRecipeGroups.AnyMythrilBar, 10);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}

public class SecretsOfTheHexesPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic) && hit.Crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}
