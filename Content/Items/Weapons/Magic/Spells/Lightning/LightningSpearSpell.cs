using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class LightningSpearSpell : BaseSpell
{
    public override int AttackCooldown => 20;
    public override int ManaCost => 25;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Vector2 vel = player.DirectionTo(TargetVector) * 20;
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                vel,
                ProjectileType<LightningSpear>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 30;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Lightning];
    }
}

public class LightningSpearSpellChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<LightningSpearSpell>(), 13, 10, true);
    }
}


public class LightningSpearSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Harpy;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.DownedEowOrBoc.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied), ItemType<LightningSpearSpell>(), 50));
    }
}