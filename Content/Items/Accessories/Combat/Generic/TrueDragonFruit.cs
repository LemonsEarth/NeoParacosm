using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class TrueDragonFruit : ModItem
{
    readonly float damageBoost = 10f;
    readonly float critBoost = 10f;
    readonly float drBoost = 5f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost, drBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 56;
        Item.defense = 8;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 10);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<TrueDragonFruitPlayer>().Active = true;
        player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        player.GetCritChance(DamageClass.Generic) += critBoost;
        player.endurance += drBoost / 100f;
        player.AddBuff(BuffID.OnFire3,2);
    }
}

public class TrueDragonFruitPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (Active)
        {
            
            //Player.width = (int)(Player.defaultWidth * 1.5f);
            //Player.height = (int)(Player.defaultHeight * 1.5f);
        }
    }
}
