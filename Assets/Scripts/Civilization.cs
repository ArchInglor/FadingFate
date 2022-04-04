using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Civilization : MonoBehaviour
{
    #region parameters
    private static float civCycleOffset = 5f;
    private static float civGrowthOffset = 0.2f;
    private static int startPointsOffset = 500;
    private static CivPoints startPoints = new CivPoints(1000, 1000, 1000);

    public static float CivCycleOffset { get => civCycleOffset; }
    #endregion

    #region civilization
    private CivPoints civilizationPoints = new CivPoints(0, 0, 0);
    private Population population = new Population(0, 0, 0);
    private float tension = 0f;
    private float naturalGrowth = 1.2f;
    private CivPoints civDebuff = new CivPoints(0, 0, 0);
    private List<Building> buildings = new List<Building>();
    #endregion

    #region game cycle
    public event UnityAction CycleUpdated;
    public event UnityAction<Population> PopulationChanged;
    [SerializeField] private float cycleDuration = 1000f;
    private float cycleTime = 0f;
    private int cycleCounter = 0;
    #endregion

    #region objects
    private TextMeshProUGUI text;
    #endregion

    private void Awake() 
    {
        text = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
    }
    private void Start() {
        CivCycle();
    }
    private void Update() 
    {
        cycleTime++;
        if (cycleTime >= cycleDuration)
        {
            UpdateCycle();
            cycleCounter++;
            cycleTime = 0f;            
        }
    }

    public void TextUpdate()
    {
        text.text = "<voffset=0.5em><size=200%><sprite=0></size></voffset> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + Mathf.RoundToInt(civilizationPoints.e) + "</color></b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=1></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">" + Mathf.RoundToInt(civilizationPoints.r) + "</color></b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=2></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">" + Mathf.RoundToInt(civilizationPoints.c) + "</color></b><br>";
        text.text += "<br><br><voffset=0.5em><size=200%><sprite=3></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEarth)) + ">" + Mathf.RoundToInt(tension*100) + "%</color></b><br>";
        
    }

    #region game cycle
    private void UpdateCycle() 
    {
        CivPoints cycleIncome = new CivPoints(0, 0, 0);
        foreach (Building building in buildings) 
        {
            cycleIncome += building.Income();
        }
        civilizationPoints += cycleIncome; 
        Growth(cycleIncome);
        TextUpdate();        
        Debug.Log("Cycle " + cycleIncome.e + ", " + cycleIncome.r + ", " + cycleIncome.c);
        CycleUpdated?.Invoke();
    }
    
    // population growth
    private void Growth(CivPoints cycleIncome)
    {
        //////////////////////////////////
        //                              //
        //                              //
        // TODO:                        //
        // GAME RULES ABOUT RESOURCES   //
        //                              //
        //                              //
        //////////////////////////////////
        population *= naturalGrowth;
        PopulationChanged(population);
    }
    #endregion

    #region civilization cycle
    private void CivCycle()
    {
        civilizationPoints = startPoints + Mathf.RoundToInt(Random.Range(-startPointsOffset, startPointsOffset));
        naturalGrowth += Random.Range(-civGrowthOffset, civGrowthOffset);
        civDebuff = new CivPoints(Random.Range(-CivCycleOffset,CivCycleOffset),
                                  Random.Range(-CivCycleOffset,CivCycleOffset),
                                  Random.Range(-CivCycleOffset,CivCycleOffset));
    }
    #endregion

    #region buildings
    public void Build(int type) 
    {
        var bd = new Building(type);
        civilizationPoints -= bd.BasicPrice;
        buildings.Add(bd);
    }
    #endregion
}

public struct CivEvent
{
    public string name;
    public string description;
    public bool once;
    public CivPoints buff;

    public CivEvent(string name, string description, bool once, CivPoints buff)
    {
        this.name = name;
        this.description = description;
        this.once = once;
        this.buff = buff;
    }
}

public struct Population 
{
    public int e;
    public int r;
    public int c;
    public Population(int e, int r, int c)
    {
        this.e = e;
        this.r = r;
        this.c = c;
    }
    public static Population operator +(Population a, Population b)
    {
        return new Population(a.e + b.e, a.r + b.r, a.c + b.c);
    }
    public static Population operator -(Population a, Population b)
    {
        return new Population(a.e - b.e, a.r - b.r, a.c - b.c);
    }
    public static Population operator *(Population a, Population b)
    {
        return new Population(a.e * b.e, a.r * b.r, a.c * b.c);
    }
    public static Population operator /(Population a, Population b)
    {
        return new Population(a.e / b.e, a.r / b.r, a.c / b.c);
    }

    public static Population operator +(Population a, int b)
    {
        return new Population(a.e + b, a.r + b, a.c + b);
    }
    public static Population operator -(Population a, int b)
    {
        return new Population(a.e - b, a.r - b, a.c - b);
    }
    public static Population operator *(Population a, int b)
    {
        return new Population(a.e * b, a.r * b, a.c * b);
    }
    public static Population operator /(Population a, int b)
    {
        return new Population(a.e / b, a.r / b, a.c / b);
    }

    public static Population operator *(Population a, float b)
    {
        return new Population(Mathf.RoundToInt(a.e * b), Mathf.RoundToInt(a.r * b), Mathf.RoundToInt(a.c * b));
    }
    public static Population operator /(Population a, float b)
    {
        return new Population(Mathf.RoundToInt(a.e / b), Mathf.RoundToInt(a.r / b), Mathf.RoundToInt(a.c / b));
    }
}

