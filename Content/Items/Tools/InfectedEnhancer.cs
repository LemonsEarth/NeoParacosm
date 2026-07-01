using NeoParacosm.Core.UI.ResearcherUI.Ascension;

namespace NeoParacosm.Content.Items.Tools;

public class InfectedEnhancer : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 28;
        Item.value = Item.sellPrice(0, 5, 0, 0);
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.rare = ItemRarityID.Yellow;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
        {
            return null;
        }

        if (player.ItemTimeIsZero)
        {
            AscensionUISystem ascSystem = GetInstance<AscensionUISystem>();
            if (ascSystem.userInterface.CurrentState == null)
            {
                ascSystem.ShowUI();
            }
            else
            {
                ascSystem.HideUI();
            }
            return true;
        }

        return null;
    }
}
