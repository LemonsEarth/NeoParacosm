using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class TrueDragonFruit : ModItem
{
    readonly float damageBoost = 10f;
    readonly float critBoost = 10f;
    readonly float drBoost = 5f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost, drBoost);
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 56;
        Item.defense = 8;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 10);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<TrueDragonFruitPlayer>().Active = true;
        player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        player.GetCritChance(DamageClass.Generic) += critBoost;
        player.endurance += drBoost / 100f;
        //player.AddBuff(BuffID.OnFire3, 2);

        if (!hideVisual)
        {
            Rectangle inflated = player.Hitbox.Inflated(16, 16);
            Dust.QuickBox(inflated.TopLeft(), inflated.BottomRight(), 2, Color.White, (d) => d.velocity = Vector2.Zero);
        }
    }
}

public class TrueDragonFruitPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}

public class TrueDragonFruitProjectile : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.hostile;
    }

    public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.GetModPlayer<TrueDragonFruitPlayer>().Active)
            {
                Rectangle inflated = targetHitbox.Inflated(16, 16);
                if (projHitbox.Intersects(inflated))
                {
                    return true;
                }
            }
        }
        return null;
    }

    public override bool CanHitPlayer(Projectile projectile, Player target)
    {

        return true;
    }
}
