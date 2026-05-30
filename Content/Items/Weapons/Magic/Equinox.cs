using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class Equinox : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 75;
        Item.DamageType = DamageClass.Magic;
        Item.width = 58;
        Item.height = 54;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.reuseDelay = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 6;
        Item.mana = 30;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ItemRarityID.Yellow;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ProjectileType<EquinoxSun>();
        Item.shootSpeed = 6;
        Item.channel = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 sunPos = player.MountedCenter - Vector2.UnitY * 45.25f;
        var sun = Projectile.NewProjectileDirect(source, sunPos, Vector2.Zero, type, damage, knockback, player.whoAmI);
        LemonUtils.DustCircle(sun.Center, 8, 5, DustID.GemTopaz, 2f);
        int sunID = sun.identity;

        Vector2 moonPos = player.MountedCenter + Vector2.UnitY * 33.94f;
        var moon = Projectile.NewProjectileDirect(source, moonPos, Vector2.Zero, ModContent.ProjectileType<EquinoxMoon>(), damage, knockback, player.whoAmI);
        LemonUtils.DustCircle(moon.Center, 8, 5, DustID.GemDiamond, 2f);
        return false;
    }
}

public class EquinoxSunDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Mothron;
    }
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<Equinox>(), 5));
    }
}
