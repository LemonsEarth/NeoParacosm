using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class GravityFieldSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 100;
    public override Vector2 GetTargetVector(Player player)
    {
        return Main.MouseWorld;
    }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<GravityField>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        if (LemonUtils.NotClient())
        {
            SoundEngine.PlaySound(SoundID.Item7, player.Center);

            Vector2 pos = Main.MouseWorld;
            Projectile.NewProjectile(
                Item.GetSource_FromAI(),
                pos,
                Vector2.Zero,
                ProjectileType<GravityField>(),
                0,
                1f,
                player.whoAmI,
                ai0: 180 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Pure, 2f),
                ai1: 1f * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f),
                ai2: 2f
                );

        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Earth, SpellElement.Pure];
    }
}