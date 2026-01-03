using NeoParacosm.Content.Projectiles.Friendly.Magic;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;

public class LeafSpell : BaseSpell
{
    public override int AttackCooldown => 45;
    public override int ManaCost => 15;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            int projectileCount = player.GetElementalExpertiseBoost(SpellElement.Nature) >= 1.3f ? 5 : 3;
            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 pos = player.Center + Main.rand.NextVector2Circular(80, 80);
                Projectile.NewProjectile(Item.GetSource_FromAI(), pos,
                    -Vector2.UnitY * 8,
                    ProjectileType<LeafProjectile>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 90,
                    ai1: 8f,
                    ai2: 1.025f);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 6;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Nature];
    }
}