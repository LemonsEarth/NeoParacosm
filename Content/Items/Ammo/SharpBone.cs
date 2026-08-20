using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Ammo;

public class SharpBone : ModItem
{
    public override void SetStaticDefaults()
    {
        AmmoID.Sets.IsSpecialist[Type] = true;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 6));
        ItemID.Sets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 68;
        Item.height = 60;
        Item.value = Item.buyPrice(0, 0, 1, 0);
        Item.rare = ItemRarityID.Green;
        Item.ammo = AmmoID.Dart;
        Item.shoot = ProjectileType<SharpBoneProjectile>();
        Item.consumable = true;
        Item.maxStack = 9999;
    }
}
