using NeoParacosm.Content.Items.Accessories.Combat.Melee;
using NeoParacosm.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class NutcrackersHead : ModItem
{
    public static float DamageBoost { get; private set; } = 15f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost);
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<NutcrackersHeadPlayer>().Active = true;
    }
}

public class NutcrackersHeadPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}

public class NutcrackersHeadItem : GlobalItem
{
    public override void OnConsumeItem(Item item, Player player)
    {
        if (player.GetModPlayer<NutcrackersHeadPlayer>().Active &&
            ItemID.Sets.IsFood[item.type] &&
            item.buffType == BuffID.WellFed || item.buffType == BuffID.WellFed2 || item.buffType == BuffID.WellFed3)
        {
            player.AddBuff(BuffType<NutcrackersHeadBuff>(), 300);
        }
    }
}

public class NutcrackersHeadBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.GetDamage(DamageClass.Generic) += NutcrackersHead.DamageBoost / 100f;
        player.statDefense *= 0.8f;
    }
}

public class NutcrackersHeadNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Nutcracker || entity.type == NPCID.NutcrackerSpinning;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<NutcrackersHead>(), 30));
    }
}