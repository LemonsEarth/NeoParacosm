using NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class DragonBreathSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 12;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<DragonHeadHeldProj>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                Vector2.Zero,
                ProjectileType<DragonHeadHeldProj>(),
                GetDamage(player),
                1f,
                player.whoAmI);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 10;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Fire];
    }
}