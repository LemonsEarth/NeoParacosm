using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class VampiresVial : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<VampiresVialPlayer>().Active = true;
    }
}

public class VampiresVialPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int healWindowTimer = 0;
    int damageTaken = 0;
    int amountHealed = 0;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateBadLifeRegen()
    {

    }


    void Lifesteal()
    {
        int amountToHeal = (int)(damageTaken * 0.5f);
        int healPerHit = amountToHeal / 5;
        if (healWindowTimer > 0 && damageTaken > 0 && amountHealed < amountToHeal)
        {
            Player.Heal(healPerHit);
            amountHealed += healPerHit;
        }
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Active) return;
        Lifesteal();
    }


    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Active || !proj.CountsAsTrueMelee()) return;
        Lifesteal();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!Active) return;
        healWindowTimer = 180;
        damageTaken = info.Damage;
        amountHealed = 0;
    }

    public override void PostUpdateEquips()
    {
        if (healWindowTimer > 0)
        {
            healWindowTimer--;
        }
    }
}

public class VampiresVialDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.CaveBat;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<VampiresVial>(), 50));
    }
}
