using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class EyezorHead : ModItem
{
    int timeHeld = 0;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 64;
        Item.DamageType = DamageClass.Magic;
        Item.width = 22;
        Item.height = 26;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.LightPurple;
        Item.autoReuse = true;
        Item.mana = 20;
        Item.shoot = ProjectileID.PurpleLaser;
        Item.holdStyle = ItemHoldStyleID.HoldFront;
        Item.shootSpeed = 5;
        Item.noMelee = true;
    }

    public override void HoldItem(Player player)
    {
        if (timeHeld < 300)
        {
            Vector2 pos = player.Center + Main.rand.NextVector2CircularEdge(32, 32);
            Vector2 dir = pos.DirectionTo(player.Center);
            Dust.NewDustDirect(
                pos,
                2, 2,
                DustID.GemDiamond,
                dir.X * 3,
                dir.Y * 3
                ).noGravity = true;
            timeHeld++;
        }
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation += new Vector2(-8 * player.direction, 0);
    }

    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        player.itemLocation += new Vector2(-16 * player.direction, 8);
    }

    public override float UseTimeMultiplier(Player player)
    {
        return 1f;
    }

    public override float UseAnimationMultiplier(Player player)
    {
        return 1f;
    }

    public override void UpdateInventory(Player player)
    {
        if (player.HeldItem.type != Type)
        {
            timeHeld = 0;
        }
    }

    public override bool? UseItem(Player player)
    {

        return null;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.Item33 with { PitchRange = (-0.1f, 0.1f) }, player.Center);
        NPC npc = LemonUtils.GetClosestNPC(player.Center, 800);
        if (npc != null)
        {
            for (int i = 0; i < (timeHeld / 60) + 1; i++)
            {
                Vector2 dirToNPC = position.DirectionTo(npc.Center);
                Vector2 vel = dirToNPC * Main.rand.NextFloat(10f, 15f);
                player.ChangeDir(LemonUtils.Sign(dirToNPC.X, 1));

                var proj = Projectile.NewProjectileDirect(
                    player.GetSource_FromThis("NeoParacosm:EyezorHead"),
                    position,
                    vel.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 12f, MathHelper.Pi / 12f)),
                    ProjectileID.PurpleLaser,
                    (int)(damage * ((timeHeld / 60f) + 1f)),
                    knockback,
                    player.whoAmI
                    );
            }
        }
        timeHeld = 0;
        return false;
    }
}

public class PurpleLaserEyezorHeadGlobalProjectile : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type == ProjectileID.PurpleLaser;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source.Context == "NeoParacosm:EyezorHead")
        {
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }
    }
}

public class EyezorHeadDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Eyezor;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(
            Condition.DownedMechBossAll.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied),
            ItemType<EyezorHead>(), 10));
    }
}