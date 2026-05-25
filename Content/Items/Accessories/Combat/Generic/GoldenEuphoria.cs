using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class GoldenEuphoria : ModItem
{
    public static float DMGBoost = 12f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DMGBoost);
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 4);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<GoldenEuphoriaPlayer>().Active = true;
    }
}

public class GoldenEuphoriaPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && target.HasBuff(BuffID.Ichor))
        {
            modifiers.FinalDamage *= 1 + (GoldenEuphoria.DMGBoost / 100f);
        }
    }
}

public class GoldenEuphoriaDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.BigMimicCrimson;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<GoldenEuphoria>(), 5));
    }
}
