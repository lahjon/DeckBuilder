using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class DebugManager : MonoBehaviour
{
    bool showConsole;
    bool showHelp;
    string input;
    public static DebugCommand SaveGame, LoadGame, Help;
    public static DebugCommand<int> AddGold, AddShards, Heal, AddEnergy, AddEnergyTurn, AddRageTurn;
    public static DebugCommand<string> AddCard, RemoveCard, UnlockProfession;
    public List<object> commandList;
    public string lastCommand;
    public Dictionary<string, string> words = new Dictionary<string, string>();
    WorldSystem world;
    Vector2 scroll;
    List<string> found = new List<string>();

    void Awake()
    {
        Help = new DebugCommand("help", "shows all commands", "help", () =>
            {
                showHelp = true;
                Debug.Log("Help");
            }
        );
        SaveGame = new DebugCommand("save", "saves the game", "save", () =>
            {
                world.SaveProgression();
                Debug.Log("Save");
            }
        );
        LoadGame = new DebugCommand("load", "loads the game", "load", () =>
            {
                world.LoadProgression();
                Debug.Log("Load");
            }
        );
        AddGold = new DebugCommand<int>("add_gold", "add x gold", "add_gold <shard_amount>", (x) =>
            {
                world.characterManager.characterCurrency.gold += x;
                Debug.Log("Gold: " + x);
            }
        );
        AddShards = new DebugCommand<int>("add_shards", "add x shards.", "add_shards <gold_amount>", (x) =>
            {
                world.characterManager.characterCurrency.shard += x;
                Debug.Log("Shards: " + x);
            }
        );
        Heal = new DebugCommand<int>("heal", "heal x amount", "heal <heal_amount>", (x) =>
            {
                world.characterManager.Heal(x);
                Debug.Log("Heal: " + x);
            }
        );
        AddCard = new DebugCommand<string>("add_card", "add card to deck", "add_card <card_name>", (x) =>
            {
                if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
                {
                    CardActivityAddCardToCombat addIt = new CardActivityAddCardToCombat();

                    CardActivitySetting data = new CardActivitySetting(new CardActivityData() { strParameter = x, type = CardActivityType.DrawCard });
                    StartCoroutine(addIt.Execute(data));
                }
            }
        );
        RemoveCard = new DebugCommand<string>("remove_card", "remove card from deck", "remove_card <card_name>", (x) =>
            {
                if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
                {
                    List<Card> cards = CombatSystem.instance.Hero.deck;
                    Card card = cards.Where(c => c.name == x).FirstOrDefault();
                    if(card != null)
                        CombatSystem.instance.Hero.deck.Remove(card);
                    Debug.Log("Remove Card: " + x);
                }
            }
        );
        AddEnergyTurn = new DebugCommand<int>("add_energy_turn", "add energy per turn", "add_energy_turn <value>", (x) =>
            {
                if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
                {
                    CombatSystem.instance.ModifyEnergy(new Dictionary<EnergyType, int>() { { EnergyType.Standard, x } });
                    CombatSystem.instance.energyTurn[EnergyType.Standard] += x;
                    Debug.Log("Added energy per turn: " + x);
                }
            }
        );
        AddRageTurn = new DebugCommand<int>("add_rage_turn", "add rage per turn", "add_rage_turn <value>", (x) =>
        {
            if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            {
                CombatSystem.instance.ModifyEnergy(new Dictionary<EnergyType, int>() { { EnergyType.Rage, x } });
                CombatSystem.instance.energyTurn[EnergyType.Rage] += x;
                Debug.Log("Added energy per turn: " + x);
            }
        }
        );
        AddEnergy = new DebugCommand<int>("add_energy", "add energy to player", "add_energy <amount>", (x) =>
            {
                if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
                {
                    CombatSystem.instance.ModifyEnergy(new Dictionary<EnergyType, int>() {{EnergyType.Standard, x}});
                    Debug.Log("added energy: " + x);
                }
            }
        );
        UnlockProfession = new DebugCommand<string>("unlock_profession", "unlock a profession", "unlock_profession <profession_type>", (x) =>
            {
                ProfessionType professionType;
                if (System.Enum.TryParse<ProfessionType>(x, out professionType))
                {
                    Debug.Log(professionType);
                    world.characterManager.UnlockProfession(professionType);
                    Debug.Log("unlocked profession: " + x);
                }
            }
        );

        commandList = new List<object>
        {
            Help,
            SaveGame, 
            LoadGame,
            AddGold,
            AddShards,
            Heal,
            AddCard,
            RemoveCard,
            AddEnergy,
            AddEnergyTurn,
            AddRageTurn,
            UnlockProfession

        };

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            words.Add(commandBase.commandFormat, commandBase.commandId);
        }
    }


    void Start()
    {
        world = WorldSystem.instance;
    }

    List<string> GetAllCards()
    {
        List<string> allCards = new List<string>();
        DatabaseSystem.instance.cards.ForEach(x => allCards.Add(x.name));
        return allCards;
    }

    public void OnEnterKey(InputAction.CallbackContext value)
    {
        if(showConsole)
        {
            HandleInput();
            input = "";
            if(lastCommand != "help")
                showConsole = false;
        }
    }

    public void OnArrowDown(InputAction.CallbackContext value)
    {
        if(showConsole && value.performed)
        {
            if (found != null && found.Any())
            {
                string[] output = found[0].Split(' ');
                input = output[0];
            }
        }
    }
    public void OnArrowUp(InputAction.CallbackContext value)
    {
        if(showConsole && value.performed)
        {
            Debug.Log("Up");
        }
    }
    public void OnSKey(InputAction.CallbackContext value)
    {
        if(!showConsole && value.performed)
        {
            world.SaveProgression();
            Debug.Log("Save Game");
        }
    }
    public void OnLKey(InputAction.CallbackContext value)
    {
        if(!showConsole && value.performed)
        {
            world.LoadProgression();
        }
    }


    public void OnToggleConsole(InputAction.CallbackContext value)
    {
        if(value.performed)
        {
            showConsole = !showConsole;
            showHelp = false;
        }
    }

    void OnGUI()
    {
        
        if (!showConsole)
        {
            input = "";
            return;
        }
        float y = 0.0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0.0f, 0.0f, Screen.width - 30.0f, 20.0f * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y + 5.0f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;
                string label = string.Format("{0} - {1}", command.commandFormat, command.commandDescription);
                Rect labelRect = new Rect(5.0f, 20.0f * i, viewport.width - 100.0f, 20.0f);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("Console");
        input = GUI.TextField(new Rect(10.0f, y + 5.0f, Screen.width - 20.0f, 20.0f), input);

        if (input != "")
        {
            lastCommand = input;
        }
        GUI.FocusControl("Console");

        if (!string.IsNullOrEmpty(input)) 
        {
            found = words.Keys.ToList().FindAll( w => w.StartsWith(input) );
        }
        if (found.Any())
        {
            y += 40;
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0.0f, 0.0f, Screen.width - 30.0f, 20.0f * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y + 5.0f, Screen.width, 90), scroll, viewport);

            for (int i = 0; i < found.Count; i++)
            {
                string label = found[i];
                Rect labelRect = new Rect(5.0f, 20.0f * i, viewport.width - 100.0f, 20.0f);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
        }

        if (input.Contains("add_card"))
        {
            y += 40;
            GUI.Box(new Rect(0, y, Screen.width, 100), "");
            Rect viewport = new Rect(0.0f, 0.0f, Screen.width - 30.0f, 20.0f * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y + 5.0f, Screen.width, 9000), scroll, viewport);

            string[] cards = GetAllCards().ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                string label = cards[i];
                Rect labelRect = new Rect(5.0f, 20.0f * i, viewport.width - 100.0f, 20.0f);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
        }

    }

    void HandleInput()
    {
        string[] properties = input.Split(' ');
        Debug.Log(properties);
        int index;

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.commandId))
            {
                if (commandList[i] is DebugCommand dc)
                {
                    dc.Invoke();
                }
                else if (commandList[i] is DebugCommand<int> dcInt && properties.Length > 1 && int.TryParse(properties[1], out index))
                {
                    dcInt.Invoke(index);
                }
                else if (commandList[i] is DebugCommand<string> dcString && properties.Length > 1 && properties[1] is string)
                {
                    dcString.Invoke(properties[1]);
                }
            }
        }
    }
}
