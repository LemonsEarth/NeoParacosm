using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Summoner.AscendedCrimson;

[AutoloadEquip(EquipType.Body)]
public class AscendedCrimsonScalemail : AscendedGlowItem
{
    public override int OriginalItemID => ItemID.CrimsonScalemail;
    public override Color Color => Color.Yellow;
    static readonly float damageBoost = 12;
    static readonly int minionBoost = 1;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, minionBoost);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 24;
        Item.defense = 6;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Summon) += damageBoost / 100;
        player.maxMinions += minionBoost;
    }
}
