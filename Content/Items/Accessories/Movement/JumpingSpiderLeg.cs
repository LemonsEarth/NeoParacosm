using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class JumpingSpiderLeg : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<JumpingSpiderLegPlayer>().Active = true;
        player.jumpSpeedBoost += 10;
    }
}

public class JumpingSpiderLegPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void Load()
    {
        On_Player.StickyMovement += On_Player_StickyMovement;
    }

    private static void On_Player_StickyMovement(On_Player.orig_StickyMovement orig, Player self)
    {
        if (!self.GetModPlayer<JumpingSpiderLegPlayer>().Active)
        {
            orig(self);
        }
    }
}
