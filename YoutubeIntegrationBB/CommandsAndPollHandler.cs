using HarmonyLib;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using TMPro;
using UnityEngine;
using YTLiveChat.Contracts.Services;


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
            NewCommand("UseItem", "forcefully uses an item", "Disadvantage");
            NewCommand("NoStamina", "The stamina is set to 0", "Disadvantage");



        }
        public void Play()
        {
            Debug.Log("Starting Vote System...");
            CmdsComp = new GameObject().AddComponent<Commands>();
            DontDestroyOnLoad(CmdsComp.gameObject);

            votes = [0, 0, 0, 0, 0];
            usersAlreadyVoted = ["0"];
            canVote = false;
            
            BasePlugin.Instance.YoutubeHandle._ytLiveChat.InitialPageLoaded += (sender, e) =>
            {
                Debug.Log("YouTube LiveChat Connected!");
                BasePlugin.Instance.YoutubeHandle._ytLiveChat.ChatReceived += VoteHandler;
                StartCoroutine(VotePollHandler());
            };
            BasePlugin.Instance.YoutubeHandle.Begin(BasePlugin.Instance.VideoID.Value);

        }


        internal void VoteHandler(object? sender, ChatReceivedEventArgs e)
        {
            Debug.Log("New chat");
            foreach (var user in usersAlreadyVoted)
            {
                if (user != e.ChatItem.Author.ChannelId && canVote)
                {
                    for (int i = 1; i < 5; i++)
                    {

                        if (e.ChatItem.Message.Select(p => p.ToString()).ToString() == i.ToString()) 
                        {
                            votes[i - 1] += 1;
                            usersAlreadyVoted = usersAlreadyVoted.AddToArray(e.ChatItem.Author.ChannelId);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            Destroy(CmdsComp.gameObject);
            Destroy(inst);
            StopCoroutine(VotePollHandler());
            votes = [0, 0, 0, 0, 0];
            usersAlreadyVoted = ["0"];
            canVote = false;
            BasePlugin.Instance.YoutubeHandle._ytLiveChat.ChatReceived -= VoteHandler;
            foreach (var item in Choices)
            {
                Destroy(item.gameObject);
            }
            Choices = [];
            BasePlugin.Instance.YoutubeHandle.Stop("");

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
                            door.LockTimed(10);
                            yield return new WaitForSeconds(0.08f);
                        }
                    }

                    break;
                case "SpeedUpNpcs":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpik = new TimeScaleModifier(1.3f, 1, 1);
                    ec.AddTimeScale(timeScaleEpik);
                    yield return new WaitForSeconds(30f);
                    ec.RemoveTimeScale(timeScaleEpik);
                    break;
                case "SpeedUpEnvironment":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpikEnv = new TimeScaleModifier(1, 1.75f,1);
                    ec.AddTimeScale(timeScaleEpikEnv);
                    yield return new WaitForSeconds(30f);
                    ec.RemoveTimeScale(timeScaleEpikEnv);
                    break;
                case "SpeedUpPlayer":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var timeScaleEpikPlr = new TimeScaleModifier(1f, 1, 1.35f);
                    ec.AddTimeScale(timeScaleEpikPlr);
                    yield return new WaitForSeconds(30f);
                    ec.RemoveTimeScale(timeScaleEpikPlr);
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
                    var cells = ec.AllTilesNoGarbage(false, false);
                    ec.SpawnNPC(choosenNpc, cells[UnityEngine.Random.Range(0, cells.Count)].position);
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
                    yield return new WaitForSeconds(7f);
                    ec.RemoveTimeScale(timeScaleEpikPlr2);
                    break;
                case "NeverGonnaGiveYou":
                    Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                    break;
                case "Distraction":
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.asm.Get<SoundObject>("Mus_Distraction"));
                    for (int i = 0; i < 43; i++) {

                        yield return new WaitForSeconds(1f);
                        Singleton<BaseGameManager>.Instance.Ec.GetBaldi().Hear(Singleton<CoreGameManager>.Instance.GetPlayer(0).gameObject, Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position, 100, true);

                    }
                    break;
                case "Spawn99Chalkles":
                    ec = Singleton<BaseGameManager>.Instance.Ec;
                    var cells2 = ec.AllTilesNoGarbage(false, false);
                    for (int i = 0; i < 99; i++)
                    {
                        ec.SpawnNPC(NPCMetaStorage.Instance.All().First(x => (x.value.name == "ChalkFace")).value, cells2[UnityEngine.Random.Range(0, cells2.Count)].position);
                    }
                    
                    break;
                case "UseItem":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.UseItem();
                    break;
                case "NoStamina":
                    Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.stamina = 0;
                    break;

            }
            yield return "";
        }

    }
}
