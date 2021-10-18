using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CodeMonkey.Utils;

public class Graph : MonoBehaviour
{

    [SerializeField] Sprite pointSprite;
    RectTransform pointContainer;
    List<GameObject> pointList = new List<GameObject>();
    [SerializeField] RectTransform label;

    private void Awake()
    {
        pointContainer = GetComponentInChildren<RectTransform>();
    }

    public void ShowGraph(List<List<int>> values, Color[] lineColour)
    {

        foreach (GameObject point in pointList)
        {
            Destroy(point);
        }
        pointList.Clear();
        
        float graphHeight = pointContainer.sizeDelta.y;
        float yMaximum = 0;
        List<int> lastVals = new List<int>();
        for (int j = 0; j < values.Count; j++)
        {
            lastVals.Clear();
            for (int i = Mathf.Max(values[j].Count - 20, 0); i < values[j].Count; i++)
            {
                lastVals.Add(values[j][i]);
            }
            if (lastVals.Max() > yMaximum)
            {
                yMaximum = lastVals.Max();
            }
        }
        float xSize = (pointContainer.sizeDelta.x * .9f) / (lastVals.Count - 1);

        for (int j = 0; j < values.Count; j++)
        {
            Vector2 lastPoint = Vector2.zero;
            bool firstPoint = true;
            int xIndex = 0;
            for (int i = Mathf.Max(values[j].Count - 20, 0); i < values[j].Count; i++)
            {
                float xPos = (pointContainer.sizeDelta.x * .05f) + xIndex * xSize;
                float yPos = values[j][i] / yMaximum * (graphHeight * .9f) + (graphHeight * .05f);
                Vector2 point = new Vector2(xPos, yPos);
                if (!firstPoint)
                {
                    GameObject line = CreateConnection(lastPoint, point, lineColour[j]);
                    pointList.Add(line);
                }
                firstPoint = false;
                lastPoint = point;

                xIndex++;
            }
        }

        for (int i = 0; i <= 10; i++)
        {
            RectTransform labelY = Instantiate(label);
            labelY.SetParent(pointContainer, false);
            labelY.gameObject.SetActive(true);
            float normalisedValue = i / 10f;
            labelY.anchoredPosition = new Vector2(11f, normalisedValue * (graphHeight * .9f) + (graphHeight * .05f));
            labelY.GetComponent<Text>().text = (Math.Round(normalisedValue * yMaximum * 2, MidpointRounding.AwayFromZero) / 2).ToString();
            pointList.Add(labelY.gameObject);
        }
    }
    public void ShowGraph(List<List<float>> values, Color[] lineColour, bool preciseLabels = false)
    {

        foreach (GameObject point in pointList)
        {
            Destroy(point);
        }
        pointList.Clear();

        float graphHeight = pointContainer.sizeDelta.y;
        float yMaximum = 0;
        List<float> lastVals = new List<float>();
        for (int j = 0; j < values.Count; j++)
        {
            lastVals.Clear();
            for (int i = Mathf.Max(values[j].Count - 20, 0); i < values[j].Count; i++)
            {
                lastVals.Add(values[j][i]);
            }
            if (lastVals.Max() > yMaximum)
            {              
                yMaximum = lastVals.Max();                
            }
        }
        float xSize = (pointContainer.sizeDelta.x * .9f) / (lastVals.Count - 1);

        for (int j = 0; j < values.Count; j++)
        {
            Vector2 lastPoint = Vector2.zero;
            bool firstPoint = true;
            int xIndex = 0;
            for (int i = Mathf.Max(values[j].Count - 20, 0); i < values[j].Count; i++)
            {
                float xPos = (pointContainer.sizeDelta.x * .05f) + xIndex * xSize;
                float yPos = values[j][i] / yMaximum * (graphHeight * .9f) + (graphHeight * .05f);
                Vector2 point = new Vector2(xPos, yPos);
                if (!firstPoint)
                {
                    GameObject line = CreateConnection(lastPoint, point, lineColour[j]);
                    pointList.Add(line);
                }
                firstPoint = false;
                lastPoint = point;

                xIndex++;
            }
        }

        for (int i = 0; i <= 10; i++)
        {
            RectTransform labelY = Instantiate(label);
            labelY.SetParent(pointContainer, false);
            labelY.gameObject.SetActive(true);
            float normalisedValue = i / 10f;
            labelY.anchoredPosition = new Vector2(11f, normalisedValue * (graphHeight * .9f) + (graphHeight * .05f));
            if (!preciseLabels)
            {
                labelY.GetComponent<Text>().text = (Math.Round(normalisedValue * yMaximum * 2, MidpointRounding.AwayFromZero) / 2).ToString();
            } else
            {
                labelY.GetComponent<Text>().text = Math.Round(normalisedValue * yMaximum, 2).ToString();
            }            
            pointList.Add(labelY.gameObject);
        }      
    }

    GameObject CreateConnection(Vector2 pointA, Vector2 pointB, Color lineColour)
    {
        GameObject line = new GameObject("PointConnection", typeof(Image));
        line.transform.SetParent(pointContainer, false);
        line.GetComponent<Image>().color = lineColour;
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (pointB - pointA).normalized;
        float dist = Vector2.Distance(pointA, pointB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(dist, 2);
        rectTransform.anchoredPosition = pointA + dir * dist * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        return line;
    }
}
