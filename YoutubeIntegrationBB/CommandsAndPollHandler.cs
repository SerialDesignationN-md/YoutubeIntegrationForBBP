using HarmonyLib;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine.UI;
using TMPro;
using UnityEngine;



namespace YoutubeIntegrationBB
{
    public record CommandType
    {
        public string nameCommand;
        public string nameShownOnChoices;
        public string TypeOfCommand;
        [OptionalField]public object[] Args;
    }
    internal class CommandsAndPollHandler : MonoBehaviour
    {
        CommandType[] SelectableCommands;
        public Commands CmdsComp;
        public string[] usersAlreadyVoted;
        public int[] votes;
        internal bool canVote = false;
        private TextMeshProUGUI inst;
        Coroutine CurrentPollCoroutine;
        public void NewCommand(string NameCommand, string nameShownOnStream, string TypeOfCommand, [Optional]object[] args)
        {
            var NewCommand = new CommandType();
            NewCommand.nameCommand = NameCommand;
            NewCommand.nameShownOnChoices = nameShownOnStream;
            NewCommand.TypeOfCommand = TypeOfCommand;
            NewCommand.Args = args;

            SelectableCommands = SelectableCommands.AddToArray(NewCommand);
        }
        void Start ()
        {
            

            NewCommand("TheFog", "The fog is here", "Disadvantage");
            NewCommand("LockAllDoors", "All the doors will be locked for a while", "Disadvantage");
            NewCommand("SpeedUpNpcs", "All NPCS will speed up for a while", "Disadvantage");
            NewCommand("SpeedUpEnvironment", "The environment will speed up for a while", "Disadvantage");
            NewCommand("SpeedUpPlayer", "You will speed up for a while", "Advantage");
            NewCommand("GiveRandomYtp", "You will gain or lose a random amount of ytps", "MixAdvantage");
            NewCommand("AllNpcsInvisible", "All the NPCS will be invisible for a while", "Disadvantage");
            NewCommand("SpawnNpc", "A random npc will spawn for the whole game on a random cell of the map", "Disadvantage");
            NewCommand("AngerBaldiPerm", "Baldi gets angry and never come back to its original speed", "Disadvantage");
            NewCommand("PraiseBaldi", "Baldi becomes happy for 5 seconds", "Advantage");
            NewCommand("SilentNpcs", "Every npcs is silent for a while", "Disadvantage");
            NewCommand("ClearInventory", "Clears the whole inventory", "Disadvantage");
            NewCommand("ShuffleInventory", "Every slots that have an item gets shuffled", "MixAdvantage");
            NewCommand("Stunned", "You are stunned for 7 seconds", "Disadvantage");
            NewCommand("NeverGonnaGiveYou", "Never gonna give you up never gonna let you down", "Troll");
            NewCommand("Distraction", "A loud music plays constantly alerting baldi of your location till the end of the music", "Disadvantage");
            NewCommand("Spawn99Chalkles", "Spawns 99 chalkles", "Troll");
            NewCommand("Spawn99Beans", "Spawns 99 beans", "Troll");
            NewCommand("Spawn99Baldis", "Spawns 99 baldis", "Troll");
            NewCommand("UseItem", "forcefully uses an item", "Disadvantage");
            NewCommand("NoStamina", "no stamina for 30 seconds, using anything to give stamina back will be useless", "Disadvantage");
            NewCommand("EnragedBaldi", "Baldi will get super angry for 10 seconds", "Disadvantage");
            // NewCommand("Spawn99Tests", "Spawn 99 ms pomps", "Troll");
            NewCommand("TooFast", "The environment will speed up 5x faster making the timer go down very quick", "Disadvantage");
            NewCommand("Negative500stamina", "Negative -500 for the stamina", "Disadvantage");
            NewCommand("Frozen", "Everything excluding you will be frozen for 10 seconds", "Advantage");
            NewCommand("BaldiIsHappier", "baldi will calm down", "Advantage");
            NewCommand("CrazyNpcSpeed", "all npcs will speed up by 20x for 2 seconds and the duration gets longer everytime this is selected (resets when going to the next floor or dying)", "UltraDisadvantage");
            


        }
        public void Play()
        {
            Debug.Log("Starting Vote System...");
            CmdsComp = new GameObject().AddComponent<Commands>();
            DontDestroyOnLoad(CmdsComp.gameObject);

            votes = [0, 0, 0, 0, 0];
            usersAlreadyVoted = [];
            canVote = false;

            BasePlugin.Instance.YoutubeHandle.callback = VoteHandler;

            CurrentPollCoroutine = StartCoroutine(VotePollHandler());
            

        }


