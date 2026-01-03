using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Pure;

public class ManaGuardSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 60;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<ManaGuardBuff>(), (int)(20 * 60 * player.GetElementalExpertiseBoost(SpellElement.Pure)));
        SoundEngine.PlaySound(SoundID.Item130 with { PitchRange = (0.5f, 0.6f)}, player.Center);
        for (int j = 0; j < 8; j++)
        {
            Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.GemSapphire, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.6f, 2f)).noGravity = true;
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.Granite, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.6f, 2f)).noGravity = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Orange;
        SpellElements = [SpellElement.Pure];
    }
}