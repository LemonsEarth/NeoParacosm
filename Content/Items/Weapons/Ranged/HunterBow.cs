using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class HunterBow : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 10;
        Item.knockBack = 6f;
        Item.crit = 5;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 32;
        Item.height = 56;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useAmmo = AmmoID.Arrow;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item5 with { PitchRange = (0.1f, 0.2f) }, player.Center);
        return true;
    }

    int standingStillTimer = 0;
    public override void HoldItem(Player player)
    {
        if (player.velocity.LengthSquared() <= 2 * 2)
        {
            if (standingStillTimer < 1000)
            {
                standingStillTimer++;
            }
        }
        else
        {
            standingStillTimer = 0;
        }
    }

    public override void ModifyWeaponCrit(Player player, ref float crit)
    {
        float boostValue = standingStillTimer / 10f;
        crit += boostValue;
    }

}
