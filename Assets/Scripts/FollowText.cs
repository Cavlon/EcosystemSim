using UnityEngine;
using UnityEngine.UI;

public class FollowText : MonoBehaviour
{

    [Header("Tweaks")]
    public Transform target;
    [SerializeField] Vector3 offset;

    Camera cam;
    Text text;
    Vector3 pos;

    void Awake()
    {
        cam = Camera.main;
        text = GetComponent<Text>();
    }


    void Update()
    {

        if (target == null)
        {
            Destroy(gameObject);
        } else
        {
            pos = cam.WorldToScreenPoint(target.position + offset);
        }      

        if (transform.position != pos)
        {
            transform.position = pos;
        }

        if (GameData.canvasBounds.Contains(pos) && pos.z > 0)
        {
            text.enabled = true;          
        } else
        {
            text.enabled = false;
        }
    }
}
