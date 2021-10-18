using UnityEngine;
using Pathfinding;

public abstract class Pathfind : State
{

    protected EntityBase entity;
    public Vector3 target;
    protected Transform trans;
    protected Seeker seeker;
    protected bool pathEnd;    

    float speed;
    float nextWaypointDistance = 1f;
    readonly float initialPathUpdateDelay = 0.5f;
    float pathUpdateDelay;
    Path path;
    int currentWaypoint;
    float distance;

    public Pathfind(EntityBase entity)
    {
        this.entity = entity;
    }
    
    public override void Tick()
    {        
        if (path == null)
            return;

        if (currentWaypoint < path.vectorPath.Count)
        {
            Vector3 targetDir = path.vectorPath[currentWaypoint] - trans.position;
            targetDir.y = 0;
            trans.position += targetDir.normalized * speed * Time.deltaTime;          
            Vector3 newDirection = Vector3.RotateTowards(trans.forward, targetDir, speed * Time.deltaTime, 0f);
            trans.rotation = Quaternion.LookRotation(newDirection);
            distance = Vector3.Distance(trans.position, path.vectorPath[currentWaypoint]);
            entity.moving = true;
        } else
        {
            pathEnd = true;
            entity.moving = false;
        }

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        if (pathUpdateDelay <= 0)
        {
            pathUpdateDelay = initialPathUpdateDelay;
        }

        pathUpdateDelay -= Time.deltaTime;
    }

    public override void OnStateEnter()
    {
        seeker = entity.seeker;
        trans = entity.transform;
        speed = entity.speed;
        NewPath();
        pathUpdateDelay = initialPathUpdateDelay;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;          
        }
    }

    protected void NewPath()
    {
        if (AstarPath.active.graphs.Length != 0 && trans != null)
        {
            seeker.StartPath(trans.position, target, OnPathComplete);
            pathEnd = false;
        }      
    }
}
