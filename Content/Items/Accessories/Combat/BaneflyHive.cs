using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

[AutoloadEquip(EquipType.Back)]
public class BaneflyHive : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<CommensalPathogen>();
    }
    public override void SetDefaults()
    {
        Item.width = 58;
        Item.height = 58;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 4);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 3;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.AddBuff(BuffType<BaneflyHiveBuff>(), 2);
    }
}

public class BaneflyHivePlayer : ModPlayer
{
    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HasBuff(BuffType<BaneflyHiveBuff>()))
        {
            Player.AddBuff(BuffID.CursedInferno, 120);
        }
    }

    public override void PostUpdateEquips()
    {
        if (Player.HasBuff(BuffType<BaneflyHiveBuff>()))
        {
            if (NPPlayer.timer % 300 == 0 && Main.myPlayer == Player.whoAmI)
            {
                for (int i = 0; i < 4; i++)
                {
                    int damage = Main.hardMode ? 20 : 10;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<Banefly>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage), 0.5f, Player.whoAmI);
                }
            }
        }
    }
}

public class BaneflyHiveBuff : ModBuff
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
