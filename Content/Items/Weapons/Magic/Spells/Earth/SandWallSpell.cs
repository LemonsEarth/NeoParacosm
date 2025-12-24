using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class SandWallSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 24;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            SoundEngine.PlaySound(SoundID.Item7, player.Center);
            for (int i = -3; i <= 3; i++)
            {
                Vector2 pos = (Main.MouseWorld / 16) + (Vector2.UnitY * i);
                Projectile.NewProjectile(Item.GetSource_FromAI(), pos * 16,
                    Vector2.Zero,
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