using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class MagicGrenadeAttachment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 92;
        Item.height = 52;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MagicGrenadeAttachmentPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<MagicClingerMuzzle>(), 1);
        recipe.AddIngredient(ItemType<GrenadeAttachment>(), 1);
        recipe.AddIngredient(ItemID.IllegalGunParts, 3);
        recipe.AddIngredient(ItemID.Ectoplasm, 10);
        recipe.AddTile(TileID.TinkerersWorkbench);
        recipe.Register();
    }
}

public class MagicGrenadeAttachmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int shootCounter = 0;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.type == ProjectileID.Bullet && Main.rand.NextBool(8))
        {
            target.AddBuff(Main.rand.NextFromList(BuffID.ShadowFlame, BuffID.OnFire3, BuffID.Frostburn2, BuffID.Venom, BuffID.Ichor), 480);
        }

        if (Active)
        {
            target.AddBuff(BuffID.CursedInferno, 480);
        }

    }

    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Active)
        {
            int cd = ((int)(2 * MathHelper.Clamp(60f / item.useTime, 1, 30)));
            if (shootCounter % cd == 0)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    velocity,
                    ProjectileType<CursedGrenade>(),
                    ((int)MathHelper.Clamp(damage * 2, 10, 300)),
                    8f,
                    Player.whoAmI,
                    ai0: 90
                    );
            }
            shootCounter++;

        }
        return true;
    }
}