using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Civilization : MonoBehaviour
{
    #region parameters
    private static float civCycleOffset = 5f;
    private static float civGrowthOffset = 3f;
    #endregion

    #region civilization
    private CivPoints civilizationPoints = new CivPoints(100000, 21342134, 2342);
    private Population population = new Population(0, 0, 0);
    private float tension = 0f;
    private List<Building> buildings = new List<Building>();
    private float naturalGrowth = 1.2f;
    #endregion

    #region game cycle
    [SerializeField] private float cycleDuration = 1000f;
    private float cycleTime = 0f;
    private int cycleCounter = 0;
    public static float CivCycleOffset { get => civCycleOffset; }
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
            Cycle();
            cycleCounter++;
            cycleTime = 0f;            
        }

        text.text = "";
        text.text += "<voffset=0.5em><size=200%><sprite=0></size></voffset> <b><color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + civilizationPoints.e + "</color></b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=1></size></voffset></color> <b><color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">" + civilizationPoints.r + "</color></b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=2></size></voffset></color> <b><color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">" + civilizationPoints.c + "</color></b><br>";
        text.text += "<br><br><voffset=0.5em><size=200%><sprite=3></size></voffset></color> <b><color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEarth)) + ">" + Mathf.RoundToInt(tension*100) + "%</color></b><br>";
        
    }

    #region game cycle
    private void Cycle() 
    {
        CivPoints cycleIncome = new CivPoints(0, 0, 0);
        foreach (Building building in buildings) cycleIncome += building.Income();
        civilizationPoints += cycleIncome; 
        Growth(cycleIncome);
    }
    
    // population growth
    private void Growth(CivPoints cycleIncome)
    {
        
    }
    #endregion

    #region civ cycle
    private void CivCycle()
    {
        naturalGrowth += Random.Range(-civGrowthOffset, civGrowthOffset);
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
}

public class Building
{
    private string name = "Building";
    private CivPoints basicIncome = new CivPoints(0f,0f,0f);
    private CivPoints basicPrice = new CivPoints(0f,0f,0f);
    private List<CivPoints> adds;
    private List<CivPoints> mults;

    public static float mainAdd = 10f;
    public static float subAdd = 6f;
    public static float subSideAdd = 3f;
    public static float mainMin = -2f;
    public static float sideMin = -4f;

    public static float mainPrice = 100f;
    public static float subPrice = 50f;
    public static float lowPrice = 10f;

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

    public void CivCycle()
    {
            basicIncome += new CivPoints(
                Random.Range(-Civilization.CivCycleOffset, Civilization.CivCycleOffset),
                Random.Range(-Civilization.CivCycleOffset, Civilization.CivCycleOffset),
                Random.Range(-Civilization.CivCycleOffset, Civilization.CivCycleOffset)
            );
    }
}
public class EconomyE : Building
{
    private string name = "Factory";
    private CivPoints basicIncome = new CivPoints(mainAdd,-mainMin,-mainMin);    
    private CivPoints basicPrice = new CivPoints(mainPrice, 0, 0);
}
public class EconomyR : Building
{
    private string name = "Church";
    private CivPoints basicIncome = new CivPoints(subSideAdd, subAdd,-sideMin);
    private CivPoints basicPrice = new CivPoints(mainPrice, subPrice, 0);
}
public class EconomyC : Building
{
    private string name = "Theatre";
    private CivPoints basicIncome = new CivPoints(subSideAdd, -sideMin, subAdd); 
    private CivPoints basicPrice = new CivPoints(mainPrice, 0, subPrice);  
}

public class ReligionE : Building
{
    private string name = "Cathedral";
    private CivPoints basicIncome = new CivPoints(-mainMin, mainAdd, -mainMin);   
    private CivPoints basicPrice = new CivPoints(subPrice, mainPrice, 0);  
}
public class ReligionR : Building
{
    private string name = "Monastery";
    private CivPoints basicIncome = new CivPoints(subSideAdd, subAdd, -sideMin);   
    private CivPoints basicPrice = new CivPoints(lowPrice, mainPrice, 0);  
}
public class ReligionC : Building
{
    private string name = "Seminary";
    private CivPoints basicIncome = new CivPoints(-sideMin, -subAdd, subSideAdd);   
    private CivPoints basicPrice = new CivPoints(lowPrice, mainPrice, subPrice);  
}

public class CultureE : Building
{
    private string name = "University";
    private CivPoints basicIncome = new CivPoints(-mainMin, -mainMin, mainAdd);   
    private CivPoints basicPrice = new CivPoints(subPrice, 0, mainPrice);  
}
public class CultureR : Building
{
    private string name = "Mass Media";
    private CivPoints basicIncome = new CivPoints(subSideAdd, -sideMin, subAdd);   
    private CivPoints basicPrice = new CivPoints(lowPrice, subPrice, mainPrice);  
}
public class CultureC : Building
{
    private string name = "Opera";
    private CivPoints basicIncome = new CivPoints(-sideMin, subSideAdd, subAdd);   
    private CivPoints basicPrice = new CivPoints(lowPrice, 0, mainPrice);  
}
