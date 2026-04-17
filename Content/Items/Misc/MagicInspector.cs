using NeoParacosm.Core.UI.ResearcherUI.Note;
using Terraria.Chat;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Misc;

public class MagicInspector : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 68;
        Item.height = 68;
        Item.value = Item.sellPrice(0, 1, 0, 0);
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.UseSound = SoundID.Item29;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            if (player.altFunctionUse != 2)
            {
                foreach (var kvp in player.NPCatalystPlayer().ElementalDamageBoosts)
                {
                    Main.NewText($"{kvp.Key}: {(int)((kvp.Value - 1) * 100)}%");
                }
            }
            else
            {
                foreach (var kvp in player.NPCatalystPlayer().ElementalExpertiseBoosts)
                {
                    Main.NewText($"{kvp.Key}: {(int)((kvp.Value - 1) * 100)}%");
                }
            }
        }
        return true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
}
