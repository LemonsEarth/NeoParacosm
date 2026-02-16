using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Core.Players;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class StarfallCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 5);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<StarfallCoatingPlayer>().Active = true;
    }
}

public class StarfallCoatingPlayer : CoatingPlayer
{
    public override int BaseCD => 20;
    public override void OnHitEffect(NPC npc, NPC.HitInfo hit)
    {
        Vector2 pos = npc.Center + new Vector2(Main.rand.NextFloat(-300, 300), -800);
        Projectile.NewProjectileDirect(
            Player.GetSource_FromThis(),
            pos,
            pos.DirectionTo(npc.Center) * Main.rand.NextFloat(5, 10),
            Main.rand.NextFromList(ProjectileID.StarVeilStar, ProjectileID.StarCloakStar, ProjectileID.Starfury),
            (int)(hit.Damage * 0.33f),
            0f,
            Player.whoAmI
            ).DamageType = DamageClass.Melee; // Changing to melee damage
    }
}

public class MerchantNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.Merchant)
        {
            shop.Add(ItemType<StarfallCoating>(), Condition.TimeNight);
        }
    }
}