        internal void VoteHandler(ChatMessage ChatMessage)
        {
           if (!usersAlreadyVoted.Contains(ChatMessage.Author) && canVote)
            {
                for (int i = 1; i < 6; i++)
                {
                    if (ChatMessage.Message == i.ToString())
                    {
                        usersAlreadyVoted = usersAlreadyVoted.AddToArray(ChatMessage.Author);
                        votes[i - 1] += 1;
                    }
                }
            }
            
        }

        public void Stop()
        {
            Destroy(CmdsComp.gameObject);
            Destroy(inst);
            if (CurrentPollCoroutine != null)
            {
                StopCoroutine(CurrentPollCoroutine);
            }
            votes = [0, 0, 0, 0, 0];
            usersAlreadyVoted = [];
            canVote = false;
            BasePlugin.Instance.YoutubeHandle.callback = (e) =>
            {

            };
            foreach (var item in Choices)
            {
                Destroy(item.gameObject);
            }
            Choices = [];
            
            

        }


        TextMeshProUGUI[] Choices;
        IEnumerator VotePollHandler()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                var Canvas = Singleton<CoreGameManager>.Instance.GetHud(0).Canvas();
                System.Random rng = new System.Random();
                var ChoicesCmds = SelectableCommands.OrderBy(x => rng.Next()).Take(5).ToArray();
                votes = [0, 0, 0, 0, 0];
                usersAlreadyVoted = [];
                inst = MTM101BaldAPI.UI.UIHelpers.CreateText<TextMeshProUGUI>(MTM101BaldAPI.UI.BaldiFonts.ComicSans12, "Hey viewers Type: 1,2,3,4,5 to vote\none of the choices down here\n",
                        Canvas.transform, Vector3.zero);
                inst.rectTransform.anchorMax = new Vector2(0.5f, 0.825f);
                inst.rectTransform.anchorMin = new Vector2(0.5f, 0.825f);
                inst.rectTransform.sizeDelta = new Vector2(200, 50);
                inst.color = Color.black;

                for (int i = 0; i < 5; i++)
                {
                    if (i != 5)
                    {
                        var text = MTM101BaldAPI.UI.UIHelpers.CreateText<TextMeshProUGUI>(MTM101BaldAPI.UI.BaldiFonts.ComicSans12, $"[{i + 1}] " + ChoicesCmds[i].nameShownOnChoices,
                            Canvas.transform, Vector3.zero);
                        text.rectTransform.anchorMax = new Vector2(0.35f, 0.25f + 0.1f * i);
                        text.rectTransform.anchorMin = new Vector2(0.35f, 0.25f + 0.1f * i);
                        text.rectTransform.sizeDelta = new Vector2(350, 100);
                        text.horizontalAlignment = HorizontalAlignmentOptions.Left;
                        
                        
                        switch (ChoicesCmds[i].TypeOfCommand)
                        {
                            default:
                                text.color = Color.black;
                                break;
                            case "Disadvantage":
                                text.color = Color.red;
                                
                                break;
                            case "UltraDisadvantage":
                                text.color = new Color(0.5f,0,0);
                                break;
                            case "Advantage":
                                text.color = Color.green;
                                break;
                            case "MixAdvantage":
                                text.color = Color.yellow;
                                break;
                            case "Troll":
                                text.color = Color.gray;
                                break;


                        }

                        Choices = Choices.AddToArray(text);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                canVote = true;
                for (int i = 30; i > 0; i--)
                {
                    for (int ii = 0; ii < 5; ii++)
                    {
                        Choices[ii].text = $"[{ii + 1}] " + ChoicesCmds[ii].nameShownOnChoices + $"({votes[ii]})";
                        inst.text = $"Hey viewers Type: 1,2,3,4,5 to vote\none of the choices down here\n{i}";
                    }
                    yield return new WaitForSeconds(1);
                }
                canVote = false;
                inst.text = "calculating votes";
                yield return new WaitForSeconds(0.45f);
                var numWinner = votes.Max();
                var Index = Array.IndexOf(votes, numWinner);
                for (int i = 0; i < 45; i++)
                {
                    Choices[Index].rectTransform.anchorMax += new Vector2(0.002f, 0);
                    Choices[Index].rectTransform.anchorMin += new Vector2(0.002f, 0);
                    yield return new WaitForSeconds(0.01f);
                }
                

                StartCoroutine(CmdsComp.ExecuteCommand(ChoicesCmds[Index].nameCommand));
                yield return new WaitForSeconds(2);
                foreach (var text in Choices)
                {
                    Destroy(text);
                }
                Choices = [];
                ChoicesCmds = [];
                Destroy(inst);

            }
        }
        
    }

