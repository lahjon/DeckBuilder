using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldScenarioTooltip : MonoBehaviour
{
    public TMP_Text difficultyText;
    public TMP_Text scenarioName;
    public TMP_Text descriptionText;
    Reward scenarioReward;
    public Transform rewardAnchor;
    public void EnableTooltip(ScenarioUI scenario)
    {
        gameObject.SetActive(true);
        if (scenarioName.text != scenario.data.ScenarioName)
        {
            if (scenarioReward != null) Destroy(scenarioReward.gameObject);
            scenarioName.text = scenario.data.ScenarioName;
            scenarioReward = Instantiate(scenario.scenarioReward, rewardAnchor).GetComponent<Reward>();
            scenarioReward.transform.localPosition = Vector3.zero;
            scenarioReward.SetWorldReward();

            transform.position = scenario.transform.position + new Vector3(0, scenarioReward.GetComponent<RectTransform>().sizeDelta.x + 1, 0);;
            descriptionText.text = scenario.data.Description;
            scenarioReward.transform.localScale *= 1.85f;
            difficultyText.text = scenario.data.difficulty.ToString();
            scenarioReward.gameObject.SetActive(true);
        }
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }
}
