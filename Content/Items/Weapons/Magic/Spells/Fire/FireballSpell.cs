using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class FireballSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 12;
    public override Vector2 TargetVector => Main.MouseWorld;
    
    public override void ShootBehaviour(Player player)
    {
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                player.DirectionTo(Main.MouseWorld) * 5 - Vector2.UnitY * 2, 
                ProjectileType<Fireball>(), 
                GetDamage(player), 
                1f, 
                player.whoAmI);
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
        SpellElements = [SpellElement.Fire];
    }
}