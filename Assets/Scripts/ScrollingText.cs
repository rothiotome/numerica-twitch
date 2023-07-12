using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private RectTransform textRectTransform;
    private Vector3 startPosition;

    [SerializeField]private float scrollPosition = 0;

    [SerializeField]private float width;

    [SerializeField] private float scrollSpeed = 10;
    
    void Awake()
    {
        TryGetComponent(out tmp);
        TryGetComponent(out textRectTransform);
    }

    private void Start()
    {
        width = tmp.preferredWidth;
        startPosition = textRectTransform.position;
    }

    void Update()
    {
        textRectTransform.position = new Vector3(-scrollPosition % width, startPosition.y, startPosition.z);
        scrollPosition += scrollSpeed * 20 * Time.deltaTime;
        if (textRectTransform.position.x + textRectTransform.rect.width < 0) scrollPosition = 0;

    }
}
