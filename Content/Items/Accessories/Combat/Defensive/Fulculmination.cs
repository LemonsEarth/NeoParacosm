using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Special;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class Fulculmination : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FulculminationPlayer>().Active = true;
    }
}

public class FulculminationPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && Main.rand.NextBool(8))
        {
            for (int i = -4; i <= 4; i++)
            {
                Vector2 pos = Player.Center + new Vector2(i * 200, -800);
                Vector2 endPos = pos + Vector2.UnitY * 1600;
                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    pos,
                    Vector2.Zero,
                    ProjectileType<YellowLightning>(),
                    100,
                    0f,
                    Player.whoAmI,
                    ai0: 30 * MathF.Abs(i),
                    ai1: endPos.X,
                    ai2: endPos.Y
                    );
            }
            if (Player.HasBuff(BuffType<ElectrifiedBodyBuff>()))
            {
                if (info.DamageSource.TryGetCausingEntity(out var entity) && entity is NPC npc)
                {
                    for (int i = -4; i <= 4; i++)
                    {
                        Vector2 pos = npc.Center + new Vector2(i * 200, -800);
                        Vector2 endPos = pos + Vector2.UnitY * 1600;
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            pos,
                            Vector2.Zero,
                            ProjectileType<YellowLightning>(),
                            100,
                            0f,
                            Player.whoAmI,
                            ai0: 60 * MathF.Abs(i),
                            ai1: endPos.X,
                            ai2: endPos.Y
                            );
                    }
                }
            }
        }
    }
}
