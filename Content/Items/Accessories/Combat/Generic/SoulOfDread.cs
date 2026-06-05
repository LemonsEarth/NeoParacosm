using System.Threading;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class SoulOfDread : ModItem
{
    public static float maxDamage = 30f;
    public static float maxDuration = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxDamage, maxDuration);
    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SoulOfDreadPlayer>().Active = true;
    }
}

public class SoulOfDreadPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int timer = 0;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void PostUpdate()
    {
        if (Active)
        {
            if (Player.lifeRegen < 0)
            {
                if (timer < SoulOfDread.maxDuration * 60)
                {
                    timer++;
                }
            }
            else
            {
                timer -= 5;
                if (timer < 0) timer = 0;
            }

            float rate = timer / SoulOfDread.maxDamage;
            float dmgBoost = timer / rate;
            Player.GetDamage(DamageClass.Generic) += dmgBoost / 100f;
        }
        else
        {
            timer = 0;
        }
    }

    public override void PostUpdateBuffs()
    {
    }

    public override void PostUpdateEquips()
    {

    }
}
