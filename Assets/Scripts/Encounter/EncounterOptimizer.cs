using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EncounterOptimizer
{
    List<Encounter> encounters;
    public int cScore;

    public int iterationLimit = 300;
    private int iteration;
    public int targetScore = 0;

    public OverworldEncounterType[] types = {
        OverworldEncounterType.RandomEvent,
        OverworldEncounterType.CombatNormal,
        OverworldEncounterType.CombatElite,
        OverworldEncounterType.Bonfire,
        OverworldEncounterType.Shop
    };

    public Dictionary<OverworldEncounterType, int[]> targets = new Dictionary<OverworldEncounterType, int[]>() {
        { OverworldEncounterType.RandomEvent,new int[]{0,2} },
        { OverworldEncounterType.CombatNormal,new int[]{0,100} },
        { OverworldEncounterType.CombatElite,new int[]{1,2} },
        { OverworldEncounterType.Bonfire,new int[]{1,1} },
        { OverworldEncounterType.Shop,new int[]{0,2} },
         };

    Dictionary<OverworldEncounterType, int> typeCounter = new Dictionary<OverworldEncounterType, int>();

    public HashSet<OverworldEncounterType> InARowOK = new HashSet<OverworldEncounterType>() { OverworldEncounterType.CombatNormal };

    public void SetEncounters(List<Encounter> encounters)
    {
        this.encounters = encounters;
        foreach (OverworldEncounterType type in types)
            typeCounter[type] = 0;

        foreach (Encounter enc in encounters)
            typeCounter[enc.encounterType]++;

        cScore = ScoreFull();
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

            ShuffleTypes();
            foreach(OverworldEncounterType type in types)
            {
                int probeScore = Probe(chosenEnc, type);
                if(probeScore > maxScore)
                {
                    maxScore = probeScore;
                    bestType = type;
                }
            }

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
            if (enc.encounterType == neigh.encounterType)
                retScore++;
            if (testType == neigh.encounterType)
                retScore--;
        }

        retScore += subScoreType(enc.encounterType, -1) - subScoreType(enc.encounterType);
        retScore += subScoreType(testType, 1) - subScoreType(testType);

        return retScore;

    }

    public int ScoreFull()
    {
        int retScore = 0;
        List<Encounter> visited = new List<Encounter>();
        

        foreach(Encounter enc in encounters)
        {
            List<Encounter> neighs = enc.neighboors.Except(visited).ToList();
            foreach(Encounter neigh in neighs)
                if (!InARowOK.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                    retScore--;

            visited.Add(enc);
        }

        foreach (OverworldEncounterType type in types)
            retScore += subScoreType(type);

        return retScore;
    }

    private int subScoreType(OverworldEncounterType type, int modifier = 0)
    {
        return      -Mathf.Min(0, (targets[type][0] + modifier) - typeCounter[type])
                    -Mathf.Min(0, typeCounter[type] - (targets[type][1] + modifier));
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
