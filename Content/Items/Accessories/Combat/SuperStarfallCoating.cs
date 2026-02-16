using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using NeoParacosm.Core.Players;
using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class SuperStarfallCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 6);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SuperStarfallCoatingPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<StarfallCoating>(), 1)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient(ItemID.SoulofLight, 8)
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}

public class SuperStarfallCoatingPlayer : CoatingPlayer
{
    public override int BaseCD => 20;
    public override void OnHitEffect(NPC npc, NPC.HitInfo hit)
    {
        Vector2 pos = npc.Center + new Vector2(Main.rand.NextFloat(-300, 300), -800);
        Projectile.NewProjectileDirect(
            Player.GetSource_FromThis(),
            pos,
            pos.DirectionTo(npc.Center) * Main.rand.NextFloat(8, 14),
            Main.rand.NextFromList(ProjectileID.SuperStar, ProjectileID.HallowStar, ProjectileType<HomingStar>()),
            (int)(hit.Damage * 0.33f),
            0f,
            Player.whoAmI,
            ai1: 180
            ).DamageType = DamageClass.Melee; // Changing to melee damage
    }
}
