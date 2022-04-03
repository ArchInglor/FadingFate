using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CoughTeam/Palette")]
public class Palette : ScriptableObject
{
    public Color colorCulture;
    public Color colorEconomy;
    public Color colorReligion;
    public Color colorEarth;
    public Color colorWater;
    public Color colorSpace;
    public float offset;
}

public enum PaletteColor
{
    colorCulture,
    colorEconomy,
    colorReligion,
    colorEarth,
    colorWater,
    colorSpace
}