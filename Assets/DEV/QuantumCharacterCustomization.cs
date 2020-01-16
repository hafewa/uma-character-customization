using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using Photon.Pun;
using Photon.Realtime;
using UMA.CharacterSystem;

public class QuantumCharacterCustomization : MonoBehaviour
{
    #region Init
    private Dictionary<int, string> matrix;

    public SharedColorTable t;
    public DynamicCharacterAvatar dca;

    public GameObject prefab, _prefab, __prefab;
    public Transform bodyFaceList, clothingList, colorList;

    public Slider h, s;
    public Image preview;

    public QuantumPlayer qp;
    public QuantumNetworking qnet;
    private List<string> wardrobeRecipes, colors;

    public void Initialize()
    {
        if (!qnet.pv.IsMine) return;
        dca = GetComponent<DynamicCharacterAvatar>();
        matrix = new Dictionary<int, string>()
        {
            { 0, "eyeSpacing" },
            { 4, "height" },
            { 5, "headSize" },
            { 6, "headWidth" },
            { 7, "neckThickness" },
            { 8, "armLength" },
            { 9, "forearmLength" },

            { 10, "armWidth" },
            { 11, "forearmWidth" },
            { 12, "handSize" },
            { 13, "feetSize" },
            { 14, "legSeparation" },
            { 15, "upperMuscle" },
            { 16, "lowerMuscle" },
            { 17, "upperWeight" },
            { 18, "lowerWeight" },
            { 19, "legSize" },

            { 20, "belly" },
            { 21, "waist" },
            { 22, "gluteusSize" },
            { 23, "earsSize" },
            { 24, "earsPosition" },
            { 25, "earsRotation" },
            { 26, "noseSize" },
            { 27, "noseCurve" },
            { 28, "noseWidth" },
            { 29, "noseInclination" },

            { 30, "nosePosition" },
            { 31, "nosePronounced" },
            { 32, "noseFlatten" },
            { 33, "chinSize" },
            { 34, "chinPronounced" },
            { 35, "chinPosition" },
            { 36, "mandibleSize" },
            { 37, "jawsSize" },
            { 38, "jawsPosition" },
            { 39, "cheekSize" },

            { 40, "cheekPosition" },
            { 41, "lowCheek Pronounced" },
            { 42, "lowCheek Position" },
            { 43, "foreheadSize" },
            { 44, "foreheadPosition" },
            { 45, "lipsSize" },
            { 46, "mouthSize" },
            { 47, "eyeRotation" },
            { 48, "eyeSize" },
            { 49, "breastSize" }
        };

        UpdateDNA();
        UpdateColors();
        UpdateWardrobe();

        if (PlayerPrefs.HasKey("data")) Load();
    }
    #endregion

    #region Update
    private void Update()
    {
        if (!qnet.pv.IsMine && !qp.busy) return;
        ChangeColor("Skin", Color.HSVToRGB(h.value, 1 + s.value, 1 - s.value));
    }

    private void UpdateDNA()
    {
        if (!qnet.pv.IsMine) return;
        foreach (Transform child in bodyFaceList) Destroy(child.gameObject);
        foreach (var x in dca.GetDNA().ToList().OrderBy(v => v.Key))
        {
            if (!x.Key.ToLower().Contains("skin"))
            {
                GameObject o = Instantiate(prefab, bodyFaceList);
                o.GetComponentInChildren<Text>().text = x.Key.BreakupCamelCase();
                Slider slider = o.GetComponentInChildren<Slider>();
                slider.onValueChanged.AddListener(delegate { ChangeValue(x.Key, slider.value); });
                ChangeValue(x.Key, slider.value = 0.5f);
                dca.ForceUpdate(true, false, false);
            }
        }
    }

    private void UpdateWardrobe()
    {
        Wardrobe();
        foreach (Transform child in clothingList) Destroy(child.gameObject);
        foreach (var recipe in wardrobeRecipes) Instantiate(_prefab, clothingList).GetComponent<QuantumSlot>().Set(recipe, dca.AvailableRecipes[recipe], this);
    }

    private void UpdateColors()
    {
        Colors();
        foreach (Transform child in colorList) Destroy(child.gameObject);
        foreach (var color in colors) Instantiate(__prefab, colorList).GetComponent<QuantumColorItem>().Set(color, this);
    }

    private void Colors()
    {
        colors = new List<string>();
        foreach (var color in dca.CurrentSharedColors) if (color.name != "Skin") colors.Add(color.name);
    }

    private void Wardrobe()
    {
        wardrobeRecipes = new List<string>();
        foreach (var recipe in dca.AvailableRecipes) wardrobeRecipes.Add(recipe.Key);
    }
    #endregion

    #region Change
    public void ChangeValue(string index, float value)
    {
        qnet.pv.RPC("RPC_ChangeValue", RpcTarget.AllBuffered, index, value);
        if (!qnet.pv.IsMine) return;
        Save();
    }

    public void ChangeWardrobe(string category, string recipe)
    {
        qnet.pv.RPC("RPC_ChangeWardrobe", RpcTarget.AllBuffered, category, recipe);
        if (!qnet.pv.IsMine) return;
        UpdateColors();
        Save();
    }

    public void ChangeGender(string race)
    {
        qnet.pv.RPC("RPC_ChangeGender", RpcTarget.AllBuffered, race);
        if (!qnet.pv.IsMine) return;
        UpdateWardrobe();
        UpdateColors();
        UpdateDNA();
        Save();
    }

    public void ChangeColor(string name, Color color)
    {
        qnet.pv.RPC("RPC_ChangeColor", RpcTarget.AllBuffered, name, color.r, color.g, color.b);
        dca.UpdateColors();
    }
    #endregion

    public void Load()
    {

    }

    public void Save()
    {
        //string skinTone = h.value + "#" + s.value;
        //string gender = dca.activeRace.name;
    }

    #region RPC
    [PunRPC]
    public void RPC_ChangeColor(string name, float r, float g, float b)
    {
        Color color = new Color(r, g, b, 1);
        t.colors[0].color = color;
        dca.SetColor(name, t.colors[0]);
        if (qnet.pv.IsMine) preview.color = color;
    }

    [PunRPC]
    public void RPC_ChangeGender(string race)
    {
        dca.ChangeRace(race);
        dca.BuildCharacter(true);
        dca.ForceUpdate(true, false, false);
    }

    [PunRPC]
    public void RPC_ChangeWardrobe(string category, string recipe)
    {
        if (recipe != "") dca.SetSlot(category, recipe);
        else dca.ClearSlot(category);
        dca.BuildCharacter(true);
    }

    [PunRPC]
    public void RPC_ChangeValue(string index, float value)
    {
        dca.GetDNA()[index].Set(value);
        dca.ForceUpdate(true, false, false);
    }
    #endregion
}
