using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FOVTrigger : MonoBehaviour
{

    public string targetTag;
    public Vector3 targetPos;
    public bool found;
    public float initialRegisterTime;
    public Transform targetTrans;
    bool checkStayCollisions;
    bool closestPoint;
    bool checkGender;
    bool isMale;
    [HideInInspector] public bool isDeer;
    [HideInInspector] public Deer deer;

    void OnCollisionStay(Collision collision)
    {
        if (isDeer)
        {
            if (collision.transform.CompareTag("Bear"))
            {
                deer.flee = true;
            } else
            {
                deer.flee = false;
            } 
        }

        if (targetTag != null)
        {
            if (collision.transform.CompareTag(targetTag) && checkStayCollisions && collision.transform != transform.parent)
            {
                if (closestPoint)
                {
                    checkStayCollisions = false;
                    List<ContactPointVals> contactPointVals = new List<ContactPointVals>();
                    foreach (ContactPoint contact in collision.contacts)
                    {
                        if (contact.otherCollider.CompareTag(targetTag) && contact.point != Vector3.zero)
                        {
                            Vector2 contactVector = new Vector2(contact.point.x, contact.point.z) - new Vector2(transform.parent.position.x, transform.parent.position.z);
                            contactPointVals.Add(new ContactPointVals(contactVector, contact.point.sqrMagnitude));
                        }
                    }
                    contactPointVals = contactPointVals.OrderBy(a => a.sqrMagnitude).ToList();
                    targetPos = new Vector3((int)contactPointVals[0].point.x, 0, (int)contactPointVals[0].point.y);
                    found = true;
                }
                else
                {
                    if (targetPos == Vector3.zero)
                    {
                        newTarget(collision.transform.position, collision.transform);
                    }
                    else if ((targetPos - transform.parent.position).sqrMagnitude > (collision.transform.position - transform.parent.position).sqrMagnitude)
                    {
                        newTarget(collision.transform.position, collision.transform);
                    }
                }
            }                   
        }    
    }

    public void FindTarget(string targetTag, bool closestPoint, bool checkGender = false, bool isMale = false)
    {
        this.targetTag = targetTag;
        checkStayCollisions = true;
        this.closestPoint = closestPoint;
        this.checkGender = checkGender;
        this.isMale = isMale;
    }

    public void EndContact()
    {
        checkStayCollisions = false;
        targetTrans = null;
        found = false;
        targetTag = null;
        targetPos = Vector3.zero;
        checkGender = false;
    }

    void newTarget(Vector3 newPos, Transform newTrans)
    {
        if (checkGender)
        {
            EntityBase mate = newTrans.GetComponent<EntityBase>();
            if (mate.male == !isMale && mate.adult == true && !mate.pregnant)
            {
                targetPos = newPos;
                targetTrans = newTrans;
                found = true;
            }
        } else
        {
            targetPos = newPos;
            targetTrans = newTrans;
            found = true;
        }
        
    }
}

public class ContactPointVals 
{
    public Vector2 point;
    public float sqrMagnitude;

    public ContactPointVals(Vector2 point, float sqrMagnitude)
    {
        this.point = point;
        this.sqrMagnitude = sqrMagnitude;
    }
}

