using UnityEngine;
using System.Linq;
using System.Collections;

public class Deer : EntityBase
{

    [SerializeField] Transform Antlers;
    public bool flee;

    public override void Awake()
    {
        base.Awake();
        baseAnimal = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManageSimulation>().baseDeer;       
    }

    public override void StatAssign()
    {
        base.StatAssign();
        for (int i = 0; i < GameData.deerAvgStats.Length; i++)
        {
            GameData.deerAvgStats[i] = GameData.AverageValue(GameData.deerAvgStats[i], GameData.deerPop, stats[i]);
        }
        GameData.deerAvgAbnormality = GameData.AverageValue(GameData.deerAvgAbnormality, GameData.deerPop, abnormality);
        GameData.deerPop += 1;
    }

    public override void FindState()
    {
        if (!instructions.Any())
        {
            if (stateString != "Wander")
            {
                StartCoroutine(SetState(new Wander(this), 0.25f));
                stateString = "Wander";
            }
        }
        else
        {
            switch (instructions[instructions.Count - 1])
            {
                case 0:

                    if (stateString != "Passive Eat")
                    {
                        Eat(0);
                        stateString = "Passive Eat";
                    }

                    break;

                case 1:

                    if (stateString != "Reproduce")
                    {
                        StartCoroutine(SetState(new Reproduce(this), 0.5f));
                        stateString = "Reproduce";
                    }
                    break;

                case 2:

                    if (stateString != "Drink")
                    {
                        StartCoroutine(SetState(new Drink(this), 0.5f));
                        stateString = "Drink";
                    }
                    break;

                case 3:

                    if (stateString != "Active Eat")
                    {
                        Eat(3);
                        stateString = "Active Eat";
                    }

                    break;

                case 4:

                    if (stateString != "Flee")
                    {
                        StartCoroutine(SetState(new Flee(this), 0.25f));
                        stateString = "Flee";
                    }

                    break;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (flee && !instInStack[4])
        {
            AddInstruction(4);
        }
    }

    public override void Eat(int instVal)
    {
        StartCoroutine(SetState(new Forage(this, instVal), 0.5f));        
    }

    public override void Male()
    {
        base.Male();
        Instantiate(Antlers, trans);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < GameData.deerAvgStats.Length; i++)
        {
            GameData.deerAvgStats[i] = GameData.AverageValue(GameData.deerAvgStats[i], GameData.deerPop, stats[i], true);
        }
        GameData.deerAvgAbnormality = GameData.AverageValue(GameData.deerAvgAbnormality, GameData.deerPop, abnormality, true);
        GameData.deerPop -= 1;
    }
}
