using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class SecretsOfTheGoblins : ModItem
{
    readonly float magicCritBoost = 8f;
    readonly float darkExpertiseBoost = 10f;
    readonly int defenseReduction = 5;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicCritBoost, darkExpertiseBoost, defenseReduction);
    public override void SetDefaults()
    {
        Item.width = 42;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetCritChance(DamageClass.Magic) += magicCritBoost;
        player.AddElementalExpertiseBoost(SpellElement.Dark, darkExpertiseBoost / 100f);
        player.statDefense -= defenseReduction;
        player.GetModPlayer<SecretsOfTheGoblinsPlayer>().Active = true;
    }
}

public class SecretsOfTheGoblinsPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic) && hit.Crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 120);
        }
    }
}

public class SecretsOfTheGoblinsDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinSorcerer;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<SecretsOfTheGoblins>(), 50, 1, 1));
    }
}
