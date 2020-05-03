using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteScript : MonoBehaviour
{
    public tietlist RoadsList = new tietlist();
    public pysakitlist pysakitlist = new pysakitlist();
    public linjastotlist linjastot = new linjastotlist();
    public List<List<string>> PossibleRoutes = new List<List<string>>();
    public int CurrentLinjasto;

    string Printstr;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLinjasto = 0;
        TextAsset asset = Resources.Load("reittiopas") as TextAsset;

        if (asset != null)
        {
            RoadsList = JsonUtility.FromJson<tietlist>(asset.text);
            pysakitlist = JsonUtility.FromJson<pysakitlist>(asset.text);
            linjastot = JsonUtility.FromJson<linjastotlist>(asset.text);
        }

        //foreach (var item in linjastot.keltainen)
        //{
        //    print(item);
        //}

        List<string> currentRoute = new List<string>();
        GetRoute("E", "F", currentRoute, 0);
        BestRoute usethisroute = new BestRoute();
        usethisroute.Lenght = 1000;

        //print all routes
        foreach (var item in PossibleRoutes)
        {
            print("complete route:");
            Printstr = "";
            foreach (var route in item)
            {
                Printstr += " -> " + route;
            }

            int routelenght = 0;
            for (int i = 1; i < item.Count; i++)
            {
                routelenght += GetLenght(item[i - 1], item[i]);
            }
            print(Printstr);
            print("Lenght: " + routelenght);

            if (routelenght < usethisroute.Lenght)
            {
                usethisroute.Route = item;
                usethisroute.Lenght = routelenght;
            }
        }

        print("Best route:");
        Printstr = "";
        foreach (var route in usethisroute.Route)
        {
            Printstr += " -> " + route;
        }
        print(Printstr);
        print("Lenght: " + usethisroute.Lenght);

        for (int i = 1; i < usethisroute.Route.Count; i++)
        {
            int line;
            ////check first the one line being used
            line = checkCurrentLinjasto(usethisroute.Route[i - 1], usethisroute.Route[i], CurrentLinjasto);

            usethisroute.RouteColor.Add(line);
            CurrentLinjasto = line;
            print("Color: "+usethisroute.RouteColor[i - 1]);

        }
    }

    public BestRoute GetBestRoute(string from, string to)
    {
        PossibleRoutes.Clear();
        List<string> currentRoute = new List<string>();
        GetRoute(from, to, currentRoute, 0);

        BestRoute usethisroute = new BestRoute();
        usethisroute.Lenght = 1000;

        foreach (var item in PossibleRoutes)
        {
            
            int routelenght = 0;
            for (int i = 1; i < item.Count; i++)
            {
                routelenght += GetLenght(item[i - 1], item[i]);
            }

            if (routelenght < usethisroute.Lenght)
            {
                usethisroute.Route = item;
                usethisroute.Lenght = routelenght;
            }
        }

        for (int i = 1; i < usethisroute.Route.Count; i++)
        {
            int line;
            ////check first the one line being used
            line = checkCurrentLinjasto(usethisroute.Route[i - 1], usethisroute.Route[i], CurrentLinjasto);

            usethisroute.RouteColor.Add(line);
            CurrentLinjasto = line;
        }


        return usethisroute;
    }

    public List<string> getConnections(string Stop)
    {
        List<string> temp = new List<string>();

        foreach (var road in RoadsList.tiet)
        {
            if (road.mista == Stop)
            {
                if (road.mihin != Stop)
                    temp.Add(road.mihin);
                //   print(Stop+"-> "+road.mihin);
            }
            if (road.mihin == Stop)
            {
                if (road.mista != Stop)
                    temp.Add(road.mista);
                //  print(Stop + "<- " + road.mista);
            }

        }
        for (int i = temp.Count-1; i >0 ; i--)
        {
            if (getLinjasto(Stop, temp[i]) == 0)
                temp.RemoveAt(i);
            
        }
        return temp;
    }

    public int GetLenght(string A, string B)
    {
        int temp = 0;

        foreach (var item in RoadsList.tiet)
        {
            if (item.mihin == A && item.mista == B)
            {
                //print(A + "- " + B + ":" + item.kesto);
                temp = item.kesto;
            }
            if (item.mista == A && item.mihin == B)
            {
                // print(A + "- " + B + ":" + item.kesto);
                temp = item.kesto;
            }
        }

        return temp;
    }

    public void GetRoute(string Start, string Stop, List<string> Route, int counter)
    {
        List<string> currentRoute = new List<string>(Route);
        currentRoute.Add(Start);

        List<string> possibilites = getConnections(Start);

        foreach (string s in possibilites)
        {
            if (s == Stop)
            {
                currentRoute.Add(s);
                PossibleRoutes.Add(currentRoute);
                return;
            }
            else
            {
                //if deadEnd
                if (Route.Contains(s))
                {
                    //  print("DeadEnd");
                }
                else
                {
                    counter++;
                    //preventing it from making too many stops
                    if (counter < 20)
                        GetRoute(s, Stop, currentRoute, counter);
                    else
                    { }
                    //  print("Over 10");

                }
            }
        }
    }

    public int getLinjasto(string A, string B)
    {
        int temp = 0;
        temp = CheckYellow(A, B);
        if (temp != 0)
            return temp;
        temp = CheckRed(A, B);
        if (temp != 0)
            return temp;
        temp = CheckGreen(A, B);
        if (temp != 0)
            return temp;
        temp = CheckBlue(A, B);
        return temp;
    }

    public int checkCurrentLinjasto(string A, string B, int current)
    {
        int temp = 0;
        switch (current)
        {
            case 1:
                temp = CheckYellow(A, B);
                break;
            case 2:
                temp = CheckRed(A, B);
                break;
            case 3:
                temp = CheckGreen(A, B);
                break;
            case 4:
                temp = CheckBlue(A, B);
                break;
            default:
                temp = getLinjasto(A, B);
                break;
        }
        if (temp == 0)
           temp= getLinjasto(A, B);
    return temp;
        
    }

    public int CheckYellow(string A, string B)
    {
        for (int i = 1; i < linjastot.linjastot.keltainen.Count; i++)
        {
            if (linjastot.linjastot.keltainen[i - 1] == A && linjastot.linjastot.keltainen[i] == B)
                return 1;
        }
        for (int i = linjastot.linjastot.keltainen.Count-1; i > 0; i--)
        {
            if (linjastot.linjastot.keltainen[i] == A && linjastot.linjastot.keltainen[i - 1] == B)
            {
                return 1;
            }
        }
        return 0;
    }
    public int CheckRed(string A, string B)
    {
        for (int i = 1; i < linjastot.linjastot.punainen.Count; i++)
        {
            if (linjastot.linjastot.punainen[i - 1] == A && linjastot.linjastot.punainen[i] == B)
                return 2;
        }
        for (int i = linjastot.linjastot.punainen.Count - 1; i > 1; i--)
        {
            if (linjastot.linjastot.punainen[i] == A && linjastot.linjastot.punainen[i - 1] == B)
                return 2;
        }
        return 0;

    }
    public int CheckGreen(string A, string B)
    {

        for (int i = 1; i < linjastot.linjastot.vihreä.Count; i++)
        {
            if (linjastot.linjastot.vihreä[i - 1] == A && linjastot.linjastot.vihreä[i] == B)
                return 3;
        }
        for (int i = linjastot.linjastot.vihreä.Count - 1; i > 1; i--)
        {
            if (linjastot.linjastot.vihreä[i] == A && linjastot.linjastot.vihreä[i - 1] == B)
                return 3;
        }
        return 0;
    }
    public int CheckBlue(string A, string B)
    {
        for (int i = 1; i < linjastot.linjastot.sininen.Count; i++)
        {
            if (linjastot.linjastot.sininen[i - 1] == A && linjastot.linjastot.sininen[i] == B)
            {
                return 4;
            }
        }
        for (int i = linjastot.linjastot.sininen.Count - 1; i > 1; i--)
        {
            if (linjastot.linjastot.sininen[i] == A && linjastot.linjastot.sininen[i - 1] == B)
                return 4;
        }
        return 0;
    }
}



public class BestRoute
{
    public List<string> Route = new List<string>();
    public List<int> RouteColor = new List<int>();
    public int Lenght = 1000;
}

#region givenData
[System.Serializable]
public class tiet
{
   public string mista;
   public string mihin;
   public int kesto;
}

[System.Serializable]
public class tietlist
{
   public List<tiet> tiet = new List<tiet>();
}

[System.Serializable]
public class pysakitlist
{
    public List<string> pysakit = new List<string>();
}

[System.Serializable]
public class linjastotlist
{
    public linjasto linjastot;
}

[System.Serializable]
public class linjasto
{
    public List<string> keltainen = new List<string>();
    public List<string> punainen = new List<string>();
    public List<string> vihreä = new List<string>();
    public List<string> sininen = new List<string>();
}


#endregion
