using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class CrystalBurstSpell : BaseSpell
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
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = player.DirectionTo(Main.MouseWorld).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8));
                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                    dir * Main.rand.NextFloat(6, 8) * player.GetElementalDamageBoost(SpellElement.Earth),
                    ProjectileType<SplittingCrystal>(),
                    (int)(GetDamage(player) * 0.5f),
                    1f,
                    player.whoAmI,
                    ai1: 1 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Pure, 2),
                    ai2: 15 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 4)
                    );
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 24;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Earth, SpellElement.Pure];
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<PebbleStrikeSpell>())
            .AddIngredient(ItemID.CrystalShard, 15)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}