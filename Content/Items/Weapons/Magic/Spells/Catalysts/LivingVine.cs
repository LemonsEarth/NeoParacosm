using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class LivingVine : BaseCatalyst
{
    static readonly float magicDamageBoost = 10;
    static readonly float magicSpeedBoost = 15;
    static readonly float natureDamageBoost = 20;
    static readonly float natureSpeedBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, magicSpeedBoost, natureDamageBoost, natureSpeedBoost);

    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 24;
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.mana = 15;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Pure] += magicDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Pure] += magicSpeedBoost / 100f;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Nature] += natureDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature] += natureSpeedBoost / 100f;
        }
    }
}

public class DropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Plantera;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<LivingVine>(), 5, 1, 1));
    }
}