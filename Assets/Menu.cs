using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    LoadData loadData;
    [SerializeField] Transform mainScreen;
    [SerializeField] Transform settingsScreen;

    [SerializeField] InputField[] inputFields = new InputField[19];

    void Start()
    {
        loadData = GameObject.Find("DataHolder").GetComponent<LoadData>();
    }

    public void SwitchScreen(bool main)
    {
        mainScreen.gameObject.SetActive(main);
        settingsScreen.gameObject.SetActive(!main);

        if (main)
        {
            loadData.seed = inputFields[0].text;
            loadData.mapSize = new Vector2(int.Parse(inputFields[1].text), int.Parse(inputFields[2].text));
            loadData.noiseScale = int.Parse(inputFields[3].text);
            loadData.initialFood = int.Parse(inputFields[4].text);
            loadData.foodDelay = float.Parse(inputFields[5].text, CultureInfo.InvariantCulture.NumberFormat);
            loadData.animalNum[0] = int.Parse(inputFields[6].text);
            loadData.animalNum[1] = int.Parse(inputFields[7].text);
            loadData.matureTime = float.Parse(inputFields[19].text);
            loadData.gestationTime = float.Parse(inputFields[20].text);
            for (int i = 0; i < 5; i++)
            {
                loadData.deerStats[i] = float.Parse(inputFields[i + 8].text, CultureInfo.InvariantCulture.NumberFormat);
                loadData.bearStats[i] = float.Parse(inputFields[i + 13].text, CultureInfo.InvariantCulture.NumberFormat);
            }
            loadData.bearStats[5] = float.Parse(inputFields[18].text, CultureInfo.InvariantCulture.NumberFormat);
        }
    }

    public void StartSim()
    {
        SceneManager.LoadScene(1);
    }
}
