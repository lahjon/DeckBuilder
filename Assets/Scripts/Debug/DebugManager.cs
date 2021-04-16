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
    public static DebugCommand<int> GoldAdd, ShardsAdd, Heal;
    public static DebugCommand<string> CardAdd, CardRemove;
    public List<object> commandList;
    public string lastCommand;
    public Dictionary<string, string> words = new Dictionary<string, string>();
    WorldSystem world;
    Vector2 scroll;

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
            Debug.Log("Save");
        }
        );
        LoadGame = new DebugCommand("load", "loads the game", "load", () =>
        {
            Debug.Log("Load");
        }
        );
        GoldAdd = new DebugCommand<int>("gold_add", "add x gold", "gold_add <shard_amount>", (x) =>
        {
            Debug.Log("Gold: " + x);
        }
        );
        ShardsAdd = new DebugCommand<int>("shards_add", "add x shards.", "shards_add <gold_amount>", (x) =>
        {
            Debug.Log("Shards: " + x);
        }
        );
        Heal = new DebugCommand<int>("heal", "heal x amount", "heal <heal_amount>", (x) =>
        {
            Debug.Log("Heal: " + x);
        }
        );
        CardAdd = new DebugCommand<string>("card_add", "add card to deck", "card_add <card_name>", (x) =>
        {
            Debug.Log("Add Card: " + x);
        }
        );
        CardRemove = new DebugCommand<string>("card_remove", "remove card from deck", "card_remove <card_name>", (x) =>
        {
            Debug.Log("Remove Card: " + x);
        }
        );

        commandList = new List<object>
        {
            Help,
            SaveGame, 
            LoadGame,
            GoldAdd,
            ShardsAdd,
            Heal,
            CardAdd,
            CardRemove
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


    public void OnToggleConsole(InputAction.CallbackContext value)
    {
        showConsole = !showConsole;
        showHelp = false;
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

        List<string> found = new List<string>();
        if (!string.IsNullOrEmpty(input)) 
        {
            found = words.Keys.ToList().FindAll( w => w.StartsWith(input) );
        }
        if (found.Count > 0)
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

    }

    void HandleInput()
    {
        string[] properties = input.Split(' ');
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
                else if (commandList[i] is DebugCommand<int> dcInt && properties.Length > 0 && int.TryParse(properties[1], out index))
                {
                    dcInt.Invoke(index);
                }
                else if (commandList[i] is DebugCommand<string> dcString && properties.Length > 0 && properties[1] is string)
                {
                    dcString.Invoke(properties[1]);
                }
            }
        }
    }
}
