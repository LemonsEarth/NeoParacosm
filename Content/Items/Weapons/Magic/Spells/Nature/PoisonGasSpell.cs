using NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;

public class PoisonGasSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 30;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (1.8f, 2f) }, player.Center);
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (0.8f, 1f) }, player.Center);

        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 10 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature],
                ProjectileType<PoisonGasProj>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai1: 20 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature]);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 18;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Nature];
    }
}

public class PoisonGasSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.MossHornet;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemType<PoisonGasSpell>(), 50));
    }
}