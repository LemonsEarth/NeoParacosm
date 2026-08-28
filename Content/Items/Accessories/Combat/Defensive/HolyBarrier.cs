using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class HolyBarrier : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HolyBarrierPlayer.MaxDR, HolyBarrierPlayer.MaxTimer / 60);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
        Item.defense = 3;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<HolyBarrierPlayer>().Active = true;
    }

    //override hold
}

public class HolyBarrierPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public int Timer { get; set; } = 0;
    public static int MaxTimer { get; set; } = 600;
    public static float MaxDR { get; set; } = 12;
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
                        ParticleID.Streak,
                        Player.RandomPos(8, 8),
                        -Vector2.UnitY * Main.rand.NextFloat(0.5f, 3), Color.LightYellow);
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
                    ProjectileType<HolyRepelProjFriendly>(),
                    0, 0,
                    ai0: 200,
                    ai1: 10,
                    ai2: 3
                    );
        }
        Timer = 0;
    }
}

public class HolyBarrierChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<HolyBarrier>(), 2, 10, true);
    }
}
