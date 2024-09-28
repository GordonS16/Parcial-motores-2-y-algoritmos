using UnityEngine;

public class Hover : Singleton<Hover>
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer rangeSpriteRenderer;

    public bool IsVisible { get; private set; }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    public void Activate(Sprite sprite)
    {
        this.spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
        rangeSpriteRenderer.enabled = true;
        IsVisible = true;
    }

    public void Deactivate()
    {
        spriteRenderer.enabled = false;
        rangeSpriteRenderer.enabled = false;
        GameManager.Instance.ClickedBtn = null;
        IsVisible = false;
    }
}
