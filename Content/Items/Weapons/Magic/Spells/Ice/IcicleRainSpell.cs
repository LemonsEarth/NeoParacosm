using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;

public class IcicleRainSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 50;
    public override Vector2 GetTargetVector(Player player)
    {
        return player.Center - Vector2.UnitY * 100;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<IcicleRainBall>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(
                Item.GetSource_FromAI(),
                player.Center - Vector2.UnitY * 64f,
                Vector2.Zero,
                ProjectileType<IcicleRainBall>(),
                (int)(GetDamage(player)),
                1f,
                player.whoAmI);

        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 24;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Ice];
    }
}

public class IcicleRainSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.IceGolem;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<IcicleRainSpell>(), 5, 1, 1));
    }
}