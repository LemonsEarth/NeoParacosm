using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class HolySpears : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 22;
        Item.knockBack = 9f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 90;
        Item.height = 38;
        Item.useTime = 15;
        Item.useAnimation = 45;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<HolySpearFriendly>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item1, player.Center);
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: 10, ai1: 0.2f, ai2: 10f);
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 64, MathHelper.Pi / 64)) * Main.rand.NextFloat(1.1f, 1.25f);
    }
}

public class HolySpearsDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetStaticDefaults()
    {

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && entity.type == NPCType<SpearKnight>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<HolySpears>(), 25, 1, 1));
    }
}
