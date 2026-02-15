using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class CommensalPathogen : ModItem
{
    readonly float enduranceDecrease = 10;
    readonly float defenseDecrease = 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(enduranceDecrease, defenseDecrease);

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<BaneflyHive>();
    }
    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 4);
        Item.rare = ItemRarityID.Orange;
        Item.lifeRegen = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddBuff(BuffType<CrimsonTendrilBuff>(), 2);
        player.endurance -= enduranceDecrease / 100;
        player.statDefense *= 0.9f;
    }
}

public class CommensalPathogenPlayer : ModPlayer
{
    public List<Projectile> CrimsonTendrils { get; set; } = new List<Projectile>();

    public override void ResetEffects()
    {
        CrimsonTendrils.RemoveAll(p => !p.active);
    }

    public override void PostUpdateEquips()
    {
        if (Player.HasBuff(BuffType<CrimsonTendrilBuff>()))
        {
            if (CrimsonTendrils.Count < 3)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    int damage = Main.hardMode ? 60 : 30;
                    Projectile p = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<CrimsonTendrilFriendly>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage), 2f, Player.whoAmI);
                    CrimsonTendrils.Add(p);
                }
            }
        }
    }
}

public class CrimsonTendrilBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {

    }
}
