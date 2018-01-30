#region Using Statements
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using P2PTank.Scenes;
using P2PTank.Services;
using WaveEngine.Framework;
#endregion

namespace P2PTank
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            //SerializerFactory.DefaultSerializationType = SerializationType.DATACONTRACT;

            // Better if serialized and stored 
            GameSettings.EnableFX = true;
            GameSettings.EnableMusic = true;
            GameSettings.FXVolume = 1.0f;
            GameSettings.MusicVolume = 1.0f;
            GameSettings.GamePadDeadZone = 0.25f;

            WaveServices.RegisterService(new AudioService());
            ScreenContext screenContext;

            if (WaveServices.Platform.PlatformFamily == PlatformFamily.Desktop)
            {
                screenContext = new ScreenContext(new GamePlayScene());
            }
            else
            {
                screenContext = new ScreenContext(
                    new GamePlayScene(),
                    new VirtualJoystickScene())
                {
                    Behavior = ScreenContextBehaviors.UpdateInBackground | ScreenContextBehaviors.DrawInBackground
                };
            }

            WaveServices.ScreenContextManager.To(screenContext);
            // WaveServices.ScreenContextManager.SetDiagnosticsActive(true);
        }
    }
}