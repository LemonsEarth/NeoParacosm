using NeoParacosm.Content.Projectiles.Interactable;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class ImmortalEnergy : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ImmortalEnergyPlayer>().Active = true;
    }
}

public class ImmortalEnergyPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    const int MAX_SPAWNS = 5;
    int spawnsAvailable = MAX_SPAWNS;

    int deathTimer = 0;
    int fullRespawnTime = 60;

    public override void ResetEffects()
    {
        Active = false;
        deathTimer = 0;
    }

    public override void UpdateDead()
    {
        if (Active)
        {
            if (deathTimer == 0)
            {
                fullRespawnTime = Player.respawnTimer;
            }
            if (spawnsAvailable > 0 && deathTimer % (fullRespawnTime / 5) == 0)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Vector2 spawnPos = Player.Center + LemonUtils.RandomVector2Rectangular(400, 400, 200, 200);
                    int velocitySign = Main.rand.NextSign();
                    Vector2 velocityDir = Player.DirectionTo(spawnPos).RotatedBy(velocitySign * (MathHelper.PiOver2 + MathHelper.PiOver4));
                    Projectile.NewProjectileDirect(
                        Player.GetSource_FromThis(),
                        spawnPos,
                        velocityDir * Main.rand.NextFloat(3f, 6f),
                        ProjectileType<ImmortalEnergyProj>(),
                        0, 0, Player.whoAmI,
                        ai1: 600,
                        ai2: Main.rand.NextSign()
                        );
                    spawnsAvailable--;
                }
            }
        }
        deathTimer++;
    }

    public override void OnRespawn()
    {
        spawnsAvailable = MAX_SPAWNS;
    }

    public override void UpdateEquips()
    {

    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {

    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        return true;
    }
}
