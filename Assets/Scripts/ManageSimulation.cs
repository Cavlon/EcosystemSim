using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class ManageSimulation : MonoBehaviour
{
    /* Current Graph Val:
     * 1 = Population
     * 2 = Speed
     * 3 = FOV Distance
     * 4 = Metabolism
     * 5 = Efficiency
     * 6 = Fertility
     * 7 = Hunting Prowess
     */

    [SerializeField] Canvas floatingUI;
    [SerializeField] Text stateText;
    public Transform baseDeer;
    public Transform baseBear;

    List<int> deerPopList = new List<int>();
    List<int> bearPopList = new List<int>();

    List<float>[] deerStatsList = new List<float>[5];
    List<float>[] bearStatsList = new List<float>[6];

    List<float> deerAbnormalityList = new List<float>();
    List<float> bearAbnormalityList = new List<float>();

    [SerializeField] Graph graph;

    void Awake()
    {
        GameData.manager = this;
        Rect bounds = floatingUI.GetComponent<RectTransform>().rect;
        bounds.x = 0;
        bounds.y = 0;
        GameData.canvasBounds = bounds;

        for (int i = 0; i < 5; i++)
        {
            deerStatsList[i] = new List<float>();
            bearStatsList[i] = new List<float>();
        }
        bearStatsList[5] = new List<float>();

        FunctionPeriodic.Create(() =>
        {
            deerPopList.Add(GameData.deerPop);
            bearPopList.Add(GameData.bearPop);

            deerAbnormalityList.Add(GameData.deerAvgAbnormality);
            bearAbnormalityList.Add(GameData.bearAvgAbnormality);

            for (int i = 0; i < 5; i++)
            {
                deerStatsList[i].Add(GameData.deerAvgStats[i].Value);
                bearStatsList[i].Add(GameData.bearAvgStats[i].Value);
            }
            bearStatsList[5].Add(GameData.bearAvgStats[5].Value);

            if (deerPopList.Count > 20)
            {
                deerPopList.RemoveAt(0);
                bearPopList.RemoveAt(0);

                deerAbnormalityList.RemoveAt(0);
                bearAbnormalityList.RemoveAt(0);

                for (int i = 0; i < 5; i++)
                {
                    deerStatsList[i].RemoveAt(0);
                    bearStatsList[i].RemoveAt(0);
                }
                bearStatsList[5].RemoveAt(0);
            }
            UpdateGraph();         
        }, 3f);
    }

    public void NewStateLabel(Transform target, out Text stateText)
    {
        stateText = Instantiate(this.stateText, floatingUI.transform);
        stateText.GetComponent<FollowText>().target = target;
        stateText.raycastTarget = false;
        stateText.maskable = false;
    }

    public void UpdateGraph()
    {
        switch (GameData.graphValue)
        {
            case 0:
                break;
            case 1:
                graph.ShowGraph(new List<List<int>> { deerPopList, bearPopList }, new Color[] { Color.green, Color.blue });
                break;
            case 2:
                graph.ShowGraph(new List<List<float>> { deerStatsList[0], bearStatsList[0] }, new Color[] { Color.green, Color.blue });
                break;
            case 3:
                graph.ShowGraph(new List<List<float>> { deerStatsList[1], bearStatsList[1] }, new Color[] { Color.green, Color.blue });
                break;
            case 4:
                graph.ShowGraph(new List<List<float>> { deerStatsList[2], bearStatsList[2] }, new Color[] { Color.green, Color.blue });
                break;
            case 5:
                graph.ShowGraph(new List<List<float>> { deerStatsList[3], bearStatsList[3] }, new Color[] { Color.green, Color.blue });
                break;
            case 6:
                graph.ShowGraph(new List<List<float>> { deerStatsList[4], bearStatsList[4] }, new Color[] { Color.green, Color.blue }, true);
                break;
            case 7:
                graph.ShowGraph(new List<List<float>> { bearStatsList[5] }, new Color[] { Color.blue }, true);
                break;
            case 8:
                graph.ShowGraph(new List<List<float>> { deerAbnormalityList, bearAbnormalityList }, new Color[] { Color.green, Color.blue }, true);
                break;
        }
    }
}
