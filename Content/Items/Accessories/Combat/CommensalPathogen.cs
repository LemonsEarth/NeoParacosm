using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat
{
    public class CommensalPathogen : ModItem
    {
        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost);
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 4);
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<CrimsonTendrilBuff>(), 2);
        }
    }
}
