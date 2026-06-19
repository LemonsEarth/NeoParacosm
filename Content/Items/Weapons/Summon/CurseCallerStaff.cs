using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Summon.Minions;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Summon;

public class CurseCallerStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
    }

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.mana = 12;
        Item.noMelee = true;
        Item.damage = 100;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.UseSound = SoundID.DD2_FlameburstTowerShot;
        Item.buffType = BuffType<SmallCursedSpiritBuff>();
        Item.shoot = ProjectileType<SmallCursedSpirit>();
        Item.rare = ItemRarityID.Red;
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

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        //recipe.AddIngredient(ItemType<SupremeMusket>(), 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}
