using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Magic;

public class AscendedCrimsonRod : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.CrimsonRod;
    public override Color Color => Color.Yellow;

    public override void SetDefaults()
    {
        Item.damage = 35;
        Item.DamageType = DamageClass.Magic;
        Item.width = 54;
        Item.height = 62;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.reuseDelay = 30;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<AscendedCrimsonRodHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.mana = 6;
        Item.channel = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
}