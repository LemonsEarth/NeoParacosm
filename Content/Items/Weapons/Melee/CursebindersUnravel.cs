using NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;
using Terraria.Audio;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class CursebindersUnravel : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.damage = 88;
        Item.DamageType = DamageClass.Melee;
        Item.width = 80;
        Item.height = 80;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 10);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<CursebindersUnravelHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            SoundEngine.PlaySound(SoundID.Item1, player.Center);
            Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<CursebindersUnravelHeldProjBlocking>(), 0, knockback, player.whoAmI);
        }
        else
        {
            SoundEngine.PlaySound(SoundID.Item71, player.Center);
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        }
        return false;
    }
}