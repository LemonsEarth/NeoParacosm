using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class MushroomDiscLauncher : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 100;
        Item.knockBack = 3f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 68;
        Item.height = 32;
        Item.useTime = 15;
        Item.useAnimation = 45;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<MushroomDisc>();
        Item.useAmmo = ItemType<GlowingMushroomSpore>();
        Item.shootSpeed = 20;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item61 with { PitchRange = (0.4f, 0.8f) }, player.Center);
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 16, MathHelper.Pi / 16));
        type = ProjectileType<MushroomDisc>();
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<MushroomCannon>())
            .AddIngredient(ItemID.ShroomiteBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
