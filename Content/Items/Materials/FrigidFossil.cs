using NeoParacosm.Core.UI.ResearcherUI.Note;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Materials;

public class FrigidFossil : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.value = Item.sellPrice(0, 0, 0, 70);
        Item.rare = ItemRarityID.Blue;
        Item.maxStack = 9999;
    }
}

public class FrigidFossilDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.UndeadViking || entity.type == NPCID.ArmoredViking;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<FrigidFossil>(), 2, 2, 6));
    }
}
