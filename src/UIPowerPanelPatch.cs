using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MegaRayReceiver;

class UIPowerPanelPatch
{
    public static int Multiplier = 10;
    public static int SliderMax = 20;

    public static ManualLogSource Log;
    static bool initialized;
    static GameObject group;
    static InputField inputMultiplier;
    static Slider sliderMultiplier;
    static Text text_factory;
    static bool eventLock;
    static UITooltip tipInputMultiplier;
    static UITooltip tipSliderMultiplier;

    [HarmonyPostfix, HarmonyPatch(typeof(UIStatisticsWindow), nameof(UIStatisticsWindow._OnOpen))]
    public static void Init()
    {
        if (group == null)
        {
            try
            {
                Slider slider0 = UIRoot.instance.uiGame.dysonEditor.controlPanel.inspector.layerInfo.slider0;
                InputField input0 = UIRoot.instance.uiGame.dysonEditor.controlPanel.inspector.layerInfo.input0;
                Text text0 = UIRoot.instance.uiGame.statWindow.performancePanelUI.cpuValueText1;
                GameObject panelObj = UIRoot.instance.uiGame.statWindow.powerAstroBox.gameObject.transform.parent.gameObject;
                RectTransform copyTransform = UIRoot.instance.uiGame.statWindow.powerAstroBox.gameObject.GetComponent<RectTransform>();

                group = new GameObject("RayReceiver_Group");
                group.transform.SetParent(panelObj.transform);
                group.AddComponent<RectTransform>();
                group.transform.localPosition = new Vector3(copyTransform.localPosition.x, copyTransform.localPosition.y);
                group.transform.localScale = copyTransform.localScale;                
                group.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                group.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                //group.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                group.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

                GameObject tmp = GameObject.Instantiate(text0.gameObject, group.transform);
                tmp.name = "text_rayrecmul";
                tmp.transform.localPosition = new Vector3(-50, 19);
                text_factory = tmp.GetComponent<Text>();
                text_factory.text = "Ray Multiplier".Translate();

                tmp = GameObject.Instantiate(input0.gameObject, group.transform);
                tmp.name = "input_rayrecmul";
                tmp.transform.localPosition = new Vector3(155, 11);
                tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 20);
                inputMultiplier = tmp.GetComponent<InputField>();
                inputMultiplier.characterValidation = InputField.CharacterValidation.Integer;
                inputMultiplier.contentType = InputField.ContentType.IntegerNumber;
                inputMultiplier.onEndEdit.AddListener(new UnityAction<string>(OnInputValueEnd));
                tipInputMultiplier = tmp.AddComponent<UITooltip>();
                tipInputMultiplier.Title = "Energy Cap Multiplier".Translate();
                tipInputMultiplier.Text = "Multiplies the ray receiver's energy cap by the given amount.".Translate();

                tmp = GameObject.Instantiate(slider0.gameObject, group.transform);
                tmp.name = "slider_rayrecmul";
                tmp.transform.localPosition = new Vector3(60, -10, -2);
                sliderMultiplier = tmp.GetComponent<Slider>();
                sliderMultiplier.minValue = 1;
                sliderMultiplier.maxValue = SliderMax;
                sliderMultiplier.wholeNumbers = true;
                sliderMultiplier.onValueChanged.AddListener(new UnityAction<float>(OnSliderChange));
                tipSliderMultiplier = tmp.AddComponent<UITooltip>();
                tipSliderMultiplier.Title = "Energy Cap Multiplier".Translate();
                tipSliderMultiplier.Text = "Multiplies the ray receiver's energy cap by the given amount.".Translate();

                initialized = true;

                RefreshUI();
            }
            catch
            {
                Log.LogError("UI component initial fail!");
            }
        }
    }

    public static void OnDestory()
    {
        GameObject.Destroy(group);
        tipSliderMultiplier?.OnDestory();
        initialized = false;
    }

    public static void RefreshUI()
    {
        eventLock = true;
        if (initialized)
        {
            inputMultiplier.text = Multiplier.ToString();
            if (Multiplier <= 10)
            {
                sliderMultiplier.value = Multiplier;
            }
            else
            {
                sliderMultiplier.value = 10 + (Multiplier - 10) / 10;
            }
        }
        eventLock = false;
    }

    public static void OnSliderChange(float val)
    {
        if (!eventLock)
        {
            val = Mathf.Round(val / 1f) * 1f;
            sliderMultiplier.value = val;
            if(val <= 10)
            {
                Multiplier = (int)val;
            }
            else
            {
                Multiplier = 10 + (int)(val - 10) * 10;
            }

            Plugin.EnergyCapMultiplier.Value = Multiplier;
            
            RefreshUI();
        }
    }

    public static void OnInputValueEnd(string val)
    {
        if (!eventLock)
        {
            if (int.TryParse(val, out int value) && value >= 1)
            {
                Multiplier = (int)value;
                Plugin.EnergyCapMultiplier.Value = Multiplier;
            }
            RefreshUI();
        }
    }

}
