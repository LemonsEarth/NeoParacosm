using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

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
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposC,
                Vector2.Zero,
                ProjectileType<MagicStar>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai0: 180,
                ai1: offposC.X,
                ai2: offposC.Y);

            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposL,
                Vector2.Zero,
                ProjectileType<MagicStar>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai0: 90,
                ai1: offposL.X,
                ai2: offposL.Y);

            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - offposR,
                Vector2.Zero,
                ProjectileType<MagicStar>(),
                GetDamage(player),
                1f,
                player.whoAmI,
                ai0: 135,
                ai1: offposR.X,
                ai2: offposR.Y);
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