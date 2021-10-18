using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Bear : EntityBase
{

    public float huntingProwess;
    [HideInInspector] public bool caughtPrey;
    [HideInInspector] public bool hunting;

    public override void Awake()
    {
        base.Awake();
        baseAnimal = GameObject.FindGameObjectWithTag("Manager").GetComponent<ManageSimulation>().baseBear;        
    }

    public override void StatAssign()
    {
        base.StatAssign();
        huntingProwess = stats[5] / 5;
        for (int i = 0; i < GameData.bearAvgStats.Length; i++)
        {
            GameData.bearAvgStats[i] = GameData.AverageValue(GameData.bearAvgStats[i], GameData.bearPop, stats[i]);
        }
        GameData.bearAvgAbnormality = GameData.AverageValue(GameData.bearAvgAbnormality, GameData.bearPop, abnormality);
        GameData.bearPop += 1;
    }

    public override void Eat(int instVal)
    {
        StartCoroutine(SetState(new Hunt(this, instVal), 0.5f));
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
        if (hunting && !caughtPrey && collision.transform.CompareTag("Deer"))
        {
            int randVal = Random.Range(0, 101);
            float delay = 2f;
            if ((randVal / 100) < huntingProwess)
            {
                Destroy(collision.gameObject);
                caughtPrey = true;
                delay = efficiency;
                UpdateStateText("Eating");
            }
            canMove = false;
            StartCoroutine(HuntDelay(delay));
        }
    }

    IEnumerator HuntDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < GameData.bearAvgStats.Length; i++)
        {
            GameData.bearAvgStats[i] = GameData.AverageValue(GameData.bearAvgStats[i], GameData.bearPop, stats[i], true);
        }
        GameData.bearAvgAbnormality = GameData.AverageValue(GameData.bearAvgAbnormality, GameData.bearPop, abnormality, true);
        GameData.bearPop -= 1;
    }
}
