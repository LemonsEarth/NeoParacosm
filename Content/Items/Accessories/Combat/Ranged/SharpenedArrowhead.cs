using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class SharpenedArrowhead : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 5, 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SharpenedArrowheadPlayer>().Active = true;
    }
}

public class SharpenedArrowheadPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.type == ProjectileID.WoodenArrowFriendly)
        {
            modifiers.ArmorPenetration += 10;
        }
    }
}

public class SharpenedArrowheadShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinTinkerer;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<SharpenedArrowhead>(), Condition.DownedGoblinArmy);
    }
}
