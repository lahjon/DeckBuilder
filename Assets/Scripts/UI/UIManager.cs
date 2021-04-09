using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Manager
{
    public int planeDistance = 10;
    public UIWarningController UIWarningController;
    public DeathScreen deathScreen;
    public EncounterUI encounterUI;
    public DebugUI debugUI;
    public EscapeMenu escapeMenu;

    public Dictionary<string, Sprite> statusEffectIcons = new Dictionary<string, Sprite>();
    public List<NamedSprite> namedEffectIcons = new List<NamedSprite>();

    protected override void Awake()
    {
        base.Awake(); 
        world.uiManager = this;
        foreach (NamedSprite namedSprite in namedEffectIcons)
            statusEffectIcons[namedSprite.name] = namedSprite.sprite;
    }

    public Sprite GetSpriteByName(string strSprite)
    {
        if (!statusEffectIcons.ContainsKey(strSprite))
        {
            Debug.LogError("Requested non-existant icon for status effect: " + strSprite);
            return null;
        }
        else
            return statusEffectIcons[strSprite];
    }

}
