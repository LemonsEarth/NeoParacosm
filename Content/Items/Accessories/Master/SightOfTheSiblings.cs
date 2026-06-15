using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class SightOfTheSiblings : ModItem
{
    float damageBoost = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost);

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
        foreach (var plr in Main.ActivePlayers)
        {
            if (plr.team == player.team)
            {
                plr.GetDamage(DamageClass.Generic) += damageBoost / 100f;
            }
        }

        player.GetModPlayer<SightOfTheSiblingsPlayer>().Active = true;
    }
}

public class SightOfTheSiblingsPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!Active) return;
        foreach (var plr in Main.ActivePlayers)
        {
            if (plr.team == Player.team && plr.whoAmI != Player.whoAmI)
            {
                plr.Hurt(info.DamageSource, info.SourceDamage, info.HitDirection);
            }
        }
    }

    public override void PostUpdateEquips()
    {

    }
}

public class SightOfTheSiblingsDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Spazmatism || entity.type == NPCID.Retinazer;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
        leadingConditionRule.OnSuccess(
            ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<NightOfTheWatcher>()));
        npcLoot.Add(leadingConditionRule);
    }
}
