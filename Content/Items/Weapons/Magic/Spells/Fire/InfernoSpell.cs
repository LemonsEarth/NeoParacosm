using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class InfernoSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 30;
     public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override void SpellAction(Player player)
    {
        
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 1f, PitchRange = (0.8f, 1f) }, player.Center);

        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 10 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire],
                ProjectileType<InfernoProj>(),
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
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Fire];
    }
}

public class InfernoSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Demon || entity.type == NPCID.VoodooDemon;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<InfernoSpell>(), 50));
    }
}

public class InfernoSpellChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<InfernoSpell>(), 3, 4, true);
    }
}