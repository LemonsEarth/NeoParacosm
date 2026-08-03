using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Ranged.AscendedShadow;

[AutoloadEquip(EquipType.Legs)]
public class AscendedShadowGreaves : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.ShadowGreaves;
    public override Color Color => Color.Purple;
    static readonly float moveSpeedBoost = 16;
    static readonly float critBoost = 8;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(moveSpeedBoost, critBoost);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 22;
        Item.defense = 5;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += moveSpeedBoost / 100;
        player.jumpSpeedBoost += 1.5f;
        player.GetCritChance(DamageClass.Ranged) += critBoost;
    }
}
