using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class BlightOfTheDevourer : ModItem
{
    public static float MiningSpeedBoost { get; private set; } = 30;
    public static float PickaxeSpeedBoost { get; private set; } = 50f;
    public static int DefenseBoost { get; private set; } = 5;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MiningSpeedBoost, PickaxeSpeedBoost, DefenseBoost);

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
        player.GetModPlayer<BlightOfTheDevourerPlayer>().Active = true;
    }
}

public class BlightOfTheDevourerPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (Active)
        {
            Player.pickSpeed -= BlightOfTheDevourer.MiningSpeedBoost / 100f;
            if (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight)
            {
                Player.statDefense += BlightOfTheDevourer.DefenseBoost;
            }
        }
    }


    public override float UseSpeedMultiplier(Item item)
    {
        return Active && item.pick > 0 ? (1 + (BlightOfTheDevourer.PickaxeSpeedBoost / 100f)) : 1f;
    }
}

public class BlightOfTheDevourerDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return (entity.type == NPCID.EaterofWorldsBody || entity.type == NPCID.EaterofWorldsHead || entity.type == NPCID.EaterofWorldsTail);
    }

    public override void OnKill(NPC npc)
    {

    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
        lastEater.OnSuccess(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<BlightOfTheDevourer>()));
        npcLoot.Add(lastEater);
    }
}
