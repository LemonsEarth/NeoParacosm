using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class PebbleStrikeSpell : BaseSpell
{
    public override int AttackCooldown => 10;
    public override int ManaCost => 12;
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
                    dir * Main.rand.NextFloat(14, 16) * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Earth],
                    ProjectileType<Pebble>(),
                    (int)(GetDamage(player) * 0.5f),
                    1f,
                    player.whoAmI);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 6;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Earth];
    }
}