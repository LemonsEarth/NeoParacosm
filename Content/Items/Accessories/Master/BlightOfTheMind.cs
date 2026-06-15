using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class BlightOfTheMind : ModItem
{
    public static float DamageBoost { get; private set; } = 10f;
    public static float AttackSpeedBoost { get; private set; } = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost, AttackSpeedBoost);

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
        player.GetModPlayer<BlightOfTheMindPlayer>().Active = true;
    }
}

public class BlightOfTheMindPlayer : ModPlayer
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
            if (Player.HasBuff(BuffID.Confused))
            {
                Player.GetDamage(DamageClass.Generic) += BlightOfTheMind.DamageBoost / 100f;
                Player.GetAttackSpeed(DamageClass.Generic) += BlightOfTheMind.AttackSpeedBoost / 100f;
            }
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active)
        {
            Player.AddBuff(BuffID.Confused, 600);
        }
    }
}

public class BlightOfTheMindDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.BrainofCthulhu;
    }

    public override void OnKill(NPC npc)
    {

    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<BlightOfTheMind>()));
    }
}
