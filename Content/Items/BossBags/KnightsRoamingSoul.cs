using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Accessories.Movement;
using NeoParacosm.Content.Items.Armor.Generic.DeathKnight;
using NeoParacosm.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.BossBags;

public class KnightsRoamingSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Item.type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Item.type] = false;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 38;
        Item.height = 28;
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<CaptainsStormGear>()));
        itemLoot.Add(ItemDropRule.FewFromOptions(2, 1, ItemType<DeathKnightHelmet>(), ItemType<DeathKnightChestplate>(), ItemType<DeathKnightGreaves>()));
    }
}
