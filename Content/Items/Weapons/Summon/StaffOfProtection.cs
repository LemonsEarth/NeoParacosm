using NeoParacosm.Core.Systems.Assets;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class StaffOfProtection : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.StaffMinionSlotsRequired[Type] = 3;
    }

    public override void SetDefaults()
    {
        Item.width = 68;
        Item.height = 68;
        Item.mana = 50;
        Item.noMelee = true;
        Item.damage = 2;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.UseSound = SFX.SummonStaff;
        Item.buffType = BuffType<StaffOfProtectionBuff>();
        Item.shoot = ProjectileType<StaffOfProtectionMinion>();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 3);
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
