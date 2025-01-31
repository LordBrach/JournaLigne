using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDataManager
{
    private static GraphDataManager instance;

    // Data
    PartyData civilData = new PartyData();
    PartyData dictData = new PartyData();
    PartyData rebelData = new PartyData();

    public PartyData CivilData { get => civilData; set => civilData = value; }
    public PartyData DictData { get => dictData; set => dictData = value; }
    public PartyData RebelData { get => rebelData; set => rebelData = value; }

    private GraphDataManager ()
    {
    }

    public static GraphDataManager Instance
    {
        get { if (instance == null) { instance = new GraphDataManager(); } return instance; }
    }

}
