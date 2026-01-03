using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Summoner.AscendedCrimson;

[AutoloadEquip(EquipType.Body)]
public class AscendedCrimsonScalemail : ModItem
{
    static readonly float damageBoost = 12;
    static readonly int minionBoost = 1;
    int timer = 0;
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

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.CrimsonScalemail, position, scale, timer, frame, spriteBatch, Color.Yellow);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.CrimsonScalemail, rotation, scale, timer, spriteBatch, Color.Yellow);
        return false;
    }
}
