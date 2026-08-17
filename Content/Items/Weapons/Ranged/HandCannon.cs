using NeoParacosm.Common.RecipeGroups;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class HandCannon : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.knockBack = 10f;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 48;
        Item.height = 34;
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Pink;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.CannonballFriendly;
        Item.useAmmo = ItemID.Cannonball;
        Item.shootSpeed = 14;
        Item.noMelee = true;
        Item.UseSound = SFX.Explosion with { Volume = 0.4f };
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-8, 8);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(
            source,
            position,
            velocity,
            type,
            damage,
            knockback,
            player.whoAmI
            );

        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Cannon, 1);
        recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}

public class AmmoModificationsGlobalItem : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        if (entity.type == ItemID.Cannonball)
        {
            entity.ammo = ItemID.Cannonball;
        }
    }

    public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
    {
        if (ammo.type == ItemID.Cannonball)
        {
            type = ProjectileID.CannonballFriendly;
        }
    }
}
