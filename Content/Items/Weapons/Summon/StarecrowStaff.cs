using NeoParacosm.Content.Projectiles.Friendly.Summon.Minions;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class StarecrowStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
    }

    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;
        Item.mana = 12;
        Item.noMelee = true;
        Item.damage = 12;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item44;
        Item.buffType = ModContent.BuffType<StarecrowBuff>();
        Item.shoot = ModContent.ProjectileType<Starecrow>();
        Item.rare = ItemRarityID.Green;
        Item.value = 20000;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);
        if (Main.myPlayer == player.whoAmI)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
        }
        return false;
    }
}
