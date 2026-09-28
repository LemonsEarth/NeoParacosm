using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class TheDarkOfNightSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 100;
    public override Vector2 GetTargetVector(Player player) { return Main.MouseWorld; }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(ParacosmSFX.DarkWhoosh with { PitchRange = (-0.2f, 0.2f) }, player.Center);
        float attackInterval = MathHelper.Lerp(15, 5, MathHelper.Clamp((player.GetElementalDamageBoost(SpellElement.Dark) - 1f) / 0.5f, 0, 1f));
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(
                Item.GetSource_FromAI(), player.Center,
                Vector2.Zero,
                ProjectileType<TheDarkOfNightProjectile>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 2f), attackInterval);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 50;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Yellow;
        SpellElements = [SpellElement.Dark];
    }
}