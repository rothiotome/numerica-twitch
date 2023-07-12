using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panel;
    private TextMeshProUGUI tmp;
    private Color startColor;

    private void Awake()
    {
        TryGetComponent(out tmp);
        startColor = tmp.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmp.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmp.color = startColor;
    }
}
