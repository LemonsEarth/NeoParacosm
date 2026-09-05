using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class PaladinsHolyBarrier : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PaladinsHolyBarrierPlayer.MaxDR, PaladinsHolyBarrierPlayer.MaxTimer / 60);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Yellow;
        Item.defense = 3;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<PaladinsHolyBarrierPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<HolyBarrier>(), 1);
        recipe.AddIngredient(ItemID.PaladinsShield, 1);
        recipe.AddIngredient(ItemID.Ectoplasm, 5);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class PaladinsHolyBarrierPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public int Timer { get; set; } = 0;
    public static int MaxTimer { get; set; } = 600;
    public static float MaxDR { get; set; } = 16;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (Active)
        {
            if (Timer < MaxTimer)
            {
                Timer++;
            }

            if (NPPlayer.Timer % 10 == 0)
            {
                for (int i = 0; i < Timer / 150f; i++)
                {
                    ParticleSystem.SpawnParticle(
                        ParticleID.Circle,
                        Player.RandomPos(8, 8),
                        -Vector2.UnitY * Main.rand.NextFloat(0.5f, 3),
                        Color.LightYellow,
                        scale: 0.8f,
                        data0: 0.1f, data1: 0.2f);
                }
            }

            Player.endurance += MathHelper.Lerp(0, MaxDR, (float)Timer / MaxTimer) / 100f;
        }
        else
        {
            Timer = 0;
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Timer == MaxTimer)
        {
            Projectile.NewProjectileDirect(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    Vector2.Zero,
                    ProjectileType<GreaterHolyRepelProjFriendly>(),
                    0, 0,
                    ai0: 250,
                    ai1: 10,
                    ai2: 3
                    );
        }
        Timer = 0;
    }
}
