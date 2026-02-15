using NeoParacosm.Content.Items.Consumables;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class MushroomCannon : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 36;
        Item.knockBack = 5f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 48;
        Item.height = 20;
        Item.useTime = 15;
        Item.useAnimation = 30;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<GlowingMushroomProj>();
        Item.useAmmo = ItemType<GlowingMushroomSpore>();
        Item.shootSpeed = 20;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item61 with { PitchRange = (0.4f, 0.8f) }, player.Center);
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 16, MathHelper.Pi / 16));
    }
}

public class MushroomNPCs : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetStaticDefaults()
    {

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (entity.type == NPCID.FungiBulb
            || entity.type == NPCID.AnomuraFungus
            || entity.type == NPCID.MushiLadybug
            || entity.type == NPCID.ZombieMushroom
            || entity.type == NPCID.ZombieMushroomHat);
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        IItemDropRule dropRule1;
        IItemDropRule dropRule2;
        switch (npc.type)
        {
            case NPCID.AnomuraFungus or NPCID.MushiLadybug or NPCID.FungiBulb:
                dropRule1 = ItemDropRule.Common(ItemType<MushroomCannon>(), 20);
                dropRule2 = ItemDropRule.Common(ItemType<GlowingSporeSack>(), 20);
                break;
            default:
                dropRule1 = ItemDropRule.Common(ItemType<MushroomCannon>(), 40);
                dropRule2 = ItemDropRule.Common(ItemType<GlowingSporeSack>(), 40);
                break;
        }
        npcLoot.Add(dropRule1);
        npcLoot.Add(dropRule2);
    }
}
