using UnityEngine;

public class Dropdown : MonoBehaviour
{
    private RectTransform container;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        container = transform.Find("container").GetComponent<RectTransform>();
        isOpen = false;
    }

    public void setOpen()
    {
        isOpen = !isOpen;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 12);
        container.localScale = scale;
        container.localPosition = new Vector3(0, (scale.y * 150) + 10, 0);
    }
}
