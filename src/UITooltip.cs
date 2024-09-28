using UnityEngine;
using UnityEngine.EventSystems;

namespace MegaRayReceiver;

public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Title = null;
    public string Text = null;
    UIButtonTip tip = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tip = UIButtonTip.Create(true, Title, Text, 3, new Vector2(0, 20), 180, gameObject.transform.parent, "", "");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tip != null)
            Destroy(tip.gameObject);
        tip = null;
    }

    public void OnDisable()
    {
        if (tip != null)
            Destroy(tip.gameObject);
        tip = null;
    }

    public void OnDestory()
    {
        if (tip != null)
            Destroy(tip.gameObject);
        tip = null;
    }
}