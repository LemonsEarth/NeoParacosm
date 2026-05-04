using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class HolyStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 14;
        Item.DamageType = DamageClass.Magic;
        Item.width = 28;
        Item.height = 64;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 6);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.mana = 24;
        Item.shoot = ProjectileType<HolyStaffHeldProj>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] == 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
}