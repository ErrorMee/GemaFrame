using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleScrollList
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/SimpleScrollList", false, 1)]
    static void CreateSimpleScrollList(MenuCommand menuCommand)
    {
        //Scroll
        GameObject go = new GameObject("SimpleScrollList");
        GameObject select = menuCommand.context as GameObject;
        if (select != null)
        {
            GameObjectUtility.SetParentAndAlign(go, select);
        }

        Undo.RegisterCreatedObjectUndo(go, "Created " + go.name);

        RectTransform rectTrans = go.AddComponent<RectTransform>();
        rectTrans.sizeDelta = new Vector3(250, 100);
        rectTrans.pivot = new Vector2(0, 0.5f);

        ScrollRect scroll = go.AddComponent<ScrollRect>();
        RectMask2D mask = go.AddComponent<RectMask2D>();
        scroll.horizontal = true;
        scroll.vertical = false;
        scroll.viewport = rectTrans;

        //list
        RectTransform list = (new GameObject("List")).AddComponent<RectTransform>();
        GameObjectUtility.SetParentAndAlign(list.gameObject, go);
        list.anchorMin = new Vector2(0f, 0.5f);
        list.anchorMax = new Vector2(0f, 0.5f);
        list.pivot = new Vector2(0f, 0.5f);

        HorizontalLayoutGroup hlg = list.gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 1;
        hlg.childForceExpandHeight = false;
        hlg.childForceExpandWidth = false;

        ContentSizeFitter csf = list.gameObject.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = list;

        //item
        RectTransform item = (new GameObject("item")).AddComponent<RectTransform>();
        item.gameObject.AddComponent<Image>();
        GameObjectUtility.SetParentAndAlign(item.gameObject, list.gameObject);

        Selection.activeObject = item;
    }
#endif

}
