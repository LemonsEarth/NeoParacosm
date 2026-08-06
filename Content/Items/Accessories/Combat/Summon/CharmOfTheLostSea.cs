using NeoParacosm.Content.Projectiles.Friendly.Summon;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class CharmOfTheLostSea : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CharmOfTheLostSeaPlayer>().Active = true;

        if (Main.myPlayer == player.whoAmI)
        {
            if (player.ownedProjectileCounts[ProjectileType<WaterSpirit>()] < 1)
            {
                Projectile.NewProjectileDirect(
                    player.GetSource_Accessory(Item),
                    player.Center,
                    Vector2.Zero,
                    ProjectileType<WaterSpirit>(),
                    (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(10),
                    1f,
                    player.whoAmI
                    );
            }
        }
    }
}

public class CharmOfTheLostSeaPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}
