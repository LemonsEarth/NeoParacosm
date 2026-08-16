using NeoParacosm.Content.NPCs.Bosses.Dreadlord;
using System.IO;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

/// <summary>
/// Controls Dark Cataclysm effect params and whether it is active globally.
/// Effects and actual shader code are handled in DarkCataclysmPlayer.
/// </summary>
public class DarkCataclysmSystem : ModSystem
{
    public static bool DarkCataclysmActive { get; set; } = false;
    public static float DCEffectOpacity = 0f;
    public static float DCEffectOpacityTimer = 0f;
    public static Color DCEffectFogColor = Color.White;
    public static Vector2 DCEffectNoFogPosition = Vector2.Zero;
    public static float DCEffectNoFogDistance = 0;
    public static float DCEffectNoFogDistanceCurrent = 0;
    public static float DCEffectMaxFogOpacity = 0.1f;
    public static float DCEffectFogOpacity = 0f;
    public static float DCEffectFogSpeed = 1f;
    public static float DCEffectFogColorMultiplier = 1f;
    public static bool AncientCallingHornInUse = false; // Set in DarkCataclysmPlayer

    // Only reset fields when Ancient Calling Horn isn't being used and Dreadlord isn't alive, as they manipulate the fog distance, position etc.
    static bool ShouldReset => !AncientCallingHornInUse && !NPC.AnyNPCs(NPCType<Dreadlord>());

    public override void PreUpdateItems()
    {
        AncientCallingHornInUse = false;
    }

    public override void PostUpdateNPCs()
    {
        // Only reset when Ancient Calling Horn isn't being used and Dreadlord isn't alive, as they manipulate the fog
        if (ShouldReset)
        {
            DCEffectFogColor = Color.White;
            DCEffectMaxFogOpacity = 0.4f;
            DCEffectNoFogDistance = 0;
            DCEffectFogSpeed = 1f;
            DCEffectFogColorMultiplier = 1;
        }
        else
        {
            DCEffectNoFogDistance = 2000;
            DCEffectMaxFogOpacity = 1f;
            DCEffectFogSpeed = 5f;
            DCEffectFogColor = Color.Lerp(DCEffectFogColor, Color.DarkRed, 1 / 60f);
            DCEffectFogColorMultiplier = 10;
        }
    }

    public override void ClearWorld()
    {
        DarkCataclysmActive = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (DarkCataclysmActive)
        {
            tag[nameof(DarkCataclysmActive)] = true;
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        DarkCataclysmActive = tag.ContainsKey(nameof(DarkCataclysmActive));
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte flags = new BitsByte();
        flags[0] = DarkCataclysmActive;
        writer.Write(flags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte flags = reader.ReadByte();
        DarkCataclysmActive = flags[0];
    }
}
