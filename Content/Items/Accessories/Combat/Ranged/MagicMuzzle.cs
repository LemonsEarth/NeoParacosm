using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class MagicMuzzle : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MagicMuzzlePlayer>().Active = true;
    }
}

public class MagicMuzzlePlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.type == ProjectileID.Bullet && Main.rand.NextBool(8))
        {
            target.AddBuff(Main.rand.NextFromList(BuffID.ShadowFlame, BuffID.OnFire3, BuffID.Frostburn2, BuffID.Venom), 480);
        }
    }
}

public class MagicMuzzleShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.ArmsDealer;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<MagicMuzzle>(), Condition.DownedMechBossAny);
    }
}
