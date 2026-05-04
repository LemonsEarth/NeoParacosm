using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class ClingerMouth : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 38;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 15);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ClingerMouthPlayer>().Active = true;
    }
}

public class ClingerMouthPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int shootCounter = 0;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Active && item.DamageType == DamageClass.Ranged)
        {
            int cd = ((int)(3 * MathHelper.Clamp(60f / item.useTime, 1, 30)));
            if (shootCounter % cd == 0)
            {
                Projectile.NewProjectileDirect(
                    source,
                    position,
                    velocity,
                    ProjectileID.CursedFlameFriendly,
                    ((int)MathHelper.Clamp(damage * 2, 10, 400)),
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

public class ClingerMouthDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Clinger;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ClingerMouth>(), 25, 1, 1));
    }
}