    internal class Commands : MonoBehaviour
    {
        
        private EnvironmentController ec;
        private float durationCommandNpc = 2;
        private IEnumerator NewGaugeCoroutine(float timelast, Sprite sprite)
        {
            var newGauge = Singleton<CoreGameManager>.Instance.GetHud(0).gaugeManager.ActivateNewGauge(sprite, timelast);
            var timeleft = timelast;
            yield return new WaitForSeconds(0.05f);
            while (timeleft > 0)
            {
                timeleft -= Time.deltaTime * Time.timeScale;
                newGauge.SetValue(timelast, timeleft);
                yield return null;
            }

            newGauge.Deactivate();
        }

        internal void NewGauge(float time, Sprite sprite)
        {
            StartCoroutine(NewGaugeCoroutine(time, sprite));
        }



        public IEnumerator ExecuteCommand(string CommandName, [Optional]object[] Args)
        {
           
            switch (CommandName)
            {
                default:
                    Debug.LogError("Command named: " + CommandName + " dosent exists, and will be skipped");
                    break;
                case "TheFog":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    Fog fog = new()
                    {
                        color = Color.black,
                        maxDist = 50,
                        startDist = 0,
                        strength = 2
                    };
                    ec.AddFog(fog);
                    ec.UpdateFog();
                    NewGauge(60, BasePlugin.asm.Get<Sprite>("Spr_FogIsHere"));
                    yield return new WaitForSeconds(60);
                    ec.RemoveFog(fog);
                    ec.UpdateFog();


                    break;
                case "LockAllDoors":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    foreach (var rooms in ec.rooms)
                    {
                        foreach (var door in rooms.doors)
                        {
                            door.Shut();
                            door.LockTimed(10);
                            yield return new WaitForSeconds(0.08f);
                        }
                    }
                    NewGauge(10, BasePlugin.asm.Get<Sprite>("Spr_LockedDoors"));


                    break;
                case "SpeedUpNpcs":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpeedCommandsHandler().Execute(["Npc",ec]));
                    
                    break;
                case "SpeedUpEnvironment":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpeedCommandsHandler().Execute(["Env", ec]));
                    break;
                case "SpeedUpPlayer":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpeedCommandsHandler().Execute(["Player", ec]));
                    break;
                case "GiveRandomYtp":
                    Singleton<CoreGameManager>.Instance.AddPoints(UnityEngine.Random.Range(-100,101)*10,0, true);

                    break;
                case "AllNpcsInvisible":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    foreach (var npc in ec.Npcs)
                    {
                        npc.Navigator.Entity.SetHidden(true);
                    }
                    NewGauge(30, BasePlugin.asm.Get<Sprite>("Spr_Invisible"));
                    yield return new WaitForSeconds(30);
                    foreach (var npc in ec.Npcs)
                    {
                        npc.Navigator.Entity.SetHidden(false);
                    }

