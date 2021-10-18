using System.Threading.Tasks;
using UnityEngine;

public class Wander : Pathfind
{

    protected bool move;
    public bool resourceFound;
    bool newRandTarget;  

    public Wander(EntityBase entity) : base(entity)
    {
        base.entity = entity;
    }

    public override void Tick()
    {        
        if (move)
        {
            WanderTick();          
        }
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        move = false;
        entity.UpdateStateText("Wandering");
        NewRandTarget(trans.position, false);
    }

    public async void NewRandTarget(Vector3 pos, bool initialWait = true)
    {
        if (initialWait)
        {
            await Task.Delay(Random.Range(2000, 4000));
        }       
        Vector3 intPos = new Vector3((int)pos.x, 0, (int)pos.z);

        int randX = Random.Range(3, 10);
        int randY = Random.Range(3, 10);
        if (Random.Range(0, 2) == 0)
        {
            randX = -randX;
        }
        if (Random.Range(0, 2) == 0)
        {
            randY = -randY;
        }
        
        Vector3 newPos = new Vector3(randX, 0, randY);
        if (resourceFound)
        {
            move = true;
            return;
        }
        target = intPos + newPos;
        newRandTarget = true;
        NewPath();
        move = false;
        await Task.Delay(Random.Range(150, 1000));
        move = true;
    }

    protected void WanderTick()
    {
        base.Tick();
        if (newRandTarget)
        {
            NewRandTarget(trans.position);
            newRandTarget = false;
        }
    }
}
