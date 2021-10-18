using UnityEngine;

public class Forage : Wander
{

    FOVTrigger fovTrigger;
    int energyYield;
    int instVal;
    bool eating;
    float eatTime;
    Transform targetTrans;
    bool initialFind = true;

    public Forage(EntityBase entity, int instVal) : base(entity)
    {
        base.entity = entity;
        this.instVal = instVal;
    }

    public override void Tick()
    {
        if (move)
        {
            WanderTick();
        }

        if (resourceFound && targetTrans == null && !eating)
        {
            fovTrigger.EndContact();
            resourceFound = false;
            targetTrans = null;
            initialFind = true;
            if (instVal == 0)
            {
                entity.UpdateStateText("Passively Foraging");
            }
            else
            {
                entity.UpdateStateText("Actively Foraging");
            }
            fovTrigger.FindTarget("Food", false);
        }

        if (fovTrigger.found && targetTrans != fovTrigger.targetTrans)
        {
            resourceFound = true;
            target = fovTrigger.targetPos;
            targetTrans = fovTrigger.targetTrans;
            NewPath();
            if (initialFind)
            {
                entity.UpdateStateText("Found Food");
                initialFind = false;
            }
        }

        if (pathEnd && resourceFound && !eating)
        {
            entity.UpdateStateText("Eating");
            eating = true;
            entity.energyYield += energyYield;
        }

        if (eatTime <= 0)
        {
            if (targetTrans != null)
            {
                Object.Destroy(targetTrans.gameObject);
            }           
            entity.RemoveInstruction(instVal);
        }

        if (eating)
        {
            eatTime -= Time.deltaTime;
        }
    }

    public override void OnStateExit()
    {
        fovTrigger.EndContact();
        if (entity.energy > 200)
        {
            if (targetTrans != null)
            {
                Object.Destroy(targetTrans.gameObject);
            }
            entity.RemoveInstruction(instVal);
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        fovTrigger = entity.fovTrig;
        fovTrigger.FindTarget("Food", false);
        pathEnd = false;
        energyYield = (int)(Random.Range(5, 16) * entity.efficiency);
        eatTime = entity.efficiency;

        if (instVal == 0)
        {
            entity.UpdateStateText("Passively Foraging");
        } else
        {
            entity.UpdateStateText("Actively Foraging");
        }
    }
}
