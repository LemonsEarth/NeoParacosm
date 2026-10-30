using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Pure;

public class MagicStarsSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 120;
    public override Vector2 GetTargetVector(Player player) { return player.Center - Vector2.UnitY * 100; }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<MagicStar>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SoundID.Item28, player.Center);
        if (LemonUtils.NotClient())
        {
            float scaledCount = (player.GetElementalExpertiseBoost(SpellElement.Pure) - 1) * 10;
            float ceilingdCount = MathF.Floor(scaledCount);
            int count = (int)MathF.Max(ceilingdCount, 1);
            for (int i = 0; i < count; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2 posOffset = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(45 * j)) * (64 + i * 16);
                    Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - posOffset,
                    Vector2.Zero,
                    ProjectileType<MagicStar>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 180 + 30 * j + i * 15,
                    ai1: posOffset.X,
                    ai2: posOffset.Y);
                }
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 14;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Pure];
    }
}

public class MagicStarsSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Tim;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.NormalvsExpert(ItemType<MagicStarsSpell>(), 2, 1));
    }
}