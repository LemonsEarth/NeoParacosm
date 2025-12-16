using NeoParacosm.Content.Items.BossSummons;
using NeoParacosm.Content.NPCs.Bosses.Dreadlord;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using System.IO;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace NeoParacosm.Core.Systems.Data;

public class WorldDataSystem : ModSystem
{
    public static float DCEffectOpacity = 0f;
    public static float DCEffectOpacityTimer = 0f;
    public static Color DCEffectFogColor = Color.White;
    public static Vector2 DCEffectNoFogPosition = Vector2.Zero;
    public static float DCEffectNoFogDistance = 0;
    public static float DCEffectNoFogDistanceCurrent = 0;
    public static float DCEffectMaxFogOpacity = 0.1f;
    public static float DCEffectFogOpacity = 0f;
    public static float DCEffectFogSpeed = 1f;
    public static bool AncientCallingHornInUse = false;
    public static bool DreadlordAlive = false;
    bool ShouldReset => !AncientCallingHornInUse && !DreadlordAlive;

    public override void PreUpdateItems()
    {
        AncientCallingHornInUse = false;
    }

    public override void PostUpdateItems()
    {
        
    }

    public override void PreUpdatePlayers()
    {
       
    }

    public override void PreUpdateNPCs()
    {
        DreadlordAlive = false;
    }

    public override void PostUpdateNPCs()
    {
        if (AncientCallingHornInUse)
        {
            DCEffectNoFogDistance = 2000;
            DCEffectMaxFogOpacity = 1f;
            DCEffectFogSpeed = 5;
            DCEffectFogColor = Color.Lerp(DCEffectFogColor, Color.Red, 1 / 60f);
        }
        if (ShouldReset)
        {
            DCEffectFogColor = Color.White;
            DCEffectMaxFogOpacity = 0.4f;
            DCEffectNoFogDistance = 0;
            DCEffectFogSpeed = 1f;
        }
    }
}
