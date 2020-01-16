using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;

public class QuantumSlot : MonoBehaviour
{
    public Text title, text;
    public int index;
    private List<string> recipeVariants;
    private QuantumCharacterCustomization qcc;
    private string c;

    public void Set(string category, List<UMATextRecipe> variants, QuantumCharacterCustomization qcc)
    {
        this.qcc = qcc;
        c = category;
        recipeVariants = new List<string>();
        title.text = category.BreakupCamelCase();
        foreach (var x in variants) recipeVariants.Add(x.name);
        index = -1;
        text.text = "None";
        qcc.ChangeWardrobe(c, "");
    }

    public void Index(int i)
    {
        index += i;
        if (index >= recipeVariants.Count) index = -1;
        else if (index <= -2) index = recipeVariants.Count - 1;
        if (index == -1)
        {
            text.text = "None";
            qcc.ChangeWardrobe(c, "");
        }
        else
        {
            text.text = "Style " + (index + 1);
            qcc.ChangeWardrobe(c, recipeVariants[index]);
        }
    }

    private string Parse(string x)
    {
        x.BreakupCamelCase();
        x.Replace("_", " ");
        x.Replace("Recipe", "");
        return x;
    }
}
