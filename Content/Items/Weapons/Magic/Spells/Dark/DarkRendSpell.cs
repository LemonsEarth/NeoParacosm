using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class DarkRendSpell : BaseSpell
{
    public override int AttackCooldown => 45;
    public override int ManaCost => 15;
    public override Vector2 TargetVector => Main.MouseWorld;

    public override void ShootBehaviour(Player player)
    {
        if (LemonUtils.NotClient())
        {
            for (int i = -2; i <= 2; i++)
            {
                Vector2 baseDirection = player.DirectionTo(TargetVector) * 0.5f;
                Vector2 rotatedDirection = baseDirection.RotatedBy(MathHelper.ToRadians(5) * i);
                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                    rotatedDirection,
                    ProjectileType<DarkBlast>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai1: 3);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 12;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Dark];
    }
}