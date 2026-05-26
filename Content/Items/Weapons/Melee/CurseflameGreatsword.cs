using NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class CurseflameGreatsword : ModItem
{
    int useCounter = 0;
    int special = 0;
    int specialCDTimer = 0;
    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.DamageType = DamageClass.Melee;
        Item.width = 76;
        Item.height = 76;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.UseSound = SoundID.Item71;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<CurseflameGreatswordHeldProj>();
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
            LemonUtils.DustCircle(player.Center, 16, 8, DustID.GemEmerald, 2f);
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

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.FieryGreatsword, 1);
        recipe.AddIngredient(ItemType<Gravesword>(), 1);
        recipe.AddIngredient(ItemID.CursedFlame, 20);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}