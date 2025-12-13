using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Players.NPEffectPlayers;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.BossSummons;

public class AncientCallingHorn : ModItem
{
    int useCounter = 0;

    public override void SetDefaults()
    {
        Item.damage = 0;
        Item.knockBack = 20;
        Item.crit = 0;
        Item.width = 46;
        Item.height = 42;
        Item.useTime = 15;
        Item.useAnimation = 300;
        Item.reuseDelay = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = null;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<PulseEffect>();
        Item.noMelee = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.ItemAnimationJustStarted)
        {
            SoundEngine.PlaySound(ParacosmSFX.AncientCallingHorn with { Volume = 0.75f }, player.Center);
            NPPlayer.savedMusicVolume = Main.musicVolume;
        }
        Main.musicVolume = 0f;
        player.GetModPlayer<NPEffectPlayer>().DCEffectNoFogPosition = player.Center;
        player.GetModPlayer<NPEffectPlayer>().DCEffectNoFogDistance = 1000;
        player.GetModPlayer<NPEffectPlayer>().DCEffectMaxFogOpacity = 1;

        return null;
    }

    public override void UpdateInventory(Player player)
    {
        
    }


    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(
            source,
            position,
            Vector2.Zero,
            ProjectileType<PulseEffect>(),
            0,
            0,
            player.whoAmI,
            2, 15, 5
            );
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 12);
        recipe.AddRecipeGroup("NeoParacosm:AnyGoldBar", 15);
        recipe.AddIngredient(ItemID.Bone, 12);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
