using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Reproduce : Wander
{
    FOVTrigger fovTrigger;
    Transform targetTrans;
    bool initialFind = true;
    bool newPath;
    float mateTime;
    bool startMateTimer;

    public Reproduce(EntityBase entity) : base(entity)
    {
        base.entity = entity;
    }

    public override void Tick()
    {

        if (entity.mateContact)
        {
            if (!entity.male && !entity.pregnant)
            {
                int randVal = Random.Range(0, 101);
                int pregnancyChance = (int)(entity.mateFertility * entity.fertility * 100);
                if (randVal < pregnancyChance)
                {
                    entity.pregnant = true;
                    entity.gestate = true;
                }
            }
            entity.UpdateStateText("Mating");            
            entity.mateContact = false;
            startMateTimer = true;
        } else
        {
            if (entity.foundMate)
            {
                if (targetTrans == null)
                {                  
                    fovTrigger.EndContact();
                    resourceFound = false;
                    targetTrans = null;
                    initialFind = true;
                    newPath = false;
                    entity.UpdateStateText("Searching for Mate");
                    fovTrigger.FindTarget(entity.tag, false, true, entity.male);
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
            if (fovTrigger.found && targetTrans != fovTrigger.targetTrans)
            {
                resourceFound = true;
                targetTrans = fovTrigger.targetTrans;
                newPath = true;
                entity.foundMate = true;

                if (initialFind)
                {
                    entity.UpdateStateText("Found Mate");
                    initialFind = false;
                }
            }
        }

        if (startMateTimer)
        {            
            mateTime += Time.deltaTime;
        }

        if (mateTime > 5f)
        {
            mateTime = 0f;
            entity.RemoveInstruction(1);
        }

        move = entity.canMove;

        if (move)
        {
            WanderTick();
        }     
    }

    public override void OnStateExit()
    {
        fovTrigger.EndContact();
        entity.foundMate = false;
        entity.mateContact = false;
        entity.canMove = true;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        fovTrigger = entity.fovTrig;
        if (!entity.mateContact)
        {
            fovTrigger.FindTarget(entity.tag, false, true, entity.male);
            entity.UpdateStateText("Searching for Mate");
        }       
    }

    public async void RegisterNewPath()
    {
        await Task.Delay(1000);
        newPath = true;
    }
}
