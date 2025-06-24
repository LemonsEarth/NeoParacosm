using NeoParacosm.Core.Players;

namespace NeoParacosm.Common.Utils
{
    public static partial class LemonUtils
    {     
        public static NPAcessoryPlayer NPAccessoryPlayer(this Player player)
        {
            return player.GetModPlayer<NPAcessoryPlayer>();
        }

        public static NPBuffPlayer NPBuffPlayer(this Player player)
        {
            return player.GetModPlayer<NPBuffPlayer>();
        }

        public static NPArmorPlayer NPArmorPlayer(this Player player)
        {
            return player.GetModPlayer<NPArmorPlayer>();
        }

        public static bool HasAnyFireDebuff(this Player player)
        {
            return player.HasBuff(BuffID.OnFire) || player.HasBuff(BuffID.Burning) || player.HasBuff(BuffID.OnFire3) 
                || player.HasBuff(BuffID.Frostburn) || player.HasBuff(BuffID.Frostburn2) || player.HasBuff(BuffID.ShadowFlame);
        }

        public static bool HasAnyPoisonDebuff(this Player player)
        {
            return player.HasBuff(BuffID.Poisoned) || player.HasBuff(BuffID.Venom);
        }
    }
}
