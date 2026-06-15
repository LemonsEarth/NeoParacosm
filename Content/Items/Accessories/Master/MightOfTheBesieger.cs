using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class MightOfTheBesieger : ModItem
{
    public static float HealthBoost { get; private set; } = 40f;
    public static float DamageTakenBoost { get; private set; } = 50f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealthBoost, DamageTakenBoost);

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Master;
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MightOfTheBesiegerPlayer>().Active = true;
    }
}

public class MightOfTheBesiegerPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (!Active)
        {
            return;
        }

        Player.buffImmune[BuffID.Bleeding] = true;
        Player.buffImmune[BuffID.CursedInferno] = true;
        Player.buffImmune[BuffID.Ichor] = true;
        Player.buffImmune[BuffID.ShadowFlame] = true;
        Player.buffImmune[BuffID.OnFire] = true;
        Player.buffImmune[BuffID.OnFire3] = true;
        Player.buffImmune[BuffID.Poisoned] = true;
        Player.buffImmune[BuffID.Venom] = true;
        Player.buffImmune[BuffID.Frostburn] = true;
        Player.buffImmune[BuffID.Frostburn2] = true;
        Player.buffImmune[BuffID.Electrified] = true;

        Player.statLifeMax2 = (int)(Player.statLifeMax2 * (1 + (MightOfTheBesieger.HealthBoost / 100f)));
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Active)
        {
            modifiers.FinalDamage *= (1 + MightOfTheBesieger.DamageTakenBoost / 100f);
        }
    }
}

public class MightOfTheBesiegerDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.TheDestroyer;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<MightOfTheBesieger>()));
    }
}
