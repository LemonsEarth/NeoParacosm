using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Armor.Generic.Stone;
using NeoParacosm.Content.NPCs.Misc;
using System.Linq;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Ranged.AscendedShadow;

[AutoloadEquip(EquipType.Head)]
public class AscendedShadowHelmet : ModItem
{
    static readonly float damageBoost = 4;
    static readonly float critBoost = 6;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critBoost);
    public static LocalizedText setBonusText;

    int timer = 0;

    public override void SetStaticDefaults()
    {
        setBonusText = this.GetLocalization("SetBonus").WithFormatArgs();
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 28;
        Item.defense = 5;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Ranged) += damageBoost / 100;
        player.GetCritChance(DamageClass.Ranged) += critBoost;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<AscendedShadowScalemail>() && legs.type == ItemType<AscendedShadowGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = setBonusText.Value;
        player.GetModPlayer<AscendedShadowArmorPlayer>().AscendedShadowArmor = true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInInventory(Item, ItemID.ShadowHelmet, position, scale, timer, frame, spriteBatch, Color.Purple);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        timer++;
        LemonUtils.DrawAscendedWeaponGlowInWorld(Item, ItemID.ShadowHelmet, rotation, scale, timer, spriteBatch, Color.Purple);
        return false;
    }
}

public class AscendedShadowArmorPlayer : ModPlayer
{
    public bool AscendedShadowArmor { get; set; } = false;
    int shadowTimer = 0;

    public override void ResetEffects()
    {
        AscendedShadowArmor = false;
    }

    public override void PostUpdateEquips()
    {

        if (shadowTimer > 0)
        {
            shadowTimer--;
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (AscendedShadowArmor && target.CanBeChasedBy() && shadowTimer == 0 && !Main.npc.Any(n => n.active && n.type == NPCType<ShadowOrbNPC>() && n.ai[0] == Player.whoAmI))
        {
            for (int i = 0; i < 2; i++)
            {
                NPC.NewNPCDirect(Player.GetSource_FromThis(), target.Center + Main.rand.NextVector2Circular(300, 300), NPCType<ShadowOrbNPC>(), ai0: Player.whoAmI);
            }
            shadowTimer = 600;
        }
    }
}
