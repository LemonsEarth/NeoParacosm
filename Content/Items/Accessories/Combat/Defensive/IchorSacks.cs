using NeoParacosm.Content.NPCs.Hostile.Crimson;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class IchorSacks : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        //player.statDefense -= 10;
        player.DefenseEffectiveness *= 0.5f;
        player.GetModPlayer<IchorSacksPlayer>().Active = true;
    }
}

public class IchorSacksPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (Active)
        {
            npc.AddBuff(BuffID.Ichor, 600);
        }
    }
}

public class IchorSacksDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCType<CrimsonWalker>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<IchorSacks>(), 20));

    }
}
