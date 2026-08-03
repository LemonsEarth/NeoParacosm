using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Summoner.AscendedCrimson;

[AutoloadEquip(EquipType.Legs)]
public class AscendedCrimsonGreaves : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.CrimsonGreaves;
    public override Color Color => Color.Yellow;
    static readonly float moveSpeedBoost = 12;
    static readonly int minionBoost = 1;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(moveSpeedBoost, minionBoost);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 22;
        Item.defense = 4;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += moveSpeedBoost / 100;
        player.jumpSpeedBoost += 1;
        player.maxMinions += minionBoost;
    }
}
