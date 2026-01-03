using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class ThunderstormSpell : BaseSpell
{
    public override int AttackCooldown => 120;
    public override int ManaCost => 180;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        float rainBoost = 1f;
        if (Main.raining)
        {
            rainBoost += 0.2f;
        }
        if (LemonUtils.NotClient())
        {
            int toMouse = MathF.Sign(player.Center.DirectionTo(Main.MouseWorld).X);
            if (toMouse == 0)
            {
                toMouse = 1;
            }

            float distance = 64;
            for (int i = 0; i < 6; i++)
            {
                Vector2 topPos = player.Center + new Vector2(distance * toMouse, -800);
                Vector2 botPos = topPos;
                Point botPosTile = botPos.ToTileCoordinates();
                while (WorldGen.InWorld(botPosTile.X, botPosTile.Y) && !WorldGen.SolidTile(botPosTile.X, botPosTile.Y))
                {
                    if (MathF.Abs(botPosTile.ToWorldCoordinates().Y - botPos.Y) > 2000)
                    {
                        break;
                    }
                    botPosTile.Y += 1;
                }
                botPos = botPosTile.ToWorldCoordinates();
                Projectile.NewProjectile(Item.GetSource_FromAI(), topPos,
                    Vector2.Zero,
                    ProjectileType<YellowLightning>(),
                    (int)(GetDamage(player) * rainBoost),
                    1f,
                    player.whoAmI,
                    ai0: 5 * i,
                    ai1: botPos.X,
                    ai2: botPos.Y);
                distance *= 2;
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 114;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Lightning];
    }
}