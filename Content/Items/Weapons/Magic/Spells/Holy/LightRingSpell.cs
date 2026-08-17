using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class LightRingSpell : BaseSpell
{
    public override int AttackCooldown => 20;
    public override int ManaCost => 12;
    public override Vector2 GetTargetVector(Player player)
    {
        return Main.MouseWorld;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<LightRingProjectile>()] < 5;
    }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SFX.CrystalSerpent with { PitchRange = (0.3f, 0.5f)}, player.Center);
        Projectile.NewProjectileDirect(
            player.GetSource_FromThis(),
            player.Center,
            player.Center.DirectionTo(Main.MouseWorld) * 30,
            ProjectileType<LightRingProjectile>(),
            GetDamage(player),
            6f,
            player.whoAmI,
            ai0: 60,
            ai1: player.GetElementalExpertiseBoostMultiplied(SpellElement.Holy, 4)
            );
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 20;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Holy];
    }
}