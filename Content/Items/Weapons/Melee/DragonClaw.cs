using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class DragonClaw : ModItem
{
    int useCounter = 0;
    int special = 0;
    int specialCDTimer = 0;
    public override void SetDefaults()
    {
        Item.damage = 45;
        Item.DamageType = DamageClass.Melee;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<DragonClawHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int direction = 1;
        if (useCounter % 2 != 0)
        {
            direction = -1;
        }
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, ai0: special, ai1: direction);
        useCounter++;
        return false;
    }
}

public class DragonClawNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public bool Active { get; set; } = false;
    public int hitCount { get; set; } = 0;

    public override void ResetEffects(NPC npc)
    {
        Active = false;
    }

    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        if (Active)
        {
            modifiers.SetCrit();
        }
    }
}