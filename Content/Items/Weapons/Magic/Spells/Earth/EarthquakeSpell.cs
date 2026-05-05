using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class EarthquakeSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 30;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;

        float xSlowDownBase = MathF.Pow(0.5f, player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f));
        float yPushBase = 25 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f);
        float baseDistance = 400 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f);
        int count = 0;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.life > 0 && !npc.friendly && npc.collideY && npc.Distance(player.Center) < baseDistance)
            {
                if (count < 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(
                            player.GetSource_FromThis(),
                            npc.Bottom,
                            -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f)) * Main.rand.NextFloat(5, 10),
                            ProjectileType<Pebble>(),
                            ((int)(GetDamage(player) * 0.4f)),
                            1f,
                            player.whoAmI
                            );
                    }
                    count++;
                }

                if (!npc.boss)
                {
                    npc.velocity.X *= xSlowDownBase * (1 - npc.knockBackResist);
                }
                npc.velocity.Y -= yPushBase * npc.knockBackResist;
                npc.SimpleStrikeNPC((int)(GetDamage(player) * npc.knockBackResist), 1);
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(npc.Bottom + Vector2.UnitX * i * 5, 1, 1, DustID.Dirt, 0, -5, Scale: 2).noGravity = true;
                }
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 45;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Earth];
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<UnearthSpell>(), 1);
        recipe.AddIngredient(ItemID.HellstoneBar, 12);
        recipe.AddTile(TileID.Bookcases);
        recipe.Register();
    }
}