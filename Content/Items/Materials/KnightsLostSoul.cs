using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Materials;

public class KnightsLostSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 4));
        ItemID.Sets.AnimatesAsSoul[Type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

        ItemID.Sets.ItemIconPulse[Type] = false; // The item pulses while in the player's inventory
        ItemID.Sets.ItemNoGravity[Type] = true; // Makes the item have no gravity

        Item.ResearchUnlockCount = 100; // Configure the amount of this item that's needed to research it in Journey mode.
    }

    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 48;
        Item.value = Item.sellPrice(0, 0, 0, 10);
        Item.rare = ItemRarityID.Yellow;
        Item.maxStack = 9999;
    }
}
