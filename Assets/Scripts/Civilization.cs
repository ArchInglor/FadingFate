using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Civilization : MonoBehaviour
{
    public static Civilization instance = null;

    #region parameters
    private static float civCycleOffset = 5f;
    private static float civGrowthOffset = 0.02f;
    private static int startPointsOffset = 250;
    private static int startPopOffset = 20;
    private static CivPoints startPoints = new CivPoints(500, 500, 500);
    private static Population startPop = new Population(50, 50, 50);
    [SerializeField] private float lowEvent = 2f;
    [SerializeField] private float midEvent = 4f;
    [SerializeField] private float maxEvent = 8f;
    [SerializeField] private float civConsumption = 0.3f;
    [SerializeField] private float naturalGrowth = 1.01f;
    public static float CivCycleOffset { get => civCycleOffset; }

    [Header("Buildings")]
    [SerializeField]public float mainAdd = 10f;
    [SerializeField]public float subAdd = 6f;
    [SerializeField]public float subSideAdd = 3f;
    [SerializeField]public float mainMin = -2f;
    [SerializeField]public float sideMin = -4f;
    [Space(10)]
    [SerializeField]public float mainPrice = 50f;
    [SerializeField]public float subPrice = 25f;
    [SerializeField]public float lowPrice = 5f;

    private int overpop1 = 500;
    private int overpop2 = 1000;
    private int overpop3 = 2000;
    #endregion

    #region civilization
    private CivPoints civilizationPoints = new CivPoints(0, 0, 0);
    private CivPoints deltaCivPoints = new CivPoints(0, 0, 0);
    private Population population = new Population(50, 50, 50);
    private Population deltaPopulation = new Population(0, 0, 0);
    private float tension = 0f;    
    private CivPoints civDebuff = new CivPoints(0, 0, 0);
    private List<Building> buildings = new List<Building>();

    private float cycleGrowth = 1f;
    private List<GrowthBuff> growthBuffs = new List<GrowthBuff>();

    private bool recyclingOfDead = false;
    #endregion

    #region game cycle
    private bool cycling = false;
    [SerializeField] private float cycleDuration = 1000f;
    private float cycleTime = 0f;
    private int cycleCounter = 0;
    private bool paused = false;
    #endregion

    
    #region civilization cycle
    private IEnumerator endCivCycle;
    #endregion

    [Header("Objects")]
    #region objects
    [SerializeField] private GameObject nuclearWarPrefab;
    [SerializeField] private GameObject enlightenmentPrefab;
    [SerializeField] private GameObject meteoritePrefab;
    private GameObject uiObject;
    [SerializeField] private GameObject eventTextPrefab;
    [SerializeField] private GameObject smallEventTextPrefab;
    private TextMeshProUGUI text;
    private TextMeshProUGUI popText;
    private Text debugText;
    private Image pauseLabel;
    private Text pauseLabelText;
    private Scrollbar cycleScroll;
    #endregion    

    private void Awake() 
    {
        instance = this;

        uiObject = GameObject.Find("UI");
        text = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
        popText = GameObject.Find("Population").GetComponent<TextMeshProUGUI>();
        debugText = GameObject.Find("DebugText").GetComponent<Text>();

        pauseLabel = GameObject.Find("Pause").GetComponent<Image>();
        pauseLabelText = pauseLabel.GetComponentInChildren<Text>();

        Color panelColor = PalettesSystem.instance.GetColor(PaletteColor.colorCulture);
        pauseLabelText.color = panelColor;
        pauseLabelText.text = "<b>Paused</b>";
        panelColor.a = 0.1f;
        pauseLabel.color = panelColor;        
        pauseLabel.gameObject.SetActive(false);

        cycleScroll = GameObject.Find("CycleScroll").GetComponent<Scrollbar>();

    }
    private void Start() {

        CivCycle();
        Cycle();
    }
    private void Update() 
    {   
        if (cycling) cycleTime += 1*Time.deltaTime;
        cycleScroll.size = cycleTime/cycleDuration;
        if (cycleTime >= cycleDuration)
        {
            Cycle();
            cycleCounter++;
            cycleTime = 0f;              
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;

            pauseLabel.gameObject.SetActive(paused);

            if (paused) Time.timeScale = 0.01f;
            else Time.timeScale = 1f;
        }
    }

    public void TextUpdate()
    {
        string eGrowth = "    ";
        if (civilizationPoints.e >= deltaCivPoints.e) eGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else eGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        string rGrowth = "    ";
        if (civilizationPoints.r >= deltaCivPoints.r) rGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else rGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        string cGrowth = "    ";
        if (civilizationPoints.c >= deltaCivPoints.c) cGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else cGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        if (tension < 0f) tension = 0f;

        text.text = "<b>Civilization</b> <br>";
        text.text += "<voffset=0.5em><size=200%><sprite=0></size></voffset> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + Mathf.RoundToInt(civilizationPoints.e) +
         "</color>" + eGrowth + "</b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=1></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">" + Mathf.RoundToInt(civilizationPoints.r) +
         "</color>" + rGrowth + "</b><br>";
        text.text += "<voffset=0.5em><size=200%><sprite=2></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">" + Mathf.RoundToInt(civilizationPoints.c) + 
         "</color>" + cGrowth + "</b><br>";
        text.text += "<br><br><voffset=0.5em><size=200%><sprite=3></size></voffset></color> <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEarth)) + ">" + Mathf.RoundToInt(tension*100) + "%</color></b><br>";

        string popeGrowth = "";
        if (population.e >= deltaPopulation.e) popeGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else popeGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        string poprGrowth = "";
        if (population.r >= deltaPopulation.r) poprGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else poprGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        string popcGrowth = "";
        if (population.c >= deltaPopulation.c) popcGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorReligion)) + ">⯅</color>";
        else popcGrowth += "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + ">⯆</color>";

        popText.text = "<b>Population</b> <br>";
        popText.text += popeGrowth + "  <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + population.e +
         "</color></b>" + "    <voffset=0.5em><size=200%><sprite=0></size></voffset><br>";
        popText.text += poprGrowth + "  <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + population.r +
         "</color></b>" + "    <voffset=0.5em><size=200%><sprite=1></size></voffset><br>";
        popText.text += popcGrowth + "  <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + population.c +
         "</color></b>" + "    <voffset=0.5em><size=200%><sprite=2></size></voffset><br><br>";
         popText.text += popcGrowth + "  <b><color=#" +
         ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEconomy)) + ">" + ((float)Mathf.Round(cycleGrowth * 100f) / 100f) +
         "</color></b>" + "    <voffset=0.5em><size=200%><sprite=4></size></voffset><br>";
        
        debugText.text = "";
        if (Application.isEditor)
        {
            debugText.text += "Pop: (" + population.e + ", " + population.r + ", " + population.c + "), ";
            debugText.text += "PopGrowth: " + (float)Mathf.Round(cycleGrowth * 100f) / 100f + ", ";
        }

    }

    #region game cycle
    private void Cycle() 
    {        
        CivPoints cycleIncome = new CivPoints(0, 0, 0);
        foreach (Building building in buildings) 
        {
            cycleIncome += building.Income();
        }
        civilizationPoints += cycleIncome; 
        if (recyclingOfDead && population.Sum-deltaPopulation.Sum < 0) 
        {
            var deadIncome = Mathf.RoundToInt((population.Sum-deltaPopulation.Sum)/3);
            civilizationPoints += deadIncome;
        }        
        Growth();        
        
        if (civilizationPoints.e <= 0)
        {
            civilizationPoints.e = 0;
            CivilizationsDeath("CrisisEconomy");
        }
        if (civilizationPoints.r <= 0)
        {
            civilizationPoints.r = 0;
            CivilizationsDeath("CrisisReligion");
        }
        if (civilizationPoints.c <= 0)
        {
            civilizationPoints.c = 0;
            CivilizationsDeath("CrisisCulture");
        }

        TextUpdate();
        deltaCivPoints = civilizationPoints;
        deltaPopulation = population;
    }
    
    // population growth
    private void Growth()
    {
        cycleGrowth = naturalGrowth;

        #region economy events
        if (civilizationPoints.e > civilizationPoints.r + civilizationPoints.c && civilizationPoints.e > population.Sum*10 && !HasGrowthBuff("Overproduction"))
        {     
            GrowthBuff gb = new GrowthBuff("Overproduction", "People is too happy with amount of goods. Growth decreased.", 0.8f);
            growthBuffs.Add(gb);
            Event(gb, true);
        }
        if (civilizationPoints.e < (civilizationPoints.r + civilizationPoints.c)/maxEvent && civilizationPoints.e < population.Sum*2 && !HasGrowthBuff("Underproduction"))
        {     
            GrowthBuff gb = new GrowthBuff("Underproduction", "People have not enough goods. Growth brutely decreased.", 0.6f);
            growthBuffs.Add(gb);
            Event(gb);
        }
        if (civilizationPoints.e/midEvent > (civilizationPoints.r + civilizationPoints.c) && !HasGrowthBuff("Glory of Capitalism"))
        {     
            GrowthBuff gb = new GrowthBuff("Glory of Capitalism", "Everyone knows its problems and no one can do anything about it. Someone's stil trying tho.", 1f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += 0.2f;
        }
        #endregion
        #region religion events
        if (civilizationPoints.r > civilizationPoints.e + civilizationPoints.c && !HasGrowthBuff("Enlightenment"))
        {
            GrowthBuff gb = new GrowthBuff("Enlightenment", "People comperhended the divine and tend to reunite with the universe.", 0.2f);
            growthBuffs.Add(gb);
            Event(gb);
            CivilizationsDeath("Enlightenment");
        }
        if (civilizationPoints.r < (civilizationPoints.e + civilizationPoints.c)/maxEvent && !HasGrowthBuff("Depravity"))
        {
            GrowthBuff gb = new GrowthBuff("Depravity", "People becoming more like an animals. Growth increased.", 1.1f);
            growthBuffs.Add(gb);
            Event(gb, true);
        }
        if (civilizationPoints.r/midEvent > (civilizationPoints.e + civilizationPoints.c) && !HasGrowthBuff("The Church"))
        {
            GrowthBuff gb = new GrowthBuff("The Church", "God's in control of everything so why should government be more important?", 1f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += 0.15f;
        }
        #endregion
        #region culture events
        if (civilizationPoints.c > civilizationPoints.e + civilizationPoints.r && !HasGrowthBuff("Singularity"))
        {
            GrowthBuff gb = new GrowthBuff("Singularity", "We are convinced that there is nothing more to investigate.", 1f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension -= 0.15f;
            var pc = population.c/2;
            population.c -= pc;
            population.e += pc/2;
            population.r += pc/2;
        }
        if (civilizationPoints.c < (civilizationPoints.e + civilizationPoints.r)/maxEvent && !HasGrowthBuff("Ignorance"))
        {
            GrowthBuff gb = new GrowthBuff("Ignorance", "People are not accustomed to think a lot. Growth increased.", 1.05f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension -= 0.3f;
        }
        if (civilizationPoints.c/midEvent > (civilizationPoints.e + civilizationPoints.r) && !HasGrowthBuff("Class Society"))
        {
            GrowthBuff gb = new GrowthBuff("Class Society", "Some people just incapable.", 1f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += 0.15f;
        }
        #endregion

        #region tension events
        if (tension > 0.3f && !HasGrowthBuff("Tensity"))
        {
            GrowthBuff gb = new GrowthBuff("Tensity", "People don't feel good about each other.", 0.99f);
            growthBuffs.Add(gb);
            Event(gb, true);
        }
        if (tension > 0.5f && !HasGrowthBuff("Disorder"))
        {
            GrowthBuff gb = new GrowthBuff("Disorder", "Amount of conflicts is growing up.", 0.95f);
            growthBuffs.Add(gb);
            Event(gb, true);
        }
        if (tension > 0.9f && !HasGrowthBuff("World War"))
        {
            GrowthBuff gb = new GrowthBuff("World War", "Species basically liquidates itself.", 0.5f);
            growthBuffs.Add(gb);
            Event(gb);
        }
        if (tension > 0.95f && !HasGrowthBuff("5-min War"))
        {
            GrowthBuff gb = new GrowthBuff("5-min War", "Leaders have used weapons of mass destruction. No one asked you.", 0.05f);
            growthBuffs.Add(gb);
            Event(gb);
            CivilizationsDeath("NuclearWar");
        }
        #endregion

        #region overpopulation events
        if (population.Sum > overpop1  && !HasGrowthBuff("High Population"))
        {
            GrowthBuff gb = new GrowthBuff("High Population", "People know there are many.", 0.99f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += 0.2f;
        }
        if (population.Sum > overpop2  && !HasGrowthBuff("Overpopulation"))
        {
            GrowthBuff gb = new GrowthBuff("Overpopulation", "People struggle to live on this small planet.", 0.9f);            
            growthBuffs.Add(gb);
            Event(gb);

            tension += 0.3f;
        }
        if (population.Sum > overpop3  && !HasGrowthBuff("Insane Population"))
        {
            GrowthBuff gb = new GrowthBuff("Insane Population", "People just can't live in all of their wideness.", 0.85f);
            growthBuffs.Add(gb);
            Event(gb);

            tension += 0.4f;
        }
        #endregion

        #region factions events
        // majorities
        if (population.e > (population.r + population.c)*lowEvent && !HasGrowthBuff("Investment in Ignorance"))
        {
            GrowthBuff gb = new GrowthBuff("Investment in Ignorance", "Intelligence was neatly executed.", 1.02f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += -0.2f;
            population.c = Mathf.RoundToInt(population.c*0.5f);
        }
        if (population.r > (population.e + population.c)*lowEvent && !HasGrowthBuff("The Word of God"))
        {
            GrowthBuff gb = new GrowthBuff("The Word of God", "Apparently God simply doesn't like science he himself created.", 1.02f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += -0.2f;
            population.c = Mathf.RoundToInt(population.c*0.5f);
        }
        if (population.c > (population.r + population.e)*lowEvent && !HasGrowthBuff("Atheism"))
        {
            GrowthBuff gb = new GrowthBuff("Atheism", "Believeres were executed because of a single fanatic.", 0.99f);
            growthBuffs.Add(gb);
            Event(gb, true);

            tension += -0.2f;
            population.r = Mathf.RoundToInt(population.r*0.5f);
        }

        // minorities
        if ((population.r + population.c)/midEvent > population.e && !HasGrowthBuff("Rich Minority"))
        {
            GrowthBuff gb = new GrowthBuff("Rich Minority", "People just realized where's all the money.", 1f);
            growthBuffs.Add(gb);
            Event(gb);

            tension += 0.4f;
        }
        if ((population.r + population.e)/midEvent > population.c && !HasGrowthBuff("Offended Artist"))
        {
            GrowthBuff gb = new GrowthBuff("Offended Artist", "Sometimes artist can be just as dangerous as a whole war. Artist can also lead to war and genocide.", 1f);
            growthBuffs.Add(gb);
            Event(gb);

            tension += 0.5f;
            population *= 0.5f;
        }
        #region cult events
        if ((population.e + population.c)/midEvent > population.r  && !HasGrowthBuff("Cult"))
        {
            GrowthBuff gb = new GrowthBuff("Cult", "You may not believe in the occult science. Better for us.", 1f);
            growthBuffs.Add(gb);
            Event(gb);

            float ev = Random.Range(0f,1f);
            if (0 <= ev && ev < 0.25f)
            {
                GrowthBuff cultSpell = new GrowthBuff("CULT Aggressive recruitment.", "Intellectuals are joining secret societies.", 1f);
                growthBuffs.Add(cultSpell);
                Event(cultSpell);

                var cMen = Mathf.RoundToInt(population.c*0.25f);
                population.c -= cMen;
                population.r += cMen;
                tension += 0.2f;
            }
            else if (0.25f <= ev && ev < 0.5f)
            {
                GrowthBuff cultSpell = new GrowthBuff("CULT Progress inquisition.", "Factories are demolished. Mass production canceled.", 1.05f);
                growthBuffs.Add(cultSpell);
                Event(cultSpell);

                var cMen = Mathf.RoundToInt(population.e*0.75f);
                population.e -= cMen;
                population.r += cMen;
                var cp = civilizationPoints.e*0.75f;
                civilizationPoints.e -= cp;
                civilizationPoints.r += cp;
                tension = 0.1f;
            }
            else if (0.5f <= ev && ev < 0.75f)
            {
                GrowthBuff cultSpell = new GrowthBuff("CULT The Great Sacrifice.", "Nations were murdered in the name of the dark deity.", 1.2f);
                growthBuffs.Add(cultSpell);
                Event(cultSpell);

                population *= 0.5f;
                civilizationPoints *= 1.5f;
                tension = 0.1f;
            }
            else if (0.75f <= ev && ev < 1f)
            {
                GrowthBuff cultSpell = new GrowthBuff("CULT Recycling of Dead.", "They're the source of the god given bioenergy.", 1.2f);
                growthBuffs.Add(cultSpell);
                Event(cultSpell);

                recyclingOfDead = true;
                tension = 0.1f;
            }

            tension += -0.2f;
            population.r = Mathf.RoundToInt(population.r*0.5f);
        }
        #endregion
        #endregion

        foreach (GrowthBuff gb in growthBuffs) cycleGrowth *= gb.buff;
        civilizationPoints -= population*civConsumption;
        population *= cycleGrowth;
    }
    #endregion

    #region civilization cycle
    private void CivCycle()
    {
        PalettesSystem.instance.RandomPalette();
        deltaCivPoints = new CivPoints(0,0,0);
        deltaPopulation = new Population(0,0,0);
        civilizationPoints = startPoints + Mathf.RoundToInt(Random.Range(-startPointsOffset, startPointsOffset));
        population = new Population(startPop.e + Random.Range(-startPopOffset, startPopOffset),
                                    startPop.r + Random.Range(-startPopOffset, startPopOffset),
                                    startPop.c + Random.Range(-startPopOffset, startPopOffset));
        naturalGrowth += Random.Range(-civGrowthOffset, civGrowthOffset);
        civDebuff = new CivPoints(Random.Range(-CivCycleOffset,CivCycleOffset),
                                  Random.Range(-CivCycleOffset,CivCycleOffset),
                                  Random.Range(-CivCycleOffset,CivCycleOffset));
        growthBuffs = new List<GrowthBuff>();
        buildings = new List<Building>();
        cycling = true;
        TextUpdate();
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

    public void Event(string text, bool small = false)
    {
        if (cycling) 
        {            
            var evText = new GameObject();
            if (small) evText = Instantiate(smallEventTextPrefab, uiObject.transform);
            else  evText = Instantiate(eventTextPrefab, uiObject.transform);
            evText.GetComponent<Text>().text = text;
            Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorCulture)) + "><b>[EVENT]</b></color> " + text);
        }
    }
    public void Event(GrowthBuff growthBuff, bool small = false)
    {
        string txt = "<color=#" + ColorUtility.ToHtmlStringRGB(PalettesSystem.instance.GetColor(PaletteColor.colorEarth)) + ">" + growthBuff.name + " </color>";
        txt += growthBuff.description;
        Event(txt, small);
    }
    public bool HasGrowthBuff(string gbname)
    {
        bool has = false;
        foreach (GrowthBuff gb in growthBuffs) if (gb.name == gbname) has = true;
        return has;
    }

    public void CivilizationsDeath(string scenarioName)
    {
        GameObject endAnim = new GameObject();
        switch (scenarioName)
        {
            case "CrisisEconomy":
                Event("Brute economy crisis left only ruins.");
                break;
            case "CrisisReligion":
                Event("God is dead, reject consciousness, embrace the chaos.");
                break;
            case "CrisisCulture":
                Event("People are happy living in anarcho primitivism. Civilization died tho.");
                break;
            case "Extinction":
                break;
            case "NuclearWar":
                endAnim = Instantiate(nuclearWarPrefab, this.transform);
                break;   
            case "Enlightenment":
                endAnim = Instantiate(enlightenmentPrefab, this.transform);
                break;
            case "Meteorite":
                endAnim = Instantiate(meteoritePrefab, this.transform);
                break;          
        }
        endCivCycle = EndCivCycle(endAnim);
        StartCoroutine(endCivCycle);
    }

    public IEnumerator EndCivCycle(GameObject endAnim)
    {
        cycling = false;
        yield return new WaitForSeconds(5);
        Destroy(endAnim);
        CivCycle();
        StopCoroutine(endCivCycle);
    }
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
public struct GrowthBuff
{
    public string name;
    public string description;
    public float buff;

    public GrowthBuff(string name, string description, float buff)
    {
        this.name = name;
        this.description = description;
        this.buff = buff;
    }
}

public class Population 
{
    public int e;
    public int r;
    public int c;   

    public int Sum { get => e+r+c; }

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

    public static CivPoints operator +(CivPoints a, Population b)
    {
        return new CivPoints(a.e + b.e, a.r + b.r, a.c + b.c);
    }
    public static CivPoints operator -(CivPoints a, Population b)
    {
        return new CivPoints(a.e - b.e, a.r - b.r, a.c - b.c);
    }
    public static CivPoints operator *(CivPoints a, Population b)
    {
        return new CivPoints(a.e * b.e, a.r * b.r, a.c * b.c);
    }
    public static CivPoints operator /(CivPoints a, Population b)
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

    public static CivPoints operator +(CivPoints a, float b)
    {
        return new CivPoints(a.e + b, a.r + b, a.c + b);
    }
    public static CivPoints operator -(CivPoints a, float b)
    {
        return new CivPoints(a.e - b, a.r - b, a.c - b);
    }
    public static CivPoints operator *(CivPoints a, float b)
    {
        return new CivPoints(a.e * b, a.r * b, a.c * b);
    }
    public static CivPoints operator /(CivPoints a, float b)
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

    public Building(int type)
    {
        this.type = (BuildingType)type;
        switch (type)
        {
            case (int)BuildingType.EconomyE:
                this.name = "Factory";
                this.basicIncome = new CivPoints(Civilization.instance.mainAdd,Civilization.instance.mainMin,Civilization.instance.mainMin);
                this.basicPrice = new CivPoints(Civilization.instance.mainPrice, 0, 0);
                break;
            case (int)BuildingType.EconomyR:
                this.name = "Church";
                this.basicIncome = new CivPoints(Civilization.instance.subSideAdd, Civilization.instance.subAdd,Civilization.instance.sideMin);
                this.basicPrice = new CivPoints(Civilization.instance.mainPrice, Civilization.instance.subPrice, 0);
                break;
            case (int)BuildingType.EconomyC:
                this.name = "Theatre";
                this.basicIncome = new CivPoints(Civilization.instance.subSideAdd, Civilization.instance.sideMin, Civilization.instance.subAdd); 
                this.basicPrice = new CivPoints(Civilization.instance.mainPrice, 0, Civilization.instance.subPrice);  
                break;
            case (int)BuildingType.ReligionE:
                this.name = "Monastery";
                this.basicIncome = new CivPoints(Civilization.instance.subSideAdd, Civilization.instance.subAdd, Civilization.instance.sideMin);   
                this.basicPrice = new CivPoints(Civilization.instance.lowPrice, Civilization.instance.mainPrice, 0);  
                break;
            case (int)BuildingType.ReligionR:
                this.name = "Cathedral";
                this.basicIncome = new CivPoints(Civilization.instance.mainMin, Civilization.instance.mainAdd, Civilization.instance.mainMin);   
                this.basicPrice = new CivPoints(Civilization.instance.subPrice, Civilization.instance.mainPrice, 0);  
                break;
            case (int)BuildingType.ReligionC:
                this.name = "Seminary";
                this.basicIncome = new CivPoints(Civilization.instance.sideMin, Civilization.instance.subAdd, Civilization.instance.subSideAdd);   
                this.basicPrice = new CivPoints(Civilization.instance.lowPrice, Civilization.instance.mainPrice, Civilization.instance.subPrice);  
                break;
            case (int)BuildingType.CultureE:
                this.name = "Mass Media";
                this.basicIncome = new CivPoints(Civilization.instance.subSideAdd, Civilization.instance.sideMin, Civilization.instance.subAdd);   
                this.basicPrice = new CivPoints(Civilization.instance.lowPrice, Civilization.instance.subPrice, Civilization.instance.mainPrice);  
                break;
            case (int)BuildingType.CultureR:
                this.name = "Opera";
                this.basicIncome = new CivPoints(Civilization.instance.sideMin, Civilization.instance.subSideAdd, Civilization.instance.subAdd);   
                this.basicPrice = new CivPoints(Civilization.instance.lowPrice, 0, Civilization.instance.mainPrice);  
                break;
            case (int)BuildingType.CultureC:
                this.name = "University";
                this.basicIncome = new CivPoints(Civilization.instance.mainMin, Civilization.instance.mainMin, Civilization.instance.mainAdd);   
                this.basicPrice = new CivPoints(Civilization.instance.subPrice, 0, Civilization.instance.mainPrice);  
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