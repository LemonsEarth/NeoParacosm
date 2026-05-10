using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.Void;

[AutoloadEquip(EquipType.Head)]
public class VoidHelmet : ModItem
{
    static readonly float damageBoost = 3;
    static readonly float critChanceBoost = 3;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critChanceBoost);

    public static LocalizedText setBonusText;

    public override void SetStaticDefaults()
    {
        setBonusText = this.GetLocalization("SetBonus");
        ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        //Item.defense = 9;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 3, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        //player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        //player.GetCritChance(DamageClass.Generic) += critChanceBoost;

    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        color = Color.White;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<VoidChestplate>() && legs.type == ItemType<VoidGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        //player.setBonus = setBonusText.Value;
        //player.statLifeMax2 /= 2;
        //player.GetModPlayer<DeathKnightArmorPlayer>().Active = true;
    }

    public override void AddRecipes()
    {

    }
}
