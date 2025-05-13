using System;
using System.Collections;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using UnityEngine;
namespace YoutubeIntegrationBB
{
    
    [BepInPlugin("partynoob.youtubeintegration","Youtube integration bb+","1.0.0.0")]

    
    public class BasePlugin : BaseUnityPlugin
    {

        internal ConfigEntry<String> ApiKey;
        internal ConfigEntry<String> VideoID;
        internal static BasePlugin Instance { get; private set; }

        internal static AssetManager asm;

        internal GameObject YoutubeHandleGO;

        internal NewYoutubeApiHandler YoutubeHandle;

        internal CommandsAndPollHandler CPH;

        internal bool Ingame = false;
        async void Awake()
        {

            Instance = this;
            ApiKey = Config.Bind("Important", "ApiKey", "unused", "unused");
            VideoID = Config.Bind("Important", "VideoId", "none", "Your youtube handle (example @Partynoob.official) ");
            MTM101BaldiDevAPI.AddWarningScreen("If you just installed this mod or whatsoever you need to go to Bepinex/config and open partynoob.youtubeintegration, for more help in how to setup check the mod page on gamebanana", false);
            MTM101BaldAPI.Registers.LoadingEvents.RegisterOnAssetsLoaded(this.Info, Preload(),false);
            
            asm = new();


            
            

            CPH = new GameObject().AddComponent<CommandsAndPollHandler>();
            DontDestroyOnLoad(CPH.gameObject);



            Harmony H = new Harmony(this.Info.Metadata.GUID);
            H.PatchAll();

            await StartLiveChat();

        }

        public static async Task StartLiveChat()
        {
            BasePlugin.Instance.YoutubeHandle = new NewYoutubeApiHandler();
            Debug.Log(BasePlugin.Instance.VideoID.Value);
            
        }

        IEnumerator Preload()
        {
            yield return 1;
            yield return "Loading stuff";
            asm.Add<SoundObject>("Mus_Distraction",
                MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "Mus_DistractionSong.wav"),
                "mus_distraction", SoundType.Effect, Color.black));
            asm.Get<SoundObject>("Mus_Distraction").subtitle = false;
        }
    }
}