public struct CivPoints 
{
    public float e;
    public float r;
    public float c;
    public CivPoints(float e, float r, float c)
    {
        this.e = e;
        this.r = r;
        this.c = c;
    }

    public static CivPoints operator +(CivPoints a, CivPoints b)
    {
        return new CivPoints(a.e + b.e, a.r + b.r, a.c + b.c);
    }
    public static CivPoints operator -(CivPoints a, CivPoints b)
    {
        return new CivPoints(a.e - b.e, a.r - b.r, a.c - b.c);
    }
    public static CivPoints operator *(CivPoints a, CivPoints b)
    {
        return new CivPoints(a.e * b.e, a.r * b.r, a.c * b.c);
    }
    public static CivPoints operator /(CivPoints a, CivPoints b)
    {
        return new CivPoints(a.e / b.e, a.r / b.r, a.c / b.c);
    }

    public static CivPoints operator +(CivPoints a, int b)
    {
        return new CivPoints(a.e + b, a.r + b, a.c + b);
    }
    public static CivPoints operator -(CivPoints a, int b)
    {
        return new CivPoints(a.e - b, a.r - b, a.c - b);
    }
    public static CivPoints operator *(CivPoints a, int b)
    {
        return new CivPoints(a.e * b, a.r * b, a.c * b);
    }
    public static CivPoints operator /(CivPoints a, int b)
    {
        return new CivPoints(a.e / b, a.r / b, a.c / b);
    }
}

#region building
public class Building
{
    private BuildingType type;
    private string name = "Building";
    private CivPoints basicIncome;
    private CivPoints basicPrice;
    private List<CivPoints> adds = new List<CivPoints>();
    private List<CivPoints> mults  = new List<CivPoints>();

    public static float mainAdd = 10f;
    public static float subAdd = 6f;
    public static float subSideAdd = 3f;
    public static float mainMin = -2f;
    public static float sideMin = -4f;

    public static float mainPrice = 100f;
    public static float subPrice = 50f;
    public static float lowPrice = 10f;

    public Building(int type)
    {
        this.type = (BuildingType)type;
        switch (type)
        {
            case (int)BuildingType.EconomyE:
                this.name = "Factory";
                this.basicIncome = new CivPoints(mainAdd,mainMin,mainMin);
                this.basicPrice = new CivPoints(mainPrice, 0, 0);
                break;
            case (int)BuildingType.EconomyR:
                this.name = "Church";
                this.basicIncome = new CivPoints(subSideAdd, subAdd,sideMin);
                this.basicPrice = new CivPoints(mainPrice, subPrice, 0);
                break;
            case (int)BuildingType.EconomyC:
                this.name = "Theatre";
                this.basicIncome = new CivPoints(subSideAdd, sideMin, subAdd); 
                this.basicPrice = new CivPoints(mainPrice, 0, subPrice);  
                break;
            case (int)BuildingType.ReligionE:
                this.name = "Monastery";
                this.basicIncome = new CivPoints(subSideAdd, subAdd, sideMin);   
                this.basicPrice = new CivPoints(lowPrice, mainPrice, 0);  
                break;
            case (int)BuildingType.ReligionR:
                this.name = "Cathedral";
                this.basicIncome = new CivPoints(mainMin, mainAdd, mainMin);   
                this.basicPrice = new CivPoints(subPrice, mainPrice, 0);  
                break;
            case (int)BuildingType.ReligionC:
                this.name = "Seminary";
                this.basicIncome = new CivPoints(sideMin, subAdd, subSideAdd);   
                this.basicPrice = new CivPoints(lowPrice, mainPrice, subPrice);  
                break;
            case (int)BuildingType.CultureE:
                this.name = "Mass Media";
                this.basicIncome = new CivPoints(subSideAdd, sideMin, subAdd);   
                this.basicPrice = new CivPoints(lowPrice, subPrice, mainPrice);  
                break;
            case (int)BuildingType.CultureR:
                this.name = "Opera";
                this.basicIncome = new CivPoints(sideMin, subSideAdd, subAdd);   
                this.basicPrice = new CivPoints(lowPrice, 0, mainPrice);  
                break;
            case (int)BuildingType.CultureC:
                this.name = "University";
                this.basicIncome = new CivPoints(mainMin, mainMin, mainAdd);   
                this.basicPrice = new CivPoints(subPrice, 0, mainPrice);  
                break;
        }
    }

    public CivPoints BasicPrice { get => basicPrice; set => basicPrice = value; }

    public void AddAdd(CivPoints add)
    {
        adds.Add(add);
    }
    public void AddMult(CivPoints mult)
    {
        adds.Add(mult);
    }
    public CivPoints Income()
    {
        var v = new CivPoints(0f,0f,0f);
        v += basicIncome;
        foreach (CivPoints a in adds) v += a;
        foreach (CivPoints b in mults) v *= b;
        return v;
    }    
}

public enum BuildingType
{
    EconomyE = 0,
    EconomyR = 1,
    EconomyC = 2,
    ReligionE = 3,
    ReligionR = 4,
    ReligionC = 5,
    CultureE = 6,
    CultureR = 7,
    CultureC = 8
}
#endregion