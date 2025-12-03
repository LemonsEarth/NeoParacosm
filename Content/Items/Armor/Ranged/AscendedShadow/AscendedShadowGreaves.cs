using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Ranged.AscendedShadow;

[AutoloadEquip(EquipType.Legs)]
public class AscendedShadowGreaves : ModItem
{
    static readonly float moveSpeedBoost = 16;
    static readonly float critBoost = 8;
    int timer = 0;
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

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.ShadowGreaves, position, scale, timer, frame, spriteBatch, Color.Purple);
        return false;
    }


    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.ShadowGreaves, rotation, scale, timer, spriteBatch, Color.Purple);
        return false;
    }
}
