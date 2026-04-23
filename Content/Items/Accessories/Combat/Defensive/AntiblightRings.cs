using NeoParacosm.Content.NPCs.Hostile.Corruption;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class AntiblightRings : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 62;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 10);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        int extraReduceTime = 2;
        for (int i = 0; i < player.buffType.Length; i++)
        {
            int buffID = player.buffType[i];
            if (buffID == BuffID.CursedInferno || buffID == BuffID.Ichor)
            {
                player.buffTime[i] -= extraReduceTime;
            }
        }
    }
}
