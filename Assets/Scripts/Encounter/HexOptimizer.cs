﻿using System.Collections.Generic;
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


    public Dictionary<OverworldEncounterType, float> commons = new Dictionary<OverworldEncounterType, float>() {
        {OverworldEncounterType.CombatNormal, 1f},
        {OverworldEncounterType.RandomEvent, 1f},
    };

    public Dictionary<OverworldEncounterType, float> uncommons = new Dictionary<OverworldEncounterType, float>() {
        {OverworldEncounterType.CombatElite, 1f},
        {OverworldEncounterType.Shop, 1f},
        {OverworldEncounterType.Bonfire, 1f}
    };

    public void SetEncounters(List<Encounter> encounters)
    {
        this.encounters = new List<Encounter>(encounters);
        foreach (OverworldEncounterType type in types)
            targets[type] = 0;
        
        DrawTargets();

        //init with correct targets
        ShuffleEncounters();
        int cursor = 0;
        foreach(OverworldEncounterType type in types)
            for(int i = 0; i < targets[type]; i++)
                this.encounters[cursor++].encounterType = type;

        foreach (OverworldEncounterType type in types)
        {
            typeCounter[type] = targets[type];
            Debug.Log(type + ":" + targets[type]);
        }

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

            int maxScore = cScore;
            OverworldEncounterType bestType = chosenEnc.encounterType;

            OverworldEncounterType original = chosenEnc.encounterType;
            ShuffleTypes();
            foreach (OverworldEncounterType type in types)
            {
                int probeScore = Probe(chosenEnc, type);
                //Debug.Log(string.Format("{0},{1}: {2} --> {3}, score {4} --> {5}", iteration, chosenEnc.name, original, type, cScore, probeScore));
                if (probeScore > maxScore)
                {
                    maxScore = probeScore;
                    bestType = type;
                }
            }

            typeCounter[original] -= 1;
            typeCounter[bestType] += 1;
            resetCount = cScore == maxScore ? resetCount + 1 : 0;

            cScore = maxScore;
            chosenEnc.encounterType = bestType;

            if (resetCount == resetLimit)
            {
                Debug.Log("restarting hexOptimizer");
                resetCount = 0;
                SetEncounters(encounters);
            }
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

        foreach (Encounter neigh in enc.neighboors)
        {
            if (!commons.Keys.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                retScore += 10;
            if (!commons.Keys.Contains(testType) && testType == neigh.encounterType)
                retScore -= 10;
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
                if (!commons.Keys.Contains(enc.encounterType) && enc.encounterType == neigh.encounterType)
                    retScore -= 10;

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
        for (int i = 0; i < types.Length - 1; i++)
        {
            OverworldEncounterType temp = types[i];
            int index = Random.Range(i, types.Length);
            types[i] = types[index];
            types[index] = temp;
        }
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
        int slotsUncommon   = (int)(Random.Range(0.4f, 0.5f) * encounters.Count);
        int slotsCommon     = encounters.Count - slotsUncommon;

        InitTargetGroup(commons, slotsCommon);
        InitTargetGroup(uncommons, slotsUncommon);
       
    }

    private void InitTargetGroup(Dictionary<OverworldEncounterType,float> group, int slots)
    {
        List<OverworldEncounterType> types = group.Keys.ToList();
        float[] rands = new float[types.Count];
        float sum = 0;
        for (int i = 0; i < group.Keys.Count; i++)
            sum += (rands[i] = Random.Range(0f, 1f) * group[types[i]]);
        for (int i = 0; i < types.Count; i++)
            rands[i] /= sum;

        float[] currentWeights = new float[types.Count];
        for (int i = 0; i < slots; i++)
        {
            float maxDistance = float.MinValue;
            int maxIndex = 0;
            for (int j = 0; j < types.Count; j++)
            {
                if (rands[j] - currentWeights[j] > maxDistance)
                {
                    maxDistance = rands[j] - currentWeights[j];
                    maxIndex = j;
                }
            }
            targets[types[maxIndex]]++;

            for (int j = 0; j < types.Count; j++)
                currentWeights[j] = targets[types[j]] / (i + 1);
        }
    }

}