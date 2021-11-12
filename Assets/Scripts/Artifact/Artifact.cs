using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Artifact : MonoBehaviour, IToolTipable
{
    public ItemEffect effect;
    public int id;
    protected Image image;
    public Button button;
    protected ArtifactData _artifactData;
    public Transform tooltipAnchor;
    bool initialized;
    public ArtifactData artifactData
    {
        get => _artifactData;
        set
        {
            _artifactData = value;
            BindData();
        }
    }
    protected void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (!initialized)
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            initialized = true;   
        }
    }

    protected void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public virtual (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>{0}</b>\n{1}", artifactData.itemName, artifactData.description);
        return (new List<string>{desc} , pos);
    }
    public void OnClick()
    {

    }
    public virtual void BindData(bool allData = true)
    {   
        Initialize();
        if (artifactData != null)
        {
            image.sprite = artifactData.artwork;
            id = artifactData.itemId;
        }
    }
}

