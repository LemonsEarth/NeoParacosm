using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class UnearthSpell : BaseSpell
{
    public override int AttackCooldown => 45;
    public override int ManaCost => 30;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;

        float xSlowDownBase = MathF.Pow(0.5f, player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f));
        float yPushBase = 20 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f);
        float baseDistance = 300 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2f);

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.life > 0 && !npc.friendly && npc.collideY && npc.Distance(player.Center) < baseDistance)
            {
                if (!npc.boss)
                {
                    npc.velocity.X *= xSlowDownBase * (1 - npc.knockBackResist);
                }
                npc.velocity.Y -= yPushBase * npc.knockBackResist;
                npc.SimpleStrikeNPC((int)(GetDamage(player) * npc.knockBackResist), 1);
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustDirect(npc.Bottom + Vector2.UnitX * i * 5, 1, 1, DustID.Dirt, 0, -5, Scale: 2).noGravity = true;
                }
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Earth];
    }
}