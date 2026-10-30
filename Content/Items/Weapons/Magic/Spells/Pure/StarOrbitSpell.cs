using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Pure;

public class StarOrbitSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 120;
    public override Vector2 GetTargetVector(Player player) { return player.Center - Vector2.UnitY * 100; }

    public override bool CanCastSpell(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<MagicStar>()] == 0;
    }

    public override void SpellAction(Player player)
    {
        SoundEngine.PlaySound(SoundID.Item28, player.Center);
        if (LemonUtils.NotClient())
        {
            float scaledCount = (player.GetElementalExpertiseBoost(SpellElement.Pure) - 1) * 10;
            float ceilingdCount = MathF.Floor(scaledCount);
            int count = (int)MathF.Max(ceilingdCount, 1);
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Vector2 posOffset = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(45 * j)) * (64 + i * 16);
                    Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center - posOffset,
                    Vector2.Zero,
                    ProjectileType<MagicStar>(),
                    GetDamage(player),
                    1f,
                    player.whoAmI,
                    ai0: 180 + 30 * j + 15 * i,
                    ai1: posOffset.X,
                    ai2: posOffset.Y);
                }
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 24;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Pink;
        SpellElements = [SpellElement.Pure];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MagicStarsSpell>(), 1);
        recipe.AddIngredient(ItemID.SoulofLight, 6);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}