namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class IchorSyringe : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 56;
        Item.height = 58;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<IchorSyringePlayer>().Active = true;
    }
}

public class IchorSyringePlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && hit.Crit)
        {
            target.AddBuff(BuffID.Ichor, 10 * 60);
            Player.AddBuff(BuffID.Ichor, 7 * 60);
        }
    }
}
