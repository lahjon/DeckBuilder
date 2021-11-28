using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HexOptimizer
{
    List<Encounter> encounters;
    TileType tileType;
    public int cScore;

    public int iterationLimit = 50;
    private int iteration;
    public int targetScore = 0;

    public OverworldEncounterType[] types = {
        OverworldEncounterType.Choice,
        OverworldEncounterType.CombatNormal,
        OverworldEncounterType.CombatElite,
        OverworldEncounterType.Blacksmith,
        OverworldEncounterType.Bonfire,
        OverworldEncounterType.Shop
    };

    Dictionary<OverworldEncounterType, int> totalCounts = new Dictionary<OverworldEncounterType, int>();
    Dictionary<OverworldEncounterType, int> fixedCounts = new Dictionary<OverworldEncounterType, int>();

    public List<OverworldEncounterType> commons = new List<OverworldEncounterType>() {
        OverworldEncounterType.CombatNormal,
        OverworldEncounterType.Choice
    };

    public List<OverworldEncounterType> uncommons = new List<OverworldEncounterType>() {
        OverworldEncounterType.CombatElite,
        OverworldEncounterType.Shop,
        OverworldEncounterType.Bonfire,
        OverworldEncounterType.Blacksmith
    };

    public Dictionary<OverworldEncounterType, float> proportions = new Dictionary<OverworldEncounterType, float>();



    public void SetEncounters(List<Encounter> encounters, TileType tileType)
    {
        this.encounters = new List<Encounter>(encounters);
        this.tileType = tileType;
        Debug.Log(tileType);
        foreach (OverworldEncounterType type in types)
        {
            fixedCounts[type] = Helpers.tileTypeToFixedAmounts[tileType][type];
            proportions[type] = Helpers.TileTypeToProportions[tileType][type];
        }

        DrawTargets();

        //init with correct targets
        ShuffleEncounters();
        int cursor = 0;
        foreach(OverworldEncounterType type in types)
            for(int i = 0; i < totalCounts[type]; i++)
                this.encounters[cursor++].encounterType = type;

        cScore = ScoreFull();
        iteration = 0;
    }

    public void Run()
    {
        int resetLimit = 10;
        int resetCount = 0;
        while (cScore != 0 && iteration++ < iterationLimit)
        {
            int idEnc = Random.Range(0, encounters.Count);
            Encounter chosenEnc = encounters[idEnc];
            Encounter bestSwap = null;
            OverworldEncounterType origType = chosenEnc.encounterType;

            int maxScore = cScore;

            ShuffleEncounters();

            foreach (Encounter swapEnc in encounters)
            {
                int probeScore = chosenEnc.encounterType == swapEnc.encounterType ? cScore : Probe(chosenEnc, swapEnc);
                //Debug.Log(string.Format("{0},{1}: {2} --> {3}, score {4} --> {5}", iteration, chosenEnc.name, original, type, cScore, probeScore));
                if (probeScore >= maxScore)
                {
                    maxScore = probeScore;
                    bestSwap = swapEnc;
                }
            }

            resetCount = cScore == maxScore ? resetCount + 1 : 0;
            cScore = maxScore;

            chosenEnc.encounterType = bestSwap.encounterType;
            bestSwap.encounterType = origType;

            if (resetCount == resetLimit)
            {
                Debug.Log("restarting hexOptimizer");
                resetCount = 0;
                SetEncounters(encounters, tileType);
            }
        }
    }

    private int Probe(Encounter enc, Encounter swapEnc)
    {
        int retScore = 0;
        // hur mycket ändras score på originalplats
        foreach (Encounter neigh in enc.neighboors)
        {
            retScore -= NeighPenalty(enc.encounterType, neigh.encounterType);
            retScore += NeighPenalty(swapEnc.encounterType, neigh.encounterType);
        }

        // hur mycket pajar den dit den hamnar
        foreach (Encounter neigh in swapEnc.neighboors)
        {
            retScore += NeighPenalty(enc.encounterType, neigh.encounterType);
            retScore -= NeighPenalty(swapEnc.encounterType, neigh.encounterType);
        }
        return cScore + retScore;
    }

    public int NeighPenalty(OverworldEncounterType type1, OverworldEncounterType type2)
    {
        if (commons.Contains(type1) || commons.Contains(type2)) return 0;
        return type1 == type2 ? -10 : 0;
    }

    public int ScorePartial(Encounter enc, OverworldEncounterType testType)
    {
        int retScore = 0;
        if (enc.encounterType == testType) return retScore;

        foreach (Encounter neigh in enc.neighboors)
        {
            if (!commons.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                retScore += 10;
            if (!commons.Contains(testType) && testType == neigh.encounterType)
                retScore -= 10;
        }

        return retScore;
    }

    public int ScoreFull()
    {
        int retScore = 0;
        List<Encounter> visited = new List<Encounter>();
        foreach (Encounter enc in encounters)
        {
            visited.Add(enc);
            if (commons.Contains(enc.encounterType)) continue;

            foreach (Encounter neigh in enc.neighboors.Except(visited))
                if (enc.encounterType == neigh.encounterType)
                    retScore -= 10;
        }

        return retScore;
    }

    private void ShuffleEncounters()
    {
        for (int i = 0; i < encounters.Count - 1; i++)
        {
            Encounter temp = encounters[i];
            int index = Random.Range(i, encounters.Count);
            encounters[i] = encounters[index];
            encounters[index] = temp;
        }
    }

    private void DrawTargets()
    {
        int totalFreeSpaces = encounters.Count - fixedCounts.Values.Sum();
        int slotsUncommon   = (int)(Random.Range(0.4f, 0.5f) * totalFreeSpaces);
        int slotsCommon     = totalFreeSpaces - slotsUncommon;

        for (int i = 0; i < types.Length; i++)
            totalCounts[types[i]] = 0; 

        InitTargetGroup(commons, slotsCommon);
        InitTargetGroup(uncommons, slotsUncommon);

        for (int i = 0; i < types.Length; i++)
            totalCounts[types[i]]+= fixedCounts[types[i]];
    }

    private void InitTargetGroup(List<OverworldEncounterType> group, int slots)
    {
        float[] rands = new float[group.Count];
        float sum = 0;
        for (int i = 0; i < group.Count; i++)
            sum += (rands[i] = Random.Range(0f, 1f) * proportions[group[i]]);
        for (int i = 0; i < group.Count; i++)
            rands[i] /= sum;

        float[] currentWeights = new float[group.Count];
        
        for (int i = 0; i < slots; i++)
        {
            float maxDistance = float.MinValue;
            int maxIndex = 0;
            for (int j = 0; j < group.Count; j++)
            {
                if (rands[j] == 0f) continue;

                if (rands[j] - currentWeights[j] > maxDistance)
                {
                    maxDistance = rands[j] - currentWeights[j];
                    maxIndex = j;
                }
            }
            totalCounts[group[maxIndex]]++;

            for (int j = 0; j < group.Count; j++)
                currentWeights[j] = totalCounts[types[j]] / (i + 1);
        }
    }

}
