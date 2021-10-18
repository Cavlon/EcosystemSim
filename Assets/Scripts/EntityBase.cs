using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class EntityBase : MonoBehaviour
{

    /* null = Wander
     * 0 = Passive Hunt / Forage
     * 1 = Reproduce
     * 2 = Drink
     * 3 = Active Hunt / Forage
     * 4 = Flee
     */

    /* Stats:
     *  0 = Speed
     *  1 = FOV Distance
     *  2 = Metabolism
     *  3 = Food Efficiency
     *  4 = Fertility
     *  5 = Hunting Prowess
     */

    [HideInInspector] public Seeker seeker;
    [HideInInspector] public FOVTrigger fovTrig;
    [HideInInspector] public bool drinking;
    [HideInInspector] public float energyYield;
    [HideInInspector] public string stateString;
    [HideInInspector] public bool moving;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool foundMate;
    [HideInInspector] public bool mateContact;
    [HideInInspector] public float mateFertility;
    [HideInInspector] public bool adult;
    [HideInInspector] public bool pregnant;
    [HideInInspector] public bool gestate;
    [HideInInspector] public float[] mateStats;
    [HideInInspector] public float mateAbnormality;
    
    [HideInInspector] public float[] stats = new float[] {5f, 10f, 5f, 5f, 0.9f};

    public float speed;
    public float metabolism;
    public float efficiency;
    public float fertility;
    public float fovDistance;
    [SerializeField] float fovAngle;

    [HideInInspector] public float matureTime = 60f;
    [HideInInspector] public float pregnancyTime = 10f;

    public bool male = false;
    public float abnormality = 0.1f;
    public float energy = 250f;
    public float thirst = 0f;
    public float sexualUrge = 0f;

    protected Transform trans;
    protected State currentState;
    protected List<int> instructions = new List<int>();
    protected bool[] instInStack = new bool[5];
    protected Transform baseAnimal;

    ManageSimulation manager;
    Text stateText;
    FieldOfView fov;
    float energyLossRate = 1f;
    float thirstLossRate = 1f;
    bool reproduce;
    int urgeThreshold;
    

    public virtual void Awake()
    {
        trans = transform;
        seeker = GetComponent<Seeker>();
        fov = GetComponent<FieldOfView>();
        adult = false;      
            
        urgeThreshold = Random.Range(50, 80);           

        int randVal = Random.Range(0, 2);
        if (randVal == 0)
        {
            Male();
        }      
        FindState();
    }

    public virtual void Update()
    {
        if (mateContact && reproduce)
        {
            reproduce = false;
            foundMate = false;
            canMove = false;
            sexualUrge = 0f;
            if (stateString != "Reproduce")
            {
                StartCoroutine(SetState(new Reproduce(this), 0.1f));
                stateString = "Reproduce";
            }                
        }

        if (pregnant && gestate)
        {
            StartCoroutine(PregnancyTime(pregnancyTime));
            gestate = false;
        }

        if (moving)
        {
            energyLossRate = 2 + (thirst / 200);
            thirstLossRate = 1.5f;
        } else
        {
            energyLossRate = 1 + (thirst / 200);
            thirstLossRate = 1;
        }

        if (manager == null)
        {
            manager = GameData.manager;
            if (manager != null)
            {
                manager.NewStateLabel(trans, out stateText);
            }          
        }

        if (currentState != null)
        {
            currentState.Tick();
        }

        if (!drinking && thirst < 100)
        {
            thirst += Time.deltaTime * thirstLossRate;
        }

        energy -= Time.deltaTime * energyLossRate;

        if (adult && !pregnant)
        {
            sexualUrge += Time.deltaTime;
        }
        
        if (energy <= 0)
        {
            Destroy(gameObject);
        }

        if (energyYield >= 0)
        {
            float eatingRate = Time.deltaTime * metabolism;
            energy += eatingRate;
            energyYield -= eatingRate;
        }  else
        {
            if (energy < 100 && !instInStack[3] && (energy + energyYield) < 250)
            {
                AddInstruction(3);
            } else if (energy < 200 && !instInStack[0] && (energy + energyYield) < 250)
            {
                AddInstruction(0);
            }
        }     

        if (thirst > 60 && !instInStack[2])
        {
            AddInstruction(2);
        }  

        if (sexualUrge > urgeThreshold && !instInStack[1])
        {            
            AddInstruction(1);
        }

        if (!adult)
        {
            matureTime -= Time.deltaTime;
            if (matureTime < 0)
            {
                adult = true;
                trans.localScale = Vector3.one;
                trans.name = baseAnimal.name;
            }
        }
    }

    public IEnumerator SetState(State state, float transTime)
    {
        yield return new WaitForSeconds(transTime);
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = state;
        if (currentState != null)
        {
            currentState.OnStateEnter();
        }
    }

    public virtual void FindState()
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
            }
        }
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        if (foundMate && !mateContact && collision.transform.CompareTag(trans.tag) && collision.transform != trans)
        {
            EntityBase mate = collision.transform.GetComponent<EntityBase>();
            if (mate.adult && mate.male != male && !pregnant && !mate.pregnant)
            {
                mateContact = true;
                reproduce = true;
                if (male)
                {
                    mate.mateFertility = fertility;
                    mate.mateStats = stats;
                    mate.mateAbnormality = abnormality;
                }
                mate.mateContact = true;
                mate.reproduce = true;
            }          
        }
    }

    public IEnumerator PregnancyTime(float gestationTime)
    {
        yield return new WaitForSeconds(gestationTime);
        pregnant = false;
        float[] childStats = new float[stats.Length];
        for (int i = 0; i < childStats.Length; i++)
        {
            childStats[i] = (mateStats[i] + stats[i]) / 2;
        }
        Transform child = Instantiate(baseAnimal, trans.position, Quaternion.identity, trans.parent);
        EntityBase childEntity = child.GetComponent<EntityBase>();
        childEntity.abnormality = (mateAbnormality + abnormality) / 2;
        childEntity.Mutations(childStats);
        childEntity.matureTime = GameData.matureTime;
        childEntity.pregnancyTime = gestationTime;
        var angle = child.rotation.eulerAngles;
        angle.y = Random.Range(-180, 181);
        child.rotation = Quaternion.Euler(angle);
        child.localScale = Vector3.one * 0.5f;
        child.name = trans.name + " Child";
    }

    public abstract void Eat(int instVal);

    public virtual void StatAssign()
    {
        speed = stats[0];
        metabolism = stats[2];
        efficiency = stats[3];
        fertility = stats[4];
        fovDistance = stats[1];
        fovAngle = 8 * Mathf.Atan(22 / (fovDistance * 20))* Mathf.Rad2Deg;
        canMove = true;
    }

    public void Mutations(float[] newStats)
    {
        stats = (float[])newStats.Clone();
        abnormality += Random.Range(-0.05f, 0.05f);
        for (int i = 0; i < stats.Length; i++)
        {

            if (i != 4 && i != 5 && i != 6)
            {
                stats[i] += Random.Range(-2f, 2f);
                stats[i] -= abnormality;
            }
            else
            {
                stats[i] += Random.Range(-0.2f, 0.2f);
                stats[i] -= abnormality / 5;
            }

            if (stats[i] < 0)
            {
                stats[i] = 0;
            }
        }
        StatAssign();
        fov.CreateFOV(fovDistance, fovAngle, this);
    }

    protected void AddInstruction(int instVal)
    {
        instructions.Add(instVal);
        instructions.Sort();
        instInStack[instVal] = true;
        FindState();
    }

    public void RemoveInstruction(int instVal)
    {       
        if (instInStack[instVal])
        {
            instructions.Remove(instVal);
            instructions.Sort();
            instInStack[instVal] = false;
        }        
        FindState();
    }

    public void UpdateStateText(string newText)
    {
        stateText.text = newText;
    }

    public virtual void Male()
    {
        male = true;
    }
}
