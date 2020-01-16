using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA.CharacterSystem;

public class QuantumColorItem : MonoBehaviour
{
    public Text title;
    public Image preview;
    public Slider color, saturation;

    private string category;
    private QuantumCharacterCustomization qcc;

    public void Set(string category, QuantumCharacterCustomization qcc)
    {
        this.qcc = qcc;
        this.category = category;
        title.text = category.BreakupCamelCase();
        saturation.value = -1;
    }

    private void Update()
    {
        if (category == "") return;
        qcc.ChangeColor(category, preview.color = Color.HSVToRGB(color.value, 1 + saturation.value, 1 - saturation.value));
    }
}
