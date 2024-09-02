using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRise : MonoBehaviour
{
    private RectTransform rt;
    private TMP_Text text;
    private float initialY;
    private Color initialColor;
    [SerializeField] private int riseDistance;
    [SerializeField] private float riseSeconds;
    [SerializeField] private bool destroy;
    [SerializeField] private bool fade;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
        initialY = rt.anchoredPosition.y;
        initialColor = text.color;
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        float t = 0;
        while (t < 1)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(initialY, initialY + riseDistance, t));
            if (fade && (t > 0.5f))
            {
                text.color = initialColor - new Color(0, 0, 0, Mathf.Lerp(0, 1, (t - 0.5f) * 2));
            }
            t += Time.deltaTime / riseSeconds;
            yield return null;
        }
        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
