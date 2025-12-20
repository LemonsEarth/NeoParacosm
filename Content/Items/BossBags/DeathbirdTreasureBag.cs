using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.BossBags;

public class DeathbirdTreasureBag : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Item.type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Item.type] = true;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 44;
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
        itemLoot.Add(ItemDropRule.OneFromOptions(1, ItemType<Gravesword>(), ItemType<DeathTolls>(), ItemType<HeadstoneRing>(), ItemType<LamentOfTheLate>(), ItemType<StarecrowStaff>()));
        itemLoot.Add(ItemDropRule.Common(ItemType<RuneOfExtinction>()));
    }
}
