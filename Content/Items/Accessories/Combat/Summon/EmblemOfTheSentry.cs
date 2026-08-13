using NeoParacosm.Content.Projectiles.Friendly.Summon;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class EmblemOfTheSentry : ModItem
{
    int timer = 0;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 15);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            if (timer % 240 == 0 && player.ownedProjectileCounts[ProjectileType<SentryTurret>()] < 6)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.5f, -0.3f) }, player.Center);
                Projectile.NewProjectileDirect(
                    player.GetSource_Accessory(Item),
                    player.Center,
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, MathHelper.Pi / 3f)) * Main.rand.NextFloat(4, 8),
                    ProjectileType<SentryTurret>(),
                    (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(10),
                    1f,
                    player.whoAmI,
                    ai1: 600
                    );
            }
        }
        timer++;
    }
}

public class EmblemOfTheSentryShopNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Mechanic;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<EmblemOfTheSentry>(), Condition.DownedSkeletron);
    }
}