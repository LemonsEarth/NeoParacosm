using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Summoner.AscendedCrimson;

[AutoloadEquip(EquipType.Legs)]
public class AscendedCrimsonGreaves : ModItem
{
    static readonly float moveSpeedBoost = 12;
    static readonly int minionBoost = 1;
    int timer = 0;
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

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.CrimsonGreaves, position, scale, timer, frame, spriteBatch, Color.Yellow);
        return false;
    }


    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.CrimsonGreaves, rotation, scale, timer, spriteBatch, Color.Yellow);
        return false;
    }
}
