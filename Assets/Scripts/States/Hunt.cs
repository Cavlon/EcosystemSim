using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Hunt : Wander
{
    Bear bear;
    FOVTrigger fovTrigger;
    int energyYield;
    int instVal;
    Transform targetTrans;
    bool initialFind = true;
    bool newPath;
    bool eating;
    float fovDistance;

    public Hunt(Bear entity, int instVal) : base(entity)
    {
        bear = entity;
        base.entity = entity;
        this.instVal = instVal;
    }

    public override void Tick()
    {
        if (bear.caughtPrey && !eating)
        {
            entity.energyYield += energyYield;
            eating = true;
        }

        if (bear.hunting)
        {
            move = bear.canMove;
            if (((target - trans.position).sqrMagnitude > Mathf.Pow(fovDistance + 5, 2) || targetTrans == null) && !eating)
            {
                fovTrigger.EndContact();
                bear.hunting = false;
                resourceFound = false;
                targetTrans = null;
                initialFind = true;
                newPath = false;
                if (instVal == 0)
                {
                    entity.UpdateStateText("Passively Hunting");
                }
                else
                {
                    entity.UpdateStateText("Actively Hunting");
                }
                fovTrigger.FindTarget("Deer", false);
                NewRandTarget(trans.position, false);
            }
            if (newPath && targetTrans != null)
            {
                target = targetTrans.position;
                newPath = false;
                NewPath();
                RegisterNewPath();
            }
            
        }

        if (move)
        {          
            if (!eating)
            {
                WanderTick();
            } else
            {
                entity.RemoveInstruction(instVal);
            }
        }        

        if (fovTrigger.found && targetTrans != fovTrigger.targetTrans && !eating)
        {
            resourceFound = true;
            targetTrans = fovTrigger.targetTrans;
            bear.hunting = true;
            newPath = true;

            if (initialFind)
            {
                entity.UpdateStateText("Chasing Prey");
                initialFind = false;
            }
        }
    }

    public override void OnStateExit()
    {
        fovTrigger.EndContact();
        bear.caughtPrey = false;
        bear.hunting = false;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        fovTrigger = entity.fovTrig;
        fovTrigger.FindTarget("Deer", false);
        energyYield = (int)(Random.Range(10, 16) * entity.efficiency);
        fovDistance = entity.fovDistance;

        if (instVal == 0)
        {
            entity.UpdateStateText("Passively Hunting");
        }
        else
        {
            entity.UpdateStateText("Actively Hunting");
        }
    }

    public async void RegisterNewPath()
    {
        await Task.Delay(1000);
        newPath = true;
    }
}
