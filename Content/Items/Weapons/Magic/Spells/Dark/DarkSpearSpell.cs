using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class DarkSpearSpell : BaseSpell
{
    public override int AttackCooldown => 45;
    public override int ManaCost => 25;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Vector2 vel = player.DirectionTo(TargetVector) * 20;
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                vel,
                ProjectileType<DarkSpear>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 25;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Dark];
    }
}