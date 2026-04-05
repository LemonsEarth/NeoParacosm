using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class BlightningStrikeSpell : BaseSpell
{
    public override int AttackCooldown => 90;
    public override int ManaCost => 90;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        float rainBoost = 1f;
        if (Main.raining)
        {
            rainBoost += 0.2f;
        }
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                        ProjectileType<RedLightning>(), (int)(GetDamage(player) * rainBoost), 6f, player.whoAmI,
                        ai1: Main.MouseWorld.X,
                        ai2: Main.MouseWorld.Y);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 114;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Lightning];
    }
}