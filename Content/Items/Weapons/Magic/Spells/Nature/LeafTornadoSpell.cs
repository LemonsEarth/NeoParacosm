using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;

public class LeafTornadoSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 30;
    public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (0.2f, 0.3f) }, player.Center);
        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.7f, PitchRange = (-0.3f, -0.1f) }, player.Center);

        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 4 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature],
                ProjectileType<LeafTornado>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai0: 180 * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Nature],
                ai1: 10
                );
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
