using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat
{
    public class CommensalPathogen : ModItem
    {
        readonly float enduranceDecrease = 10;
        readonly float defenseDecrease = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(enduranceDecrease, defenseDecrease);
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 4);
            Item.rare = ItemRarityID.Orange;
            Item.lifeRegen = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffType<CrimsonTendrilBuff>(), 2);
            player.endurance -= enduranceDecrease / 100;
            player.statDefense *= 0.9f;
        }
    }
}
