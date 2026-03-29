using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Pure;

public class MagicStarsSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 120;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        SoundEngine.PlaySound(SoundID.Item28, player.Center);
        if (LemonUtils.NotClient())
        {
            Vector2 offposC = Vector2.UnitY * 64;
            Vector2 offposL = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(-30)) * 64;
            Vector2 offposR = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(30)) * 64;
            float scaledCount = (player.GetElementalExpertiseBoost(SpellElement.Pure) - 1) * 10;
            float ceilingdCount = MathF.Ceiling(scaledCount);
            int count = (int)MathF.Max(ceilingdCount, 1);
            for (int i = 0; i < count; i++)
            {

                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposC,
                    Vector2.Zero,
                    ProjectileType<MagicStar>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 180 + 60 * i,
                    ai1: offposC.X,
                    ai2: offposC.Y);

                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposL,
                    Vector2.Zero,
                    ProjectileType<MagicStar>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 90 + 60 * i,
                    ai1: offposL.X,
                    ai2: offposL.Y);

                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposR,
                    Vector2.Zero,
                    ProjectileType<MagicStar>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 135 + 60 * i,
                    ai1: offposR.X,
                    ai2: offposR.Y);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 14;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Pure];
    }
}