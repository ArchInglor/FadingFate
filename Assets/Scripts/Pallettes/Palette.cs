using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CoughTeam/Palette")]
public class Palette : ScriptableObject
{
    public Color colorEconomy;
    public Color colorReligion;
    public Color colorCulture;
    public Color colorEarth;
    public Color colorWater;
    public Color colorSpace;
    public float offset;
}

public enum PaletteColor
{
    colorEconomy,
    colorReligion,
    colorCulture,
    colorEarth,
    colorWater,
    colorSpace
}