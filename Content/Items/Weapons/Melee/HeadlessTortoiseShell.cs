using NeoParacosm.Content.Projectiles.Friendly.Melee;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class HeadlessTortoiseShell : ModItem
{
    public override void SetStaticDefaults()
    {
        //Item.staff[Type] = true;

    }

    public override void SetDefaults()
    {
        Item.damage = 50;
        Item.DamageType = DamageClass.Melee;
        Item.width = 58;
        Item.height = 58;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.Pink;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<HeadlessTortoiseShellProj>();
        Item.shootSpeed = 10;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override void HoldItem(Player player)
    {
        player.endurance += 5f / 100f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity * 3, type, damage, knockback, player.whoAmI, ai1: LemonUtils.Sign(player.DirectionTo(Main.MouseWorld).X, 1));
        return false;
    }
}

public class HeadlessTortoiseShellBuff : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed += 20f / 100f;
        player.endurance += 10f / 100f;
        player.statDefense += 5;
    }
}

public class HeadlessTortoiseShellPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HeldItem.type == ItemType<HeadlessTortoiseShell>())
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (player.whoAmI != Player.whoAmI && player.team == Player.team && player.DistanceSQ(Player.Center) < 500 * 500)
                {
                    player.AddBuff(BuffType<HeadlessTortoiseShellBuff>(), 60 * 15);
                }
            }
        }
    }
}