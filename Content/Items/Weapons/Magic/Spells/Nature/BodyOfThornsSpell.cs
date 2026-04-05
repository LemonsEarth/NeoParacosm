using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;

public class BodyOfThornsSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 60;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<BodyOfThornsBuff>(), ((int)(20 * 60 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Nature, 2))));
        SoundEngine.PlaySound(SoundID.Grass with { PitchRange = (-0.3f, 0.3f) }, player.Center);
        for (int j = 0; j < 6; j++)
        {
            Dust.NewDustDirect(player.RandomPos(-player.width / 2, -player.height / 2), 2, 2, DustID.GemEmerald, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: Main.rand.NextFloat(0.5f, 0.75f)).noGravity = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 2);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Nature];
    }
}