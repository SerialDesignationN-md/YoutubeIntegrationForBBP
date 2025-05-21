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

        internal YouTubeChatListener YoutubeHandle;

        internal CommandsAndPollHandler CPH;

        internal bool Ingame = false;
        async void Awake()
        {

            Instance = this;
            ApiKey = Config.Bind("Important", "ApiKey", "none", "The youtube data api v3 Key");
            VideoID = Config.Bind("Important", "VideoId", "none", "The current video id ");
            MTM101BaldiDevAPI.AddWarningScreen("If you just installed this mod or whatsoever you need to go to Bepinex/config and open partynoob.youtubeintegration, for more help in how to setup check the mod page on gamebanana", false);
            MTM101BaldAPI.Registers.LoadingEvents.RegisterOnAssetsLoaded(this.Info, Preload(),false);
            
            asm = new();


            
            

            CPH = new GameObject().AddComponent<CommandsAndPollHandler>();
            DontDestroyOnLoad(CPH.gameObject);


            YoutubeHandle = new GameObject("YoutubeHandler").AddComponent<YouTubeChatListener>();
            DontDestroyOnLoad(YoutubeHandle.gameObject); 
            Harmony H = new Harmony(this.Info.Metadata.GUID);
            H.PatchAll();


        }

        

        IEnumerator Preload()
        {
            yield return 2;
            yield return "Loading sounds";
            asm.Add<SoundObject>("Mus_Distraction",
                MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "Mus_DistractionSong.wav"),
                "mus_distraction", SoundType.Effect, Color.black));
            asm.Get<SoundObject>("Mus_Distraction").subtitle = false;
            asm.Add<SoundObject>("Mus_Ohnoes",
                MTM101BaldAPI.ObjectCreators.CreateSoundObject(MTM101BaldAPI.AssetTools.AssetLoader.AudioClipFromMod(this, "ohnoes.ogg"),
                "mus_distraction", SoundType.Effect, Color.black));
            asm.Get<SoundObject>("Mus_Ohnoes").subtitle = false;

            yield return "Loading Images";

            asm.Add<Sprite>("Spr_PlaceHolder",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "PlaceHolderIcon.png"));

            asm.Add<Sprite>("Spr_Stun",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Plr_Stun.png"));
            asm.Add<Sprite>("Spr_PlrSpeedUp",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Plr_SpeedUp.png"));
            asm.Add<Sprite>("Spr_NpcSpeedUp",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Npc_SpeedUp.png"));
            asm.Add<Sprite>("Spr_NpcExtremeSpeedUp",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Npc_ExtremeSpeedUp.png"));
            asm.Add<Sprite>("Spr_Freeze",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Npc_Freeze.png"));
            asm.Add<Sprite>("Spr_Invisible",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Npc_Invisible.png"));
            asm.Add<Sprite>("Spr_Silent",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Npc_Silent.png"));
            asm.Add<Sprite>("Spr_LoudMusic",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "LoudMusic.png"));
            asm.Add<Sprite>("Spr_FogIsHere",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "FogIsHere.png"));
            asm.Add<Sprite>("Spr_EnvSpeedUp",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Env_SpeedUp.png"));
            asm.Add<Sprite>("Spr_LockedDoors",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Env_LockedDoors.png"));
            asm.Add<Sprite>("Spr_HappyBaldi",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Bal_Happy.png"));
            asm.Add<Sprite>("Spr_AngryBaldi",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Bal_Angry.png"));
            asm.Add<Sprite>("Spr_99baldi",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "99baldi.png"));
            asm.Add<Sprite>("Spr_NoStamina",
                AssetLoader.SpriteFromMod(this, Vector2.one / 2, 15, "Plr_NoStamina.png"));
        }
    }
}
