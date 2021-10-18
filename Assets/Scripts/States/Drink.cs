using UnityEngine;

public class Drink : Wander
{

    FOVTrigger fovTrigger;
    bool stringUpdated;

    public Drink(EntityBase entity) : base(entity)
    {
        base.entity = entity;
    }

    public override void Tick()
    {
        if (move)
        {
            WanderTick();          
        }
        if (fovTrigger.found)
        {
            target = fovTrigger.targetPos + trans.position;
            NewPath();
            resourceFound = true;
            fovTrigger.EndContact();
            entity.UpdateStateText("Found Water");
        }
        if (pathEnd && resourceFound)
        {            
            if (!stringUpdated)
            {
                entity.UpdateStateText("Drinking");
                stringUpdated = true;
            }
            entity.drinking = true;
            entity.thirst -= Time.deltaTime * 3;
            if (entity.thirst <= 0)
            {
                entity.thirst = 0;
                entity.drinking = false;
                entity.RemoveInstruction(2);
            }
        }
    }

    public override void OnStateExit()
    {       
        if (entity.thirst < 60)
        {
            entity.RemoveInstruction(2);
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        fovTrigger = entity.fovTrig;
        fovTrigger.FindTarget("Water", true);
        pathEnd = false;
        entity.UpdateStateText("Searching for Water");
    }

}
