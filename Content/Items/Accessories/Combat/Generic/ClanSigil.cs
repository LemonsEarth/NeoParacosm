using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class ClanSigil : ModItem
{
    readonly float dmgBoost = 4f;
    readonly float critBoost = 4f;
    public static float NearbyRange { get; private set; } = 400;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(dmgBoost, critBoost);
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        foreach (var ply in Main.ActivePlayers)
        {
            if (player.team == ply.team && player.DistanceSQ(ply.Center) < NearbyRange * NearbyRange)
            {
                ply.GetDamage(DamageClass.Generic) += dmgBoost / 100f;
                ply.GetCritChance(DamageClass.Generic) += critBoost;
                ply.statDefense += 2;
            }
        }

        if (!hideVisual)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(
                    player.Center - Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / 16f) + MathHelper.ToRadians((float)Main.timeForVisualEffects)) * NearbyRange,
                    DustID.GemDiamond,
                    Vector2.Zero
                    ).noGravity = true;
            }
        }
    }
}
