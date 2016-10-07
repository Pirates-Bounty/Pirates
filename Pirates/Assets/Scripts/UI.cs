using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class UI {
    public static GameObject CreateButton(string name, string text, Font font, Color fontColor, int fontSize, Transform parent, Sprite sprite, Sprite highlightedSprite, Vector3 position, Vector2 minAnchor, Vector2 maxAnchor, UnityEngine.Events.UnityAction method) {
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

        GameObject buttonText = new GameObject("Text");
        buttonText.transform.parent = buttonGO.transform;
        RectTransform textRectTransform = buttonText.AddComponent<RectTransform>();
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.anchorMin = Vector3.zero;
        textRectTransform.anchorMax = Vector3.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        Text t = buttonText.AddComponent<Text>();
        if (text != "") {
            t.text = text;
        }
        t.alignment = TextAnchor.MiddleCenter;
        t.font = font;
        t.color = fontColor;
        t.fontSize = fontSize;
        t.resizeTextForBestFit = true;

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

        GameObject inputText = new GameObject("Text");
        inputText.transform.parent = inputGO.transform;
        RectTransform textRectTransform = inputText.AddComponent<RectTransform>();
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.anchorMin = Vector3.zero;
        textRectTransform.anchorMax = Vector3.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
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
}

