using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public TMP_Dropdown StartStation;
    public TMP_Dropdown EndStation;
    public GameObject Startparent;


    public List<Sprite> Buslines;
    public GameObject RoutePrefab;
    public GameObject RouteParts;

    [SerializeField]
    List<GameObject> RoutePartsList;

    pysakitlist Stops;
    BestRoute bestRoute;

    // Start is called before the first frame update
    void Start()
    {
        RoutePartsList = new List<GameObject>();
        TextAsset asset = Resources.Load("reittiopas") as TextAsset;

        Stops = JsonUtility.FromJson<pysakitlist>(asset.text);
        List<TMP_Dropdown.OptionData> dropDownOption = new List<TMP_Dropdown.OptionData>();

        foreach(var item in Stops.pysakit)
        {
            dropDownOption.Add(new TMP_Dropdown.OptionData(item));
        }

        StartStation.AddOptions(dropDownOption);
        EndStation.AddOptions(dropDownOption);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateBestRoute()
    {


        foreach (Transform child in RouteParts.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (StartStation.value == EndStation.value)
            return;

        bestRoute = FindObjectOfType<RouteScript>().GetBestRoute(Stops.pysakit[StartStation.value], Stops.pysakit[EndStation.value]);
        for (int i = 0; i < bestRoute.Route.Count-1; i++)
        {
            GameObject temp= Instantiate(RoutePrefab, RouteParts.transform);
            Debug.Log(i);
            temp.GetComponentInChildren<Image>().overrideSprite = Buslines[bestRoute.RouteColor[i]];
            if(i<bestRoute.Route.Count-1)
            {
                temp.GetComponentInChildren<TMP_Text>().text = bestRoute.Route[i] + " -> " + bestRoute.Route[i + 1];
            }
            else
            {
                temp.GetComponentInChildren<TMP_Text>().text = bestRoute.Route[i];
            }

            RoutePartsList.Add(temp);
        }
       
     
    }
}
