using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AssetDisplay : MonoBehaviour
{
    public static AssetDisplay Instance;

    private readonly Vector2 maxSpriteSize = new Vector2(1200f, 800f);
    private readonly Vector2 maxVideoSize = new Vector2(1280f, 675f);

    [SerializeField] private Image image;
    [SerializeField] private RectTransform imageRectTransform;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;

    public void DisplaySprite(Sprite sprite)
    {
        if (sprite == image.sprite && image.gameObject.activeInHierarchy)
        {
            image.gameObject.SetActive(false);
            return;
        }

        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);

        image.sprite = sprite;
        image.SetNativeSize();
        imageRectTransform.sizeDelta = ConstrainSizeProportionally(imageRectTransform.sizeDelta, maxSpriteSize);
        image.gameObject.SetActive(true);
    }

    public void DisplayVideo(string url)
    {
        if (url == videoPlayer.url && videoPlayer.gameObject.activeInHierarchy)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
            return;
        }

        image.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(true);

        videoPlayer.url = url;
        videoPlayer.prepareCompleted += VideoPrepared;
        videoPlayer.Prepare();
    }

    private void VideoPrepared(VideoPlayer videoPlayer)
    {
        var texture = videoPlayer.texture;
        var size = new Vector2(texture.width, texture.height);
        var renderTexture = new RenderTexture((int)size.x, (int)size.y, 24);
        rawImage.texture = renderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.Play();
    }

    private void Awake()
    {
        Instance = this;
    }

    private Vector2 ConstrainSizeProportionally(Vector2 startingSize, Vector2 constrainingSize)
    {
        var scaleFactorX = 1f;
        var scaleFactorY = 1f;

        if (startingSize.x > constrainingSize.x)
            scaleFactorX = constrainingSize.x / startingSize.x;

        if (startingSize.y > constrainingSize.y)
            scaleFactorY = constrainingSize.y / startingSize.y;

        float scaleFactor = scaleFactorX < scaleFactorY ? scaleFactorX : scaleFactorY;
        return startingSize * scaleFactor;
    }
}
