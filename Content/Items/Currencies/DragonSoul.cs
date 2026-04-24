using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;

namespace NeoParacosm.Content.Items.Currencies;

public class DragonSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 4));
        ItemID.Sets.AnimatesAsSoul[Type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

        ItemID.Sets.ItemIconPulse[Type] = true; // The item pulses while in the player's inventory
        ItemID.Sets.ItemNoGravity[Type] = true; // Makes the item have no gravity

        Item.ResearchUnlockCount = 100; // Configure the amount of this item that's needed to research it in Journey mode.
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.width = 52;
        Item.height = 52;
        Item.value = Item.sellPrice(0, 0, 0, 0);
        Item.rare = ItemRarityID.Pink;
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

public class DragonSoulCurrency : CustomCurrencySingleCoin
{
    public DragonSoulCurrency(int coinItemId, long currencyCap, string currencyTextKey, Color currencyTextColor) : base(coinItemId, currencyCap)
    {
        CurrencyTextKey = currencyTextKey;
        CurrencyTextColor = currencyTextColor;
    }
}

public class DragonSoulCurrencySystem : ModSystem
{
    public static int DragonSoulCurrency { get; set; }

    public override void PostSetupContent()
    {
        DragonSoulCurrency = CustomCurrencyManager.RegisterCurrency(new DragonSoulCurrency(
            coinItemId: ItemType<DragonSoul>(),
            currencyCap: 999,

            currencyTextKey: "Mods.NeoParacosm.Items.DragonSoul.CurrencyDisplayName",
            currencyTextColor: Color.Gold
        ));
    }
}

public class DragonSoulDropNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        void AddDragonSoul(int denominator, int min, int max)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<DragonSoul>(), denominator, min, max));
        }

        switch (npc.type)
        {
            case NPCID.WyvernHead:
                AddDragonSoul(1, 2, 5);
                break;
            case NPCID.FlyingSnake:
                AddDragonSoul(10, 1, 2);
                break;
            case NPCID.Lihzahrd:
                AddDragonSoul(10, 1, 2);
                break;
            case NPCID.LihzahrdCrawler:
                AddDragonSoul(20, 1, 2);
                break;
            case NPCID.DD2Betsy:
                AddDragonSoul(1, 20, 25);
                break;
        }
    }
}
