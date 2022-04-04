using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorSetter))]
public class SitizenView : MonoBehaviour
{
    private ColorSetter[] _colorSetters;
    private SitizenViewType _type;

    public SitizenViewType Type => _type;

    private void Awake()
    {
        _colorSetters = GetComponents<ColorSetter>();
    }

    public void UpdateType(SitizenViewType type)
    {
        _type = type;
        switch (type)
        {
            case SitizenViewType.Economy:
                foreach (var colorSetter in _colorSetters)
                {

                    colorSetter.UpdateColor(PaletteColor.colorEconomy);
                }
                break;
            case SitizenViewType.Religion:
                foreach (var colorSetter in _colorSetters)
                {

                    colorSetter.UpdateColor(PaletteColor.colorReligion);
                }
                break;
            case SitizenViewType.Culture:
                foreach (var colorSetter in _colorSetters)
                {

                    colorSetter.UpdateColor(PaletteColor.colorCulture);
                }
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
