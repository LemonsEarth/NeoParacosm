using Terraria.Chat;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using static NeoParacosm.Core.LocalizationReferences.Mods.NeoParacosm.Items.SoulOfDread;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class SoulOfDread : ModItem
{
    public static float maxDamage = 30f;
    public static float maxDuration = 20f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(maxDamage, maxDuration);
    public override void SetDefaults()
    {
        Item.width = 58;
        Item.height = 60;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }

    public override bool? UseItem(Player player)
    {
        if (player.ItemAnimationJustStarted)
        {
            
            if (LemonUtils.NotClient())
            {
                if (WorldGen.AllowedToSpreadInfections)
                {
                    ChatHelper.BroadcastChatMessage(this.GetLocalization("InfectionSpreadEnabled").ToNetworkText(), Color.Purple);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(this.GetLocalization("InfectionSpreadDisabled").ToNetworkText(), Color.Purple);
                }
            }
        }
        return null;
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
    bool hasDamagingDebuff = false;
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
                hasDamagingDebuff = true;
            }
            else
            {
                hasDamagingDebuff = false;
            }
        }
        else
        {
            hasDamagingDebuff = false;
        }
    }

    public override void UpdateEquips()
    {
        if (Active)
        {
            if (hasDamagingDebuff)
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
            float dmgBoost = MathHelper.Lerp(0, SoulOfDread.maxDamage, (float)timer / (SoulOfDread.maxDuration * 60f));

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
