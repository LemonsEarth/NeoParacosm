using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class StoneHeart : ModItem
{
    public static int LifeBoost { get; set; } = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeBoost);

    public override void SetDefaults()
    {
        Item.width = 42;
        Item.height = 56;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.statLifeMax2 += LifeBoost;
        int extraReduceTime = 2;
        for (int i = 0; i < player.buffType.Length; i++)
        {
            int buffID = player.buffType[i];
            if (buffID == BuffID.Poisoned || buffID == BuffID.OnFire)
            {
                player.buffTime[i] -= extraReduceTime;
            }
        }
    }
}

public class StoneHeartDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GraniteGolem;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<StoneHeart>(), 15, 1, 1));
    }
}
