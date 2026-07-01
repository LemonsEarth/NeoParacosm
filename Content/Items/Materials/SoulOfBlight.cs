using NeoParacosm.Core.Conditions;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Materials;

public abstract class SoulOfBlight : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 3));
        ItemID.Sets.AnimatesAsSoul[Type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

        ItemID.Sets.ItemIconPulse[Type] = true; // The item pulses while in the player's inventory
        ItemID.Sets.ItemNoGravity[Type] = true; // Makes the item have no gravity

        Item.ResearchUnlockCount = 100; // Configure the amount of this item that's needed to research it in Journey mode.
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.width = 40;
        Item.height = 40;
        Item.value = Item.sellPrice(0, 0, 5, 0);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.Gold.ToVector3()); // Makes this item glow when thrown out of inventory.
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 255, 255, 50); // Makes this item render at full brightness.
    }
}

public class SoulOfBlightDesert : SoulOfBlight { }
public class SoulOfBlightIce : SoulOfBlight { }
public class SoulOfBlightJungle : SoulOfBlight { }
public class SoulOfBlightUnderworld : SoulOfBlight { }
public class SoulOfBlightSpace : SoulOfBlight { }

public class SoulOfBlightDropNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {

    }

    static void AddSoulOfBlight(ref GlobalLoot globalLoot, Condition biomeCondition, int soulID)
    {
        var dcActiveCondition = new LeadingConditionRule(LemonConditions.DarkCataclysmActive.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied));
        dcActiveCondition.OnSuccess(
                ItemDropRule.ByCondition(
                    biomeCondition.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied),
                    soulID, 3, 1, 3));
        globalLoot.Add(dcActiveCondition);
    }

    public override void ModifyGlobalLoot(GlobalLoot globalLoot)
    {
        AddSoulOfBlight(ref globalLoot, Condition.InDesert, ItemType<SoulOfBlightDesert>());
        AddSoulOfBlight(ref globalLoot, Condition.InSnow, ItemType<SoulOfBlightIce>());
        AddSoulOfBlight(ref globalLoot, Condition.InJungle, ItemType<SoulOfBlightJungle>());
        AddSoulOfBlight(ref globalLoot, Condition.InUnderworld, ItemType<SoulOfBlightUnderworld>());
        AddSoulOfBlight(ref globalLoot, Condition.InSpace, ItemType<SoulOfBlightSpace>());
    }
}