                    break;
                case "SpawnNpc":
                    var allNpcs = NPCMetaStorage.Instance.All();
                    var choosenNpc = allNpcs[UnityEngine.Random.Range(0, allNpcs.Length)].value;
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpawnNpcHandler().Execute([choosenNpc, 0, 1, ec, ""]));
                    break;
                case "AngerBaldiTemp":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    
                        ec.GetBaldi().GetExtraAnger(3.5f);
                    
                    break;
                case "AngerBaldiPerm":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    
                        ec.GetBaldi().GetAngry(3.5f);
                    
                    break;
                case "PraiseBaldi":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    NewGauge(5, BasePlugin.asm.Get<Sprite>("Spr_HappyBaldi"));
                    ec.GetBaldi().Praise(5);
                    break;
                case "ResetActivities":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    foreach (var item in ec.activities)
                    {
                        if (item.NotebookCollected || item.IsCompleted)
                        {
                            item.ReInit();
                            Singleton<BaseGameManager>.Instance.AddNotebookTotal(1);
                        }
                    }
                    break;
                case "SilentNpcs":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    foreach (var npc in ec.Npcs)
                    {
                        if (npc.TryGetComponent<PropagatedAudioManager>(out var audMan))
                        {
                            audMan.volumeModifier = 0;
                        }

                    }
                    NewGauge(30, BasePlugin.asm.Get<Sprite>("Spr_Silent"));
                    yield return new WaitForSeconds(30);
                    foreach (var npc in ec.Npcs)
                    {
                        if (npc.TryGetComponent<PropagatedAudioManager>(out var audMan))
                        {
                            audMan.volumeModifier = 1;
                        }

                    }
                    break;
                case "ClearInventory":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ClearItems();

                    break;
                case "ShuffleInventory":
                    var slotsToFill = 0;
                    foreach (var item in Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.items)
                    {
                        if (item != ItemMetaStorage.Instance.FindByEnum(Items.None).value)
                        {
                            slotsToFill++;
                        }
                    }
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.ClearItems();
                    for (int i = 0; i < slotsToFill; i++)
                    {
                        Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.AddItem(ItemMetaStorage.Instance.All()[UnityEngine.Random.Range(0, ItemMetaStorage.Instance.All().Length)].value);
                    }


                    

                    break;
                case "Stunned":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpikPlr2 = new TimeScaleModifier(1f, 1, 0f);
                    ec.AddTimeScale(timeScaleEpikPlr2);
                    NewGauge(7, BasePlugin.asm.Get<Sprite>("Spr_Stun"));
                    yield return new WaitForSeconds(7f);
                    ec.RemoveTimeScale(timeScaleEpikPlr2);
                    break;
                case "NeverGonnaGiveYou":
                    Application.OpenURL("https://www.youtube.com/watch?v=GqmMCjLzn4I");
                    break;
                case "Distraction":
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.asm.Get<SoundObject>("Mus_Distraction"));
                    NewGauge(45, BasePlugin.asm.Get<Sprite>("Spr_LoudMusic"));
                    for (int i = 0; i < 43; i++) {

                        yield return new WaitForSeconds(1f);
                        Singleton<BaseGameManager>.Instance.Ec.GetBaldi().Hear(Singleton<CoreGameManager>.Instance.GetPlayer(0).gameObject, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position, 100, true);

                    }
                    break;
                case "Spawn99Chalkles":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    
                    StartCoroutine(new SpawnNpcHandler().Execute([NPCMetaStorage.Instance.Get(Character.Chalkles).value, 0, 99, ec, ""]));
                    break;
                case "Spawn99Beans":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpawnNpcHandler().Execute([NPCMetaStorage.Instance.Get(Character.Beans).value, 0, 99, ec, ""]));

                    break;
                case "Spawn99Tests":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpawnNpcHandler().Execute([NPCMetaStorage.Instance.Get(Character.LookAt).value, 0, 99, ec, ""]));

                    break;
                case "Spawn99Baldis":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    StartCoroutine(new SpawnNpcHandler().Execute([NPCMetaStorage.Instance.Get(Character.Baldi).value, 10, 15, ec, "baldi"]));

                    break;
                case "UseItem":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.UseItem();
                    break;
                case "NoStamina":
                    var timeleft = 30f;
                    NewGauge(30, BasePlugin.asm.Get<Sprite>("Spr_NoStamina"));
                    while (timeleft > 0f)
                    {
                        Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina = 0;
                        timeleft = Time.deltaTime;
                        yield return null;
                    }
                    break;
                case "EnragedBaldi":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    ec.GetBaldi().GetAngry(20f);
                    NewGauge(10, BasePlugin.asm.Get<Sprite>("Spr_AngryBaldi"));
                    yield return new WaitForSeconds(10);
                    ec.GetBaldi().GetAngry(-20f);
                    break;
                case "Frozen":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timescale = new TimeScaleModifier(1, 1, 1);
                    ec.AddTimeScale(timescale);
                    while (timescale.environmentTimeScale >= 0.05f && timescale.npcTimeScale >= 0.05f)
                    {
                        timescale.npcTimeScale = Mathf.Lerp(timescale.npcTimeScale, 0, 0.02f);
                        timescale.environmentTimeScale = Mathf.Lerp(timescale.environmentTimeScale, 0, 0.02f);
                        yield return null;
                    }
                    NewGauge(10, BasePlugin.asm.Get<Sprite>("Spr_Frozenr"));
                    yield return new WaitForSeconds(10);
                    ec.RemoveTimeScale(timescale);
                    break;
                case "Framed":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).RuleBreak("Running", 30, 50);
                    NewGauge(30, BasePlugin.asm.Get<Sprite>("Spr_PlaceHolder"));
                    
                    
                    break;
                case "TooFast":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpikEnv2 = new TimeScaleModifier(1, 5f, 1);
                    ec.AddTimeScale(timeScaleEpikEnv2);
                    Singleton<CoreGameManager>.Instance.GetHud(0).BaldiTv.ShowLevelTimeWarning(ec);
                    NewGauge(10, BasePlugin.asm.Get<Sprite>("Spr_EnvSpeedUp"));
                    yield return new WaitForSeconds(10f);
                    ec.RemoveTimeScale(timeScaleEpikEnv2);
                    break;
                case "BaldiIsHappier":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    ec.GetBaldi().GetAngry(-4f);
                    
                    break;
                case "TpRandomRoom":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    
                    var cells4 = ec.AllTilesNoGarbage(false, false); ;
                    var rndcell = cells4[UnityEngine.Random.Range(0, cells4.Count)];
                    while (!rndcell.room)
                    {
                        rndcell = cells4[UnityEngine.Random.Range(0, cells4.Count)];
                    }
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(new Vector3(rndcell.position.x, 0, rndcell.position.z));
                    break;
                case "CrazyNpcSpeed":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpik99 = new TimeScaleModifier(20f, 1, 1);
                    ec.AddTimeScale(timeScaleEpik99);
                    NewGauge(durationCommandNpc, BasePlugin.asm.Get<Sprite>("Spr_NpcExtremeSpeedUp"));
                    yield return new WaitForSeconds(durationCommandNpc);
                    ec.RemoveTimeScale(timeScaleEpik99);
                    durationCommandNpc += 2;
                    break;
                case "Negative500stamina":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina = -500;
                    break;







            }
            yield return "";
        }

    }

    abstract class CommandsConstruct
    {
        internal abstract IEnumerator Execute([Optional] object[] args);
    }
    class SpeedCommandsHandler : CommandsConstruct
    {
        Commands Cmd;
        EnvironmentController ec;
        internal override IEnumerator Execute([Optional] object[] args)
        {
            // 0 == Type Of Command
            // 1 == EnviromnentController
            Cmd = BasePlugin.Instance.CPH.CmdsComp;
            ec = (EnvironmentController)args[1];
            var timeScale= new TimeScaleModifier(1.3f, 1, 1);
            var sprite = "Spr_NpcSpeedUp";
            var duration = 30f;
            switch ((string)args[0])
            {
                case "Player":
                    sprite = "Spr_PlrSpeedUp";
                    timeScale = new TimeScaleModifier(1f, 1.3f, 1);
                    break;
                case "Npc":
                    sprite = "Spr_NpcSpeedUp";
                    timeScale = new TimeScaleModifier(1.3f, 1f, 1);
                    break;
                case "Env":
                    sprite = "Spr_EnvSpeedUp";
                    timeScale = new TimeScaleModifier(1f, 1f, 1.3f);
                    break;

            }
            ec.AddTimeScale(timeScale);
            Cmd.NewGauge(duration, BasePlugin.asm.Get<Sprite>(sprite));
            yield return new WaitForSeconds(duration);
            ec.RemoveTimeScale(timeScale);
        }
    }
    class SpawnNpcHandler : CommandsConstruct
    {
        Commands Cmd;
        EnvironmentController ec;
        NPC[] npcs;
        // 0 = Npc
        // 1 = Duration
        // 2 = How many
        // 3 = Ec
        // 4 = Special Type
        
        internal override IEnumerator Execute([Optional] object[] args)
        {
            Cmd = BasePlugin.Instance.CPH.CmdsComp;
            ec = (EnvironmentController)args[3];
            var cells = ec.AllTilesNoGarbage(false, false);
            var npc = args[0];
            var duration = (float)args[1];
            var howMany = (int)args[2];
            var specialType = (string)args[4];  
            for (int i = 0; i < howMany; i++)
            {

                npcs = npcs.AddToArray(ec.SpawnNPC((NPC)npc, cells[UnityEngine.Random.Range(0, cells.Count)].position));
                yield return new WaitForEndOfFrame();
            }
            if (specialType == "baldi")
            {
                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.asm.Get<SoundObject>("Mus_Ohnoes"));
            }
            if (duration > 0.1f)
            {
                Cmd.NewGauge(duration, BasePlugin.asm.Get<Sprite>("Spr_EnvSpeedUp"));
                yield return new WaitForSeconds(duration);
                foreach (var item in npcs)
                {
                    item.Despawn();
                }
            }
        }
    }
}
