using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Items.Pickups;

namespace NeoParacosm.Core.Players;

public partial class NeoParacosmPlayer : ModPlayer
{
    public bool roundShield { get; set; } = false;
    public bool forestCrest { get; set; } = false;
    int forestCrestPickupCooldown = 0;

    void ResetAccessoryFields()
    {
        roundShield = false;
        forestCrest = false;
    }

    public override void PostUpdateEquips()
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>())) // On Hurt code is in Main
        {
            Player.noKnockback = true;
        }

        if (forestCrestPickupCooldown > 0) forestCrestPickupCooldown--;
    }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (victim is NPC npc)
        {
            if (Main.rand.NextBool(10) && forestCrestPickupCooldown == 0)
            {
                Item item = Player.QuickSpawnItemDirect(Player.GetSource_OnHit(victim, "Forest Crest Hit"), ModContent.ItemType<SmallFlowerPickup>(), 1);
                item.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI, number2: 0);
                forestCrestPickupCooldown = 30;
            }
        }
    }
}
