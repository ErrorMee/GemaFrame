using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using XLua;

[LuaCallCSharp]
public class EventTriggerListener : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IUpdateSelectedHandler, ISelectHandler
{
    public delegate void VoidDelegate(GameObject go);

    private VoidDelegate m_onClick;
    public VoidDelegate onClick
    {
        get { return m_onClick; }
        set
        {
            if (m_onClick != null)
            {
                m_onClick = null;
            }
            m_onClick = value;
        }
    }

    private VoidDelegate m_onDown;
    public VoidDelegate onDown
    {
        get { return m_onDown; }
        set
        {
            if (m_onDown != null)
            {
                m_onDown = null;
            }
            m_onDown = value;
        }
    }

    private VoidDelegate m_onEnter;
    public VoidDelegate onEnter
    {
        get { return m_onEnter; }
        set
        {
            if (m_onEnter != null)
            {
                m_onEnter = null;
            }
            m_onEnter = value;
        }
    }

    private VoidDelegate m_onExit;
    public VoidDelegate onExit
    {
        get { return m_onExit; }
        set
        {
            if (m_onExit != null)
            {
                m_onExit = null;
            }
            m_onExit = value;
        }
    }

    private VoidDelegate m_onUp;
    public VoidDelegate onUp
    {
        get { return m_onUp; }
        set
        {
            if (m_onUp != null)
            {
                m_onUp = null;
            }
            m_onUp = value;
        }    
    }

    private VoidDelegate m_onSelect;
    public VoidDelegate onSelect
    {
        get { return m_onSelect; }
        set
        {
            if (m_onSelect != null)
            {
                m_onSelect = null;
            }
            m_onSelect = value;
        }
    }

    private VoidDelegate m_onUpdateSelect;
    public VoidDelegate onUpdateSelect
    {
        get { return m_onUpdateSelect; }
        set
        {
            if (m_onUpdateSelect != null)
            {
                m_onUpdateSelect = null;
            }
            m_onUpdateSelect = value;
        }
    }

    private VoidDelegate m_onLongPress;
    public VoidDelegate onLongPress
    {
        get { return m_onLongPress; }
        set
        {
            if (m_onLongPress != null)
            {
                m_onLongPress = null;
            }
            m_onLongPress = value;
        }
    }

    public delegate void VectorDelegate(GameObject go, PointerEventData pointerData);
    private VectorDelegate m_onClickDetail;
    public VectorDelegate onClickDetail
    {
        get { return m_onClickDetail; }
        set
        {
            if (m_onClickDetail != null)
            {
                m_onClickDetail = null;
            }
            m_onClickDetail = value;
        }
    }

    private VectorDelegate m_onDownDetail;
    public VectorDelegate onDownDetail
    {
        get { return m_onDownDetail; }
        set
        {
            if (m_onDownDetail != null)
            {
                m_onDownDetail = null;
            }
            m_onDownDetail = value;
        }
    }

    private VectorDelegate m_onEnterDetail;
    public VectorDelegate onEnterDetail
    {
        get { return m_onEnterDetail; }
        set
        {
            if (m_onEnterDetail != null)
            {
                m_onEnterDetail = null;
            }
            m_onEnterDetail = value;
        }
    }

    private VectorDelegate m_onExitDetail;
    public VectorDelegate onExitDetail
    {
        get { return m_onExitDetail; }
        set
        {
            if (m_onExitDetail != null)
            {
                m_onExitDetail = null;
            }
            m_onExitDetail = value;
        }
    }

    private VectorDelegate m_onUpDetail;
    public VectorDelegate onUpDetail
    {
        get { return m_onUpDetail; }
        set
        {
            if (m_onUpDetail != null)
            {
                m_onUpDetail = null;
            }
            m_onUpDetail = value;
        }
    }


    private Coroutine _LongClick;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //UIManagerEx.GetInstance().Click(gameObject);
        if (onClick != null && !longPressTriggerd) onClick(gameObject);
        if (onClickDetail != null && !longPressTriggerd) onClickDetail(gameObject, eventData);
        //         if(gameObject.GetComponent<UnityEngine.UI.Button>()!=null)
        //             SoundManager.Instance.PlayUISound(4001, gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        longPressTriggerd = false;
        if (onDown != null)
        {
            onDown(gameObject);
        };
        if (onLongPress != null)
        {
            if (_LongClick != null) StopCoroutine(_LongClick);
            _LongClick = StartCoroutine(LongClick(0.5f));
        }
        if (onDownDetail != null) onDownDetail(gameObject, eventData);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
        if (onEnterDetail != null) onEnterDetail(gameObject, eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
        if (onExitDetail != null) onExitDetail(gameObject, eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
        if (onUpDetail != null) onUpDetail(gameObject, eventData);
        if (onLongPress != null && _LongClick != null)
        {
            StopCoroutine(_LongClick);
            _LongClick = null;
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }
    private bool longPressTriggerd = false;
    private IEnumerator LongClick(float t)
    {
        yield return new WaitForSeconds(t);
        if (onLongPress != null)
        {
            longPressTriggerd = true;
            onLongPress(gameObject);
        }
    }

    protected void OnDestroy()
    {
        onClick = null;
        onDown = null;
        onEnter = null;
        onExit = null;
        onLongPress = null;
        onSelect = null;
        onUp = null;
        onUpdateSelect = null;

        onClickDetail = null;
        onDownDetail = null;
        onEnterDetail = null;
        onExitDetail = null;
        onUpDetail = null;
    }

}