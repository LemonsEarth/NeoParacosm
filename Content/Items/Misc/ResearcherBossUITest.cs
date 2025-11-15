using NeoParacosm.Core.UI.ResearcherUI.Boss;
using NeoParacosm.Core.UI.ResearcherUI.Note;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Misc;

/*public class ResearcherBossUITest : ModItem
{
    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(999999, 2, false));
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.value = Item.buyPrice(0, 0, 0, 0);
        Item.useTime = 1;
        Item.useAnimation = 1;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.rare = ItemRarityID.White;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
        {
            return null;
        }
        ResearcherBossUISystem noteSystem = GetInstance<ResearcherBossUISystem>();
        if (noteSystem.userInterface.CurrentState == null)
        {
            noteSystem.ShowUI();
            
        }
        else
        {
            noteSystem.HideUI();
        }

        return null;
    }
}*/
