

using NeoParacosm.Core.Conditions;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TheGuardDuty : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 8;
        Item.knockBack = 4f;
        Item.crit = 3;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 70;
        Item.height = 24;
        Item.useTime = 5;
        Item.useAnimation = 5;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item11;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.HornetStinger;
        Item.useAmmo = AmmoID.Bullet;
        Item.shootSpeed = 10;
        Item.noMelee = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8));
        type = ProjectileID.HornetStinger;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.rand.NextBool(30))
        {
            SoundEngine.PlaySound(SoundID.Item97, player.Center);
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectileDirect(source, position, velocity * Main.rand.NextFloat(0.3f, 1f), ProjectileID.Bee, damage, 2f);
                Dust.NewDustDirect(position, 2, 2, DustID.Honey2, velocity.X, velocity.Y, Scale: 2f).noGravity = true;
            }
        }
        return true;
    }
}

public class HornetNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetStaticDefaults()
    {

    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && (entity.type == NPCID.Hornet || entity.type == NPCID.MossHornet);
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        ItemDropWithConditionRule rule1 = new ItemDropWithConditionRule(ItemType<TheGuardDuty>(), 50, 1, 1, new EyeOfCthulhuDowned());
        npcLoot.Add(rule1);
    }
}
