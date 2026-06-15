using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class NightOfTheWatcher : ModItem
{
    public static int MinionBoost { get; private set; } = 2;
    float damageBoost = 5f;
    float moveSpeedBoost = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionBoost, damageBoost, moveSpeedBoost);

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Master;
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.GetLifePercent() > 0.5f)
        {
            player.GetModPlayer<NightOfTheWatcherPlayer>().Active = true;
        }
        else
        {
            player.moveSpeed += moveSpeedBoost / 100f;
            player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        }
    }
}

public class NightOfTheWatcherPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void PostUpdateEquips()
    {
        if (Active)
        {
            if (Player.maxMinions <= 1)
            {
                Player.maxMinions += NightOfTheWatcher.MinionBoost;
            }
        }
    }
}

public class NightOfTheWatcherDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.EyeofCthulhu;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<NightOfTheWatcher>()));
    }
}
