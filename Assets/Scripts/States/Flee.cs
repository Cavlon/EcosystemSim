using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Wander
{

    FOVTrigger fovTrigger;
    Transform targetTrans;
    float fovDistance;

    public Flee(Deer entity) : base(entity)
    {
        base.entity = entity;
    }

    public override void Tick()
    {
        if (move)
        {
            WanderTick();
        }

        if (fovTrigger.found && targetTrans != fovTrigger.targetTrans)
        {
            resourceFound = true;
            targetTrans = fovTrigger.targetTrans;
            target = newTarget(targetTrans.position);
            NewPath();
            entity.UpdateStateText("Fleeing");
        }

        if (resourceFound && pathEnd)
        {
            entity.RemoveInstruction(4);
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        fovTrigger = entity.fovTrig;
        fovTrigger.FindTarget("Bear", false);
        fovDistance = entity.fovDistance;
        pathEnd = false;
    }

    public override void OnStateExit()
    {
        fovTrigger.EndContact();
    }

    Vector3 newTarget(Vector3 bearPos)
    {
        Vector3 bearDir = (bearPos - trans.position).normalized;
        bearDir = -bearDir;
        Vector3 newPos = bearDir * fovDistance * 2.5f;
        newPos += new Vector3(Random.Range(-3, 4), 0, Random.Range(-3, 4));
        return newPos;
    }
}
