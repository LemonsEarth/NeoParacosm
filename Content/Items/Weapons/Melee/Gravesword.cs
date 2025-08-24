using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class Gravesword : ModItem
{
    int useCounter = 0;
    int special = 0;
    int specialCDTimer = 0;
    public override void SetDefaults()
    {
        Item.damage = 45;
        Item.DamageType = DamageClass.Melee;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.UseSound = SoundID.Item71;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<GraveswordHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2 && !player.IsGrounded() && specialCDTimer == 0)
        {
            special = 1;
            specialCDTimer = 180;
            return true;
        }
        else
        {
            special = 0;
            return null;
        }
    }

    public override void UpdateInventory(Player player)
    {
        if (specialCDTimer == 1)
        {
            LemonUtils.DustCircle(player.Center, 16, 8, DustID.Ash, 2f, color: Color.Black);
        }

        if (specialCDTimer > 0)
        {
            specialCDTimer--;
        }
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int direction = 1;
        if (useCounter % 2 != 0)
        {
            direction = -1;
        }
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: special, ai1: direction);
        useCounter++;
        return false;
    }
}