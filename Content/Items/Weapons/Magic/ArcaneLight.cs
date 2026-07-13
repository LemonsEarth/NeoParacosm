using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class ArcaneLight : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.DamageType = DamageClass.Magic;
        Item.width = 28;
        Item.height = 50;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Pink;
        Item.UseSound = SFX.CrystalSerpent with { PitchRange = (-0.6f, -0.4f) };
        Item.autoReuse = true;
        Item.mana = 15;
        Item.shoot = ProjectileType<ArcaneLightProjectile>();
        Item.shootSpeed = 5;
        Item.noMelee = true;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        //player.itemLocation += new Vector2(-8 * player.direction, 24);
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //SoundEngine.PlaySound(ParacosmSFX.ChurchBell with { PitchRange = (-0.7f, -0.4f) }, player.Center);
        for (int i = -3; i <= 3; i++)
        {
            Projectile.NewProjectile(
                source,
                position,
                Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI,
                ai0: i * 100,
                ai1: Main.MouseWorld.X,
                ai2: Main.MouseWorld.Y);
        }
        return false;
    }
}

public class ArcaneLightDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinShark;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ArcaneLight>(), 3, 1, 1));
    }
}