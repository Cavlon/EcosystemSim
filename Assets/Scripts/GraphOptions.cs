using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphOptions : MonoBehaviour
{

    [SerializeField] Transform graph;
    [SerializeField] Text onText;
    [SerializeField] Text graphTitle;
    bool graphActive;
    ManageSimulation manager;

    private void Start()
    {
        manager = transform.parent.GetComponent<ManageSimulation>();
    }

    public void ActivateGraph()
    {
        graphActive = !graphActive;
        graph.gameObject.SetActive(graphActive);
        if (graphActive)
        {
            GameData.graphValue = 1;
            onText.text = "OFF GRAPH";
            graphTitle.text = "POPULATION";
        } else
        {
            GameData.graphValue = 0;
            onText.text = "ON GRAPH";
            graphTitle.text = "";
        }
        
    }

    public void SwitchGraph(bool increase)
    {
        if (increase)
        {
            GameData.graphValue += 1;
            if (GameData.graphValue == 9) 
            {
                GameData.graphValue = 1;
            }
        } else
        {
            GameData.graphValue -= 1;
            if (GameData.graphValue == 0)
            {
                GameData.graphValue = 8;
            }
        }
        switch (GameData.graphValue) 
        {
            case 1:
                graphTitle.text = "POPULATION";
                break;
            case 2:
                graphTitle.text = "SPEED";
                break;
            case 3:
                graphTitle.text = "FOV DISTANCE";
                break;
            case 4:
                graphTitle.text = "METABOLISM";
                break;
            case 5:
                graphTitle.text = "FOOD EFFICIENCY";
                break;
            case 6:
                graphTitle.text = "FERTILITY";
                break;
            case 7:
                graphTitle.text = "HUNTING PROWESS";
                break;
            case 8:
                graphTitle.text = "ABNORMALITY";
                break;
        }
        manager.UpdateGraph();
    }
}
