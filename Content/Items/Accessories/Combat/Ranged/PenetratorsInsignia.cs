using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class PenetratorsInsignia : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<PenetratorsInsigniaPlayer>().Active = true;
    }
}

public class PenetratorsInsigniaPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsClass(DamageClass.Ranged))
        {
            modifiers.ArmorPenetration += MathF.Min((int)(proj.velocity.Length() * 0.5f), 30);
        }
    }
}
