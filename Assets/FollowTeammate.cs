using UnityEngine;

public class FollowTeammate : MonoBehaviour
{
    private Camera cam;

    public Transform teammate;
    private RectTransform rTransform;
    private SpriteRenderer icon;

    public Sprite[] icons = new Sprite[8];

    private CanvasGroup canvas;

    private void Start()
    {
        cam = Camera.main;

        rTransform = GetComponent<RectTransform>();
        icon = GetComponent<SpriteRenderer>();
        canvas = GetComponent<CanvasGroup>();

        try
        {
            if (GamePlayerInfo.playerNum % 2 == 0) // Even numbers are humans.
            {
                teammate = GamePlayerInfo.playerTransforms[GamePlayerInfo.playerNum + 1];
                icon.sprite = icons[GamePlayerInfo.playerNum + 1];
            }
            else // Odd numbers are fairies.
            {
                teammate = GamePlayerInfo.playerTransforms[GamePlayerInfo.playerNum - 1];
                icon.sprite = icons[GamePlayerInfo.playerNum - 1];
            }
        }
        catch { }

        if (teammate == null)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (teammate == null)
            return;

        if (cam.WorldToViewportPoint(teammate.position).x > 0 && cam.WorldToViewportPoint(teammate.position).x < 1 &&
            cam.WorldToViewportPoint(teammate.position).y > 0 && cam.WorldToViewportPoint(teammate.position).y < 1)
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, 0, Time.deltaTime * 4f);
        }
        else
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, 1, Time.deltaTime * 4f);
        }

        rTransform.position = new Vector2(Mathf.Clamp(cam.WorldToScreenPoint(teammate.position).x, 24, Screen.width - 24), Mathf.Clamp(cam.WorldToScreenPoint(teammate.position).y, 24, Screen.height - 24));
    }
}
