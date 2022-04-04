using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorSetter))]
public class SitizenView : MonoBehaviour
{
    private ColorSetter _colorSetter;
    private SitizenViewType _type;

    public SitizenViewType Type => _type;

    private void Awake()
    {
        _colorSetter = GetComponent<ColorSetter>();
    }

    public void UpdateType(SitizenViewType type)
    {
        _type = type;
        switch (type)
        {
            case SitizenViewType.Economy:
                _colorSetter.UpdateColor(PaletteColor.colorEconomy);
                break;
            case SitizenViewType.Religion:
                _colorSetter.UpdateColor(PaletteColor.colorReligion);
                break;
            case SitizenViewType.Culture:
                _colorSetter.UpdateColor(PaletteColor.colorCulture);
                break;
        }
    }
}

public enum SitizenViewType
{
    Economy,
    Religion,
    Culture
}
