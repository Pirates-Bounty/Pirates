using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class UI{
    public static GameObject CreateButton(string name, string text, Font font,
    Color fontColor, int fontSize, Transform parent, Sprite sprite, Sprite highlightedSprite,
    Vector3 position, Vector2 minAnchor, Vector2 maxAnchor, UnityEngine.Events.UnityAction method) {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.parent = parent;
        buttonGO.AddComponent<RectTransform>();
        Image image = buttonGO.AddComponent<Image>();
        image.sprite = sprite;
        Button b = buttonGO.AddComponent<Button>();
        b.onClick.AddListener(method);
        b.transition = Selectable.Transition.SpriteSwap;
        SpriteState state = new SpriteState();
        state.highlightedSprite = highlightedSprite;
        b.spriteState = state;
        RectTransform rectTransform = buttonGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = position;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        GameObject buttonText = new GameObject("Text");
        buttonText.transform.parent = buttonGO.transform;
        RectTransform textRectTransform = buttonText.AddComponent<RectTransform>();
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.anchorMin = Vector3.zero;
        textRectTransform.anchorMax = Vector3.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        textRectTransform.localScale = Vector3.one;
        textRectTransform.localRotation = Quaternion.identity;
        Text t = buttonText.AddComponent<Text>();
        if (text != "") {
            t.text = text;
        }
        t.alignment = TextAnchor.MiddleCenter;
        t.font = font;
        t.color = fontColor;
        t.fontSize = fontSize;

        return buttonGO;
    }

    public static GameObject CreateText(string name, string text, Font font, Color fontColor, int fontSize, Transform parent, Vector3 position, Vector2 minAnchor, Vector2 maxAnchor, TextAnchor alignment, bool bestFit) {
        GameObject textGO = new GameObject(name);
        textGO.transform.parent = parent;
        textGO.AddComponent<RectTransform>();
        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = position;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;


        Text t = textGO.AddComponent<Text>();
        if (text != "") {
            t.text = text;
        }
        t.alignment = alignment;
        t.font = font;
        t.color = fontColor;
        t.fontSize = fontSize;
        t.resizeTextForBestFit = bestFit;

        return textGO;
    }
    public static GameObject CreatePanel(string name, Sprite sprite, Color color, Transform parent, Vector3 position, Vector2 minAnchor, Vector2 maxAnchor) {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.parent = parent;
        panelGO.AddComponent<RectTransform>();
        RectTransform rectTransform = panelGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = position;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;


        Image image = panelGO.AddComponent<Image>();
        image.color = color;
        if (sprite) {
            image.sprite = sprite;
            
        }

        return panelGO;
    }

    public static GameObject CreateInput(string name, string text, string placeholder, Font font, Color fontColor, int fontSize, Transform parent, Sprite sprite, Vector3 position, Vector2 minAnchor, Vector2 maxAnchor, UnityEngine.Events.UnityAction<string> method) {
        GameObject inputGO = new GameObject(name);
        inputGO.transform.parent = parent;
        inputGO.AddComponent<RectTransform>();
        Image image = inputGO.AddComponent<Image>();
        image.sprite = sprite;
        InputField i = inputGO.AddComponent<InputField>();
        i.onEndEdit.AddListener(method);
        i.onValueChanged.AddListener(method);

        RectTransform rectTransform = inputGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = position;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        GameObject inputText = new GameObject("Text");
        inputText.transform.parent = inputGO.transform;
        RectTransform textRectTransform = inputText.AddComponent<RectTransform>();
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.anchorMin = Vector3.zero;
        textRectTransform.anchorMax = Vector3.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        textRectTransform.localRotation = Quaternion.identity;

        Text t = inputText.AddComponent<Text>();
        if (text != "") {
            t.text = text;
        }
        t.alignment = TextAnchor.MiddleCenter;
        t.font = font;
        t.color = fontColor;
        t.fontSize = fontSize;

        GameObject placeholderText = new GameObject("Placeholder");
        placeholderText.transform.parent = inputGO.transform;
        RectTransform placeholderTextRectTransform = placeholderText.AddComponent<RectTransform>();
        placeholderTextRectTransform.localPosition = Vector3.zero;
        placeholderTextRectTransform.anchorMin = Vector3.zero;
        placeholderTextRectTransform.anchorMax = Vector3.one;
        placeholderTextRectTransform.offsetMin = Vector2.zero;
        placeholderTextRectTransform.offsetMax = Vector2.zero;
        placeholderTextRectTransform.localScale = Vector3.one;
        Text pt = placeholderText.AddComponent<Text>();
        if (placeholder != "") {
            pt.text = placeholder;
        }
        pt.alignment = TextAnchor.MiddleCenter;
        pt.font = font;
        pt.color = fontColor;
        pt.fontSize = fontSize;

        i.textComponent = t;
        i.placeholder = pt;
        return inputGO;
    }

    public static GameObject CreateYesNoDialog(string name, string text, Font font, Color fontColor, int fontSize, Sprite sprite, Sprite buttonSprite, Sprite highlightedButtonSprite, Color color, Transform parent, Vector3 position, Vector2 minAnchor, Vector2 maxAnchor, UnityEngine.Events.UnityAction yesAction) {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.parent = parent;
        panelGO.AddComponent<RectTransform>();
        RectTransform rectTransform = panelGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = position;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        Image image = panelGO.AddComponent<Image>();
        image.color = color;
        if (sprite) {
            image.sprite = sprite;
        }

        CreateText("Text", text, font, fontColor, fontSize * 2, panelGO.transform, Vector3.zero, new Vector2(0.25f, 0.5f), new Vector2(0.75f, 0.9f), TextAnchor.MiddleCenter, true);
        CreateButton("Yes Button", "Yes", font, fontColor, fontSize, panelGO.transform, buttonSprite, highlightedButtonSprite, Vector3.zero, new Vector2(0.1f, 0.1f), new Vector2(0.4f, 0.4f), yesAction);
        CreateButton("No Button", "No", font, fontColor, fontSize, panelGO.transform, buttonSprite, highlightedButtonSprite, Vector3.zero, new Vector2(0.6f, 0.1f), new Vector2(0.9f, 0.4f), delegate { GameObject.Destroy(panelGO); });
        return panelGO;
    }

    //function not fully implemented, only the knob appears, the bar is still missing
    public static GameObject CreateSlider(string name, Transform parent, Sprite sliderImage, Vector2 minAnchor, Vector2 maxAnchor, UnityEngine.Events.UnityAction<float> method)
    {
        Debug.LogWarning("CreateSlider function not fully implemented, only the knob appears, the bar is still missing");
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.parent = parent;

        RectTransform rectTransform = sliderGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
        rectTransform.position = Vector3.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;


        Slider sliderComp = sliderGO.AddComponent<Slider>();
        sliderComp.value = 1;
        sliderComp.onValueChanged.AddListener(method);
        //sliderComp.targetGraphic = sliderImage;
        GameObject handleGO = new GameObject();
        handleGO.transform.parent = sliderGO.transform;

        RectTransform tempRT1 = new RectTransform();
        
        //sliderComp.fillRect = tempRT1;
        
        Image tempImg = handleGO.AddComponent<Image>();
        tempImg.sprite = sliderImage;
        sliderComp.targetGraphic = tempImg;

        if (!sliderImage)
            Debug.Log("RAWR NOT FOUND");
        else
            Debug.Log("RAWR FOUND");

        return sliderGO;
    }
}

