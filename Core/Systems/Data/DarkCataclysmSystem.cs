namespace NeoParacosm.Core.Systems.Data;

public class DarkCataclysmSystem : ModSystem
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
        if (ShouldReset)
        {
            DCEffectFogColor = Color.White;
            DCEffectMaxFogOpacity = 0.4f;
            DCEffectNoFogDistance = 0;
            DCEffectFogSpeed = 1f;
        }
        else
        {
            DCEffectNoFogDistance = 1500;
            DCEffectMaxFogOpacity = 1f;
            DCEffectFogSpeed = 5;
            DCEffectFogColor = Color.Lerp(DCEffectFogColor, Color.Red, 1 / 60f);
        }
    }
}
