using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class RotPerfume : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 11;
        Item.DamageType = DamageClass.Magic;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 10;
        Item.useAnimation = 20;
        Item.reuseDelay = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item45;
        Item.autoReuse = true;
        Item.mana = 12;
        Item.shoot = ModContent.ProjectileType<RotGas>();
        Item.shootSpeed = 3;
        Item.noMelee = true;
    }
}