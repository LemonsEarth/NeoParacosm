using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class BloodArtistSkull : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.lifeRegen = 2;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BloodArtistSkullPlayer>().Active = true;
    }
}

public class BloodArtistSkullPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }
}

public class BloodArtistSkullNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && !entity.friendly && !entity.dontTakeDamage;
    }

    static bool ShouldSpawnLostSoul(NPC npc)
    {
        if (npc.lifeMax < 30 || npc.realLife >= 0)
        {
            return false;
        }

        float maxHP = 600;
        float lerpT = MathF.Min(maxHP, npc.lifeMax) / maxHP;
        int chanceDenominator = (int)MathHelper.Lerp(10, 1, lerpT);
        if (Main.rand.NextBool(chanceDenominator))
        {
            return true;
        }
        return false;
    }

    public override void OnKill(NPC npc)
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.GetModPlayer<BloodArtistSkullPlayer>().Active && player.DistanceSQ(npc.Center) < 600 * 600)
            {
                if (ShouldSpawnLostSoul(npc))
                {
                    Vector2 toPlayer = npc.DirectionTo(player.Center);
                    int damage = (int)(MathF.Min(npc.lifeMax, 100));
                    float knockback = (1 - npc.knockBackResist) * 20;
                    Projectile.NewProjectileDirect(
                        npc.GetSource_Death("NeoParacosm:BloodArtistSkull"),
                        npc.Center,
                        toPlayer * Main.rand.NextFloat(4, 6),
                        ProjectileType<LostSoulFriendly>(),
                        damage,
                        knockback,
                        player.whoAmI,
                        ai0: 60,
                        ai1: 600,
                        ai2: 6
                        );
                }
            }
        }
    }
}
