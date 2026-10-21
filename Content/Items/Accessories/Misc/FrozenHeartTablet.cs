using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class FrozenHeartTablet : ModItem
{
    public static int RestorationBoost { get; set; } = 30;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RestorationBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Blue;
        Item.defense = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FrozenHeartTabletPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemType<HeartTablet>());
        recipe1.AddIngredient(ItemType<FrigidFossil>(), 5);
        recipe1.AddTile(TileID.Anvils);
        recipe1.Register();
    }
}

public class FrozenHeartTabletItem : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.healLife > 0;
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (player.GetModPlayer<FrozenHeartTabletPlayer>().Active)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ProjectileType<IcyMistProj>() && proj.owner == player.whoAmI)
                {
                    proj.Kill();
                    break;
                }
            }
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(
                    item.GetSource_FromAI(),
                    player.Center,
                    Vector2.Zero,
                    ProjectileType<IcyMistProj>(),
                    2,
                    1f,
                    player.whoAmI,
                    ai0: 360 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 2),
                    ai1: player.GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 1));
            }
        }

        return null;
    }
}

public class FrozenHeartTabletPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (Active)
        {
            healValue += FrozenHeartTablet.RestorationBoost;
        }
    }
}
