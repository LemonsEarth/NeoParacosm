using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Ranged.AscendedShadow;

[AutoloadEquip(EquipType.Body)]
public class AscendedShadowScalemail : ModItem
{
    static readonly float damageBoost = 8;
    static readonly int drBoost = 8;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, drBoost);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 24;
        Item.defense = 8;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Ranged) += damageBoost / 100;
        player.endurance += drBoost / 100f;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.ShadowScalemail, position, scale, frame, spriteBatch, Color.Purple);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.ShadowScalemail, rotation, scale, spriteBatch, Color.Purple);
        return false;
    }
}
