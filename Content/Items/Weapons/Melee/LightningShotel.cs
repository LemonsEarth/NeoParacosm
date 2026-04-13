using Terraria.DataStructures;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Dusts;
using Terraria.Audio;
using NeoParacosm.Core.Systems.Assets;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class LightningShotel : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 110;
        Item.DamageType = DamageClass.Melee;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 15);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.useTurn = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {

        return true;
    }

    public override void AddRecipes()
    {
        //Recipe recipe1 = CreateRecipe();

        //recipe1.Register();
    }

    public override float UseTimeMultiplier(Player player)
    {
        return player.HasBuff(BuffType<OverchargedBuff>()) ? 0.33f : 1f;
    }

    public override float UseAnimationMultiplier(Player player)
    {
        return player.HasBuff(BuffType<OverchargedBuff>()) ? 0.33f : 1f;
    }
}

public class OverchargedBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.endurance += 15f / 100f;
        player.moveSpeed += 15f / 100f;
        player.AddElementalDamageBoost(SpellElement.Lightning, 0.2f);
        player.AddElementalExpertiseBoost(SpellElement.Lightning, 0.4f);
    }
}

public class OverchargedBuffPlayer : ModPlayer
{
    public override void ResetEffects()
    {

    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HeldItem.type == ItemType<LightningShotel>() && !Player.HasBuff(BuffType<OverchargedBuff>()))
        {
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (0.3f, 0.5f), Volume = 0.5f}, Player.Center);
            Player.AddBuff(BuffType<OverchargedBuff>(), ((int)(600 * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 2f))));
            LemonUtils.DustBurst(10, Player.Center, DustType<StreakDust>(), 5, 5, 1.2f, 2f, Color.Gold);
        }
    }
}
