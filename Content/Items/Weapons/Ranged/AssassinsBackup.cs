using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class AssassinsBackup : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 22;
        Item.knockBack = 7f;
        Item.crit = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 5;
        Item.useAnimation = 25;
        Item.reuseDelay = 45;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<AssassinsBackupProj>();
        Item.shootSpeed = 10;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item1, player.Center);
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 16, MathHelper.Pi / 16)) * Main.rand.NextFloat(0.75f, 1.5f);
    }
}

public class AngryBonesNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetStaticDefaults()
    {

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (entity.type == NPCID.CursedSkull || entity.type == NPCID.GiantCursedSkull);
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<AssassinsBackup>(), 10, 1, 1));
    }
}
