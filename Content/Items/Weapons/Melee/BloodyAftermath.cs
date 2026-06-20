using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.Audio;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class BloodyAftermath : ModItem
{
    int useCounter = 0;
    public override void SetDefaults()
    {
        Item.damage = 250;
        Item.DamageType = DamageClass.Melee;
        Item.width = 100;
        Item.height = 100;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<BloodyAftermathSlash>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
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

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        LemonUtils.DrawDreadlordWeaponGlowInInventory(Type, position, scale, spriteBatch);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        LemonUtils.DrawDreadlordWeaponGlowInWorld(Item, rotation, scale, spriteBatch);
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2 && player.GetModPlayer<BloodyAftermathPlayer>().HitCount >= BloodyAftermathPlayer.MAX_HIT_COUNT)
        {
            player.GetModPlayer<BloodyAftermathPlayer>().HitCount = 0;
            SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 0.5f, PitchRange = (0.2f, 0.8f) }, player.Center);

            LemonUtils.DustBurst(8, player.Center, DustType<FireDust>(), 4, 4, 0.5f, 1.5f, Color.Red);
            LemonUtils.DustBurst(8, player.Center, DustType<FireDust>(), 4, 4, 0.5f, 1.5f, Color.DarkRed);
            LemonUtils.DustBurst(8, player.Center, DustType<FireDust>(), 4, 4, 0.5f, 1.5f, Color.Black);

            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectileDirect(
                        source,
                        player.Center + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * 200 * k + Vector2.UnitX * 80,
                        Vector2.Zero,
                        ProjectileType<BloodyAftermathSlash2>(),
                        damage,
                        knockback,
                        player.whoAmI,
                        ai0: 0,
                        ai1: 1,
                        ai2: 10 + k * 10
                        );
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectileDirect(
                        source,
                        player.Center + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * 200 * k - Vector2.UnitX * 80,
                        Vector2.Zero,
                        ProjectileType<BloodyAftermathSlash2>(),
                        damage,
                        knockback,
                        player.whoAmI,
                        ai0: 1,
                        ai1: -1,
                        ai2: 10 + k * 10
                        );
                }
            }
        }
        else
        {
            int direction = LemonUtils.Sign(player.DirectionTo(Main.MouseWorld).X, 1);
            int flipVertically = useCounter % 2 == 0 ? 1 : 0;
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: flipVertically, ai1: direction);
        }
        useCounter++;
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<SupremeLightsBane>(), 1);
        recipe.AddIngredient(ItemType<SupremeBloodButcherer>(), 1);
        recipe.AddIngredient(ItemType<NightmareScale>(), 12);
        recipe.AddIngredient(ItemType<DivineFlesh>(), 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}

public class BloodyAftermathPlayer : ModPlayer
{
    public int HitCount { get; set; } = 0;
    public const int MAX_HIT_COUNT = 20;
    public override void ResetEffects()
    {

    }
}