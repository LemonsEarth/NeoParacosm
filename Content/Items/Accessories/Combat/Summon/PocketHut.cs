using NeoParacosm.Content.NPCs.Friendly.Special;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class PocketHut : ModItem
{
    int timer = 0;
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (timer % 600 == 0)
        {
            if (LemonUtils.NotClient())
            {
                NPC.NewNPCDirect(
                    player.GetSource_Accessory(Item), player.Center, NPCType<FriendlyGoblinMinion>(), ai0: 600, ai2: player.whoAmI);
            }
        }
        timer++;
    }
}