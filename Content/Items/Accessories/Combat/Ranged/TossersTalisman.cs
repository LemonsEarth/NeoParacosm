using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class TossersTalisman : ModItem
{
    public static float DamageBoost { get; set; } = 12f;
    public static float CritChanceBoost { get; set; } = 8f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost, CritChanceBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<TossersTalismanPlayer>().Active = true;
    }
}

public class TossersTalismanPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        if (Active && item.damage > 0 && item.consumable && item.useStyle != ItemUseStyleID.None)
        {
            damage += TossersTalisman.DamageBoost / 100f;
        }
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        if (Active && item.damage > 0 && item.consumable && item.useStyle != ItemUseStyleID.None)
        {
            crit += TossersTalisman.CritChanceBoost;
        }
    }
}

public class TossersTalismanDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinArcher
            || entity.type == NPCID.GoblinPeon
            || entity.type == NPCID.GoblinThief
            || entity.type == NPCID.GoblinSorcerer
            || entity.type == NPCID.GoblinWarrior;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<TossersTalisman>(), 75, 40));
    }
}
