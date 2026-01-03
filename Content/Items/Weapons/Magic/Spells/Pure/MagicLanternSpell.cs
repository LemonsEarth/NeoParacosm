using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Pure;

public class MagicLanternSpell : BaseSpell
{
    public override int AttackCooldown => 45;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        SoundEngine.PlaySound(SoundID.Item28, player.Center);
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.Center.DirectionTo(Main.MouseWorld) * 10,
                ProjectileType<MagicLantern>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai1: 1500 * player.GetElementalExpertiseBoost(SpellElement.Pure));
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 2);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Pure];
    }
}