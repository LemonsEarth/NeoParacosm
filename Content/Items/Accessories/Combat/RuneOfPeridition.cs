using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RuneOfPeridition : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 12, true));
        ItemID.Sets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1, 0, 0);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RuneOfPeriditionPlayer>().runeOfPeridition = true;
        player.GetDamage(DamageClass.Generic) += 10f / 100f;
    }
}

public class RuneOfPeriditionPlayer : ModPlayer
{
    public bool runeOfPeridition { get; set; } = false;
    public override void ResetEffects()
    {
        runeOfPeridition = false;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (runeOfPeridition)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.team == Player.team)
                {
                    player.Heal(player.statLifeMax2 / 3);
                }
            }
        }
    }

    public override void PostUpdateEquips()
    {
        if (runeOfPeridition)
        {
            Player.aggro += 600;
            Player.statDefense *= 0.9f;
        }
    }
}
