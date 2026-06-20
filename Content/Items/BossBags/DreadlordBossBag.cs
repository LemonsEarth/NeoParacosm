using NeoParacosm.Content.Items.Accessories.Combat.Generic;
using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.BossBags;

public class DreadlordBossBag : ModItem
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
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<SoulOfDread>()));
        itemLoot.Add(ItemDropRule.Common(ItemType<NightmareScale>(), 1, 24, 40));
        itemLoot.Add(ItemDropRule.Common(ItemType<DivineFlesh>(), 1, 24, 40));
    }
}
