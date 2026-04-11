using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class GrenadeAttachment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 72;
        Item.height = 26;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 15);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<GrenadeAttachmentPlayer>().Active = true;
    }
}

public class GrenadeAttachmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int shootCounter = 0;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Active && item.useAmmo == AmmoID.Bullet)
        {
            int cd = ((int)(5 * MathHelper.Clamp(60f / item.useTime, 1, 30)));
            if (shootCounter % cd == 0)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    velocity,
                    ProjectileID.Grenade,
                    item.damage * 3,
                    8f,
                    Player.whoAmI
                    );
            }
            shootCounter++;
            return true;
        }
        return true;
    }
}

public class GrenadeAttachmentShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.ArmsDealer;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<GrenadeAttachment>(), Condition.DownedSkeletron);
    }
}
