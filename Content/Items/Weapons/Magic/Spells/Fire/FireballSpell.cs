using NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class FireballSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 12;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 5 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire] - Vector2.UnitY * 2,
                ProjectileType<Fireball>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 18;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Fire];
    }
}

public class FireballSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Demon || entity.type == NPCID.VoodooDemon;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<FireballSpell>(), 50, 1, 1));
    }
}