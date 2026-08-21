using NeoParacosm.Content.Items.Accessories.Combat.Magic;
using NeoParacosm.Content.Items.Accessories.Misc;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Core.Players;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class MossHeart : ModItem
{
    public static int LifeBoost { get; set; } = 40;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeBoost);

    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MossHeartPlayer>().Active = true;
        player.statLifeMax2 += LifeBoost;
        int extraReduceTime = 4;
        for (int i = 0; i < player.buffType.Length; i++)
        {
            int buffID = player.buffType[i];
            if (buffID == BuffID.Poisoned || buffID == BuffID.Venom)
            {
                player.buffTime[i] -= extraReduceTime;
            }
        }

        if (player.ZoneJungle)
        {
            player.AddBuff(BuffID.ManaRegeneration, 2);
            player.AddBuff(BuffID.MagicPower, 2);
            player.AddBuff(BuffID.Regeneration, 2);
        }

        if (Main.myPlayer == player.whoAmI && NPPlayer.Timer % 8 == 0 && player.velocity.LengthSquared() > 5 * 5)
        {
            Projectile.NewProjectileDirect(
                player.GetSource_Accessory(Item),
                player.Center,
                Vector2.UnitY.RotatedByRandom(6.28f) * 0.3f,
                Main.rand.Next(ProjectileID.SporeGas, ProjectileID.SporeGas3 + 1),
                12,
                0f,
                player.whoAmI
                );
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<StoneHeart>(), 1);
        recipe.AddIngredient(ItemType<MagicMossball>(), 1);
        recipe.AddIngredient(ItemType<MagicSpores>(), 1);
        recipe.AddIngredient(ItemID.Vine, 2);
        recipe.AddIngredient(ItemID.Stinger, 5);
        recipe.AddIngredient(ItemID.JungleSpores, 6);
        recipe.AddIngredient(ItemType<PureLifeEnergy>(), 2);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class MossHeartPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (Active && Player.HasAnyFireDebuff())
        {
            Player.DOTDebuff(10);
        }
    }
}
