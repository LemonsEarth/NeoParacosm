using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat
{
    [AutoloadEquip(EquipType.Back)]
    public class BaneflyHive : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 4);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffType<BaneflyHiveBuff>(), 2);
        }
    }
}
