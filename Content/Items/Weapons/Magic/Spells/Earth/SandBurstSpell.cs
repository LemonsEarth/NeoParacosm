using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class SandBurstSpell : BaseSpell
{
    public override int AttackCooldown => 24;
    public override int ManaCost => 18;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            SoundEngine.PlaySound(SoundID.Item7, player.Center);
            for (int i = 0; i < 6; i++)
            {
                Vector2 dir = player.DirectionTo(Main.MouseWorld).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi/8, MathHelper.Pi/8));
                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                    dir * Main.rand.NextFloat(6, 7) * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 3) - Vector2.UnitY * 3,
                    ProjectileID.SandBallGun,
                    (int)(GetDamage(player) * 0.5f),
                    1f,
                    player.whoAmI,
                    ai0: 2);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Earth];
    }
}