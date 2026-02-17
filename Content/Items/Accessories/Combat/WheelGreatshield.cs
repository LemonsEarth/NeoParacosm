using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Content.Projectiles.Hostile.Death;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class WheelGreatshield : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
        Item.defense = 3;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<WheelGreatshieldPlayer>().wheelGreatshield = true;
        player.moveSpeed += 0.1f;
    }
}

public class WheelGreatshieldPlayer : ModPlayer
{
    public bool wheelGreatshield { get; set; } = false;
    public override void ResetEffects()
    {
        wheelGreatshield = false;
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        if (wheelGreatshield)
        {
            modifiers.Knockback *= 0f;
        }
    }

}
