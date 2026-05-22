using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class GingerbreadsWill : ModItem
{
    float speedBoost = 33f;
    float attackSpeedBoost = 10f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(speedBoost, attackSpeedBoost);
    public override void SetDefaults()
    {
        Item.width = 42;
        Item.height = 58;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.HasBuff(BuffID.WellFed) && !player.HasBuff(BuffID.WellFed2) && !player.HasBuff(BuffID.WellFed3))
        {
            player.moveSpeed += speedBoost / 100f;
            player.GetAttackSpeed(DamageClass.Generic) += attackSpeedBoost / 100f;
        }
    }
}

public class GingerbreadsWillDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GingerbreadMan;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<GingerbreadsWill>(), 100));

    }
}
