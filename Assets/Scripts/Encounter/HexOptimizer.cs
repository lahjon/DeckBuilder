using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HexOptimizer
{
    List<Encounter> encounters;
    public int cScore;

    public int iterationLimit = 50;
    private int iteration;
    public int targetScore = 0;

    public OverworldEncounterType[] types = {
        OverworldEncounterType.RandomEvent,
        OverworldEncounterType.CombatNormal,
        OverworldEncounterType.CombatElite,
        OverworldEncounterType.Bonfire,
        OverworldEncounterType.Shop
    };

    Dictionary<OverworldEncounterType, int> targets = new Dictionary<OverworldEncounterType, int>();
    Dictionary<OverworldEncounterType, int> typeCounter = new Dictionary<OverworldEncounterType, int>();


    HashSet<OverworldEncounterType> Commons = new HashSet<OverworldEncounterType>() { 
        OverworldEncounterType.CombatNormal, 
        OverworldEncounterType.RandomEvent 
    };

    public void SetEncounters(List<Encounter> encounters)
    {
        this.encounters = encounters;
        foreach (OverworldEncounterType type in types)
            typeCounter[type] = 0;

        foreach (Encounter enc in encounters)
            typeCounter[enc.encounterType]++;

        int uncommons = (int)(Random.Range(0.4f, 0.5f) * encounters.Count);
        int commons = encounters.Count - uncommons;
        targets[OverworldEncounterType.CombatNormal] = Random.Range(0,commons);
        targets[OverworldEncounterType.RandomEvent] = commons - targets[OverworldEncounterType.CombatNormal];

        targets[OverworldEncounterType.CombatElite] = Random.Range(0, uncommons + 1);
        uncommons -= targets[OverworldEncounterType.CombatElite];
        targets[OverworldEncounterType.Bonfire] = Random.Range(0, uncommons + 1);
        uncommons -= targets[OverworldEncounterType.Bonfire];
        targets[OverworldEncounterType.Shop] = uncommons;

        foreach (OverworldEncounterType type in types)
            Debug.Log(targets[type]);


        cScore = ScoreFull();
        Debug.Log("score: " + cScore);
        iteration = 0;
    }

    public void Run()
    {
        while(cScore != 0 && iteration++ < iterationLimit)
        {
            int idEnc = Random.Range(0, encounters.Count);
            Encounter chosenEnc = encounters[idEnc];

            int maxScore = cScore;
            OverworldEncounterType bestType = chosenEnc.encounterType;

            OverworldEncounterType original = chosenEnc.encounterType;
            ShuffleTypes();
            foreach(OverworldEncounterType type in types)
            {
                int probeScore = Probe(chosenEnc, type);
                //Debug.Log(string.Format("{0},{1}: {2} --> {3}, score {4} --> {5}", iteration, chosenEnc.name, original, type, cScore, probeScore));
                if(probeScore > maxScore)
                {
                    maxScore = probeScore;
                    bestType = type;
                }
            }

            typeCounter[original] -= 1;
            typeCounter[bestType] += 1;
            cScore = maxScore;
            chosenEnc.encounterType = bestType;
        }
    }

    private int Probe(Encounter enc, OverworldEncounterType type)
    {
        return cScore + ScorePartial(enc, type);
    }

    public int ScorePartial(Encounter enc, OverworldEncounterType testType)
    {
        int retScore = 0;
        if (enc.encounterType == testType) return retScore;
        
        foreach(Encounter neigh in enc.neighboors)
        {
            if (!Commons.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                retScore+=10;
            if (!Commons.Contains(testType) &&  testType == neigh.encounterType)
                retScore-=10;
        }

        //Remove old partial subscore
        retScore += subScoreType(enc.encounterType, -1) - subScoreType(enc.encounterType);
        //Calculate new
        retScore += subScoreType(testType, 1) - subScoreType(testType);

        return retScore;

    }

    public int ScoreFull()
    {
        int retScore = 0;
        List<Encounter> visited = new List<Encounter>();
        foreach (Encounter enc in encounters)
        {
            foreach (Encounter neigh in enc.neighboors.Except(visited))
                if (!Commons.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                    retScore-=10;

            visited.Add(enc);
        }

        foreach (OverworldEncounterType type in types)
            retScore += subScoreType(type);

        return retScore;
    }

    private int subScoreType(OverworldEncounterType type, int modifier = 0)
    {
        return -Mathf.Abs(targets[type] - (typeCounter[type] + modifier));
    }

    private void ShuffleTypes()
    {
        for(int i = 0; i < types.Length-1; i++)
        {
            OverworldEncounterType temp = types[i];
            int index = Random.Range(i, types.Length);
            types[i] = types[index];
            types[index] = temp;
        }
    }
}
