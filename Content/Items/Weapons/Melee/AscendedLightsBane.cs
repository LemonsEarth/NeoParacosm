﻿using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Melee;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class AscendedLightsBane : ModItem
{
    int timer = 0;
    int altFireCD = 0;

    public static HashSet<NPC> hitNPCs { get; set; } = new();

    public override void SetDefaults()
    {
        Item.damage = 25;
        Item.DamageType = DamageClass.Melee;
        Item.width = 54;
        Item.height = 54;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<AscendedLightsBaneHeldProj>();
        Item.shootSpeed = 30;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            altFireCD = 300;
            SoundEngine.PlaySound(SoundID.Item71, player.Center);
            foreach (NPC npc in hitNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    npc.AddBuff(ModContent.BuffType<LightsBaneDebuff>(), 60);
                }
            }
            hitNPCs.Clear();
        }
        return null;
    }

    public override void UpdateInventory(Player player)
    {
        if (altFireCD == 1)
        {
            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 1f }, player.Center);
            LemonUtils.DustCircle(player.Center, 8, 5, DustID.Corruption, 2f);
        }
        if (altFireCD > 0) altFireCD--;
    }

    public override bool AltFunctionUse(Player player)
    {
        return altFireCD == 0;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.LightsBane, position, scale, timer, frame, spriteBatch, Color.Purple);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.LightsBane, rotation, scale, timer, spriteBatch, Color.Purple);
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float speedBoost = player.altFunctionUse == 2 ? 2f : 1f;
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, speedBoost);
        return false;
    }
}