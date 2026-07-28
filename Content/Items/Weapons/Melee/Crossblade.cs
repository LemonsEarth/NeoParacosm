using Terraria.DataStructures;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Dusts;
using Terraria.Audio;
using NeoParacosm.Core.Systems.Assets;
using Microsoft.Build.Framework;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class Crossblade : ModItem
{
    int hitCount = 0;

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.DamageType = DamageClass.Melee;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 8;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.useTurn = true;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10;
        Item.useAmmo = AmmoID.Arrow;
        Item.consumeAmmoOnFirstShotOnly = true;
        Item.consumeAmmoOnLastShotOnly = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (hitCount >= 10 && player.altFunctionUse == 2)
        {
            for (int i = 0; i < 6; i++)
            {
                var res = player.PickAmmo(Item, out int projID, out float speed, out int dmg, out float kb, out int usedAmmoItemID);
                if (res)
                {
                    Projectile.NewProjectileDirect(
                        source,
                        player.itemLocation,
                        player.DirectionTo(player.itemLocation).RotatedBy(i * MathHelper.Pi / 6f * player.direction) * speed,
                        projID,
                        (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(dmg),
                        kb,
                        player.whoAmI
                        );
                }
            }

            hitCount = 0;
        }


        return false;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hitCount < 10)
        {
            hitCount++;
            if (hitCount == 10)
            {
                SoundEngine.PlaySound(SFX.BowShot, player.Center);
            }
        }
    }
}

public class CrossbladeDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinWarrior;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<Crossblade>(), 30, 1, 1));
    }
}
