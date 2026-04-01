using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class ElectrifiedBodyBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class ElectrifiedBodyPlayer : ModPlayer
{
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<ElectrifiedBodyBuff>()))
        {
            target.velocity *= (1 - target.knockBackResist);
        }
    }

    public override void UpdateEquips()
    {
        if (Main.myPlayer == Player.whoAmI && Player.HasBuff(BuffType<ElectrifiedBodyBuff>()) && NPPlayer.timer % 180 == 0)
        {
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (0.1f, 0.5f), Volume = 0.5f}, Player.Center);
            float dustSpeed = Player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 2) * 2;
            for (int j = 0; j < 9; j++)
            {
                Dust.NewDustDirect(Player.RandomPos(-Player.width / 2, -Player.height / 2), 2, 2, DustType<StreakDust>(),
                    Main.rand.NextFloat(-dustSpeed, dustSpeed), Main.rand.NextFloat(-dustSpeed, dustSpeed), Scale: Main.rand.NextFloat(0.5f, 0.75f)).noGravity = true;
            }

            //Dust.NewDustPerfect(Player.Center + Vector2.UnitX * 100 * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 5), DustID.GemDiamond, Vector2.Zero, Scale: 4f).noGravity = true;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && npc.Distance(Player.Center) < 100 * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 8))
                {
                    npc.SimpleStrikeNPC((int)(90 * Player.GetElementalDamageBoost(SpellElement.Lightning)), 1);
                }
            }
        }
    }
}
