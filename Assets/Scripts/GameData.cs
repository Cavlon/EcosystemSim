using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{

    public static Rect canvasBounds;
    public static ManageSimulation manager;
    public static int graphValue;

    public static int deerPop;
    public static int bearPop;
    public static float?[] deerAvgStats = new float?[5];
    public static float?[] bearAvgStats = new float?[6];
    public static float deerAvgAbnormality;
    public static float bearAvgAbnormality;
    public static float matureTime;

    public static float AverageValue(float? average, int population, float value, bool removeVal = false)
    {
        if (!average.HasValue)
        {
            average = value;
        } else if (!removeVal)
        {
            if (population != 0)
            {
                float tempVal = average.Value * population;
                tempVal += value;
                tempVal /= population + 1;
                average = tempVal;
            } else
            {
                average = 0;
            }         
        } else
        {
            if (population != 0)
            {
                float tempVal = average.Value * population;
                tempVal -= value;
                tempVal /= population - 1;
                average = tempVal;
            } else
            {
                average = 0;
            }           
        }
        if (average == Mathf.Infinity || average < 0)
        {
            average = 0;
        }
        return average.Value;
    }
}
