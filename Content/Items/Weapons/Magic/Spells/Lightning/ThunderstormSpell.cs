using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class LightningStrikeSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 24;
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
            Vector2 topPos = Main.MouseWorld - Vector2.UnitY * 800;
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
                ai1: botPos.X,
                ai2: botPos.Y);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 36;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Lightning];
    }
}