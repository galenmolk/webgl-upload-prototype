using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetGallery : MonoBehaviour
{
    public static AssetGallery Instance;

    [SerializeField] private GameObject container;
    [SerializeField] private AssetSlot assetSlotPrefab;
    
    public IEnumerator AddSprite(UploadData upload)
    {
        yield return StartCoroutine(DownloadSprite(upload.url, sprite =>
        {
            var assetSlot = Instantiate(assetSlotPrefab, container.transform);
            assetSlot.CreateSpriteSlot(upload.fileName, sprite);
        }));
    }

    public IEnumerator AddAudioClip(UploadData upload)
    {
        yield return StartCoroutine(DownloadAudioClip(upload.url, audioClip =>
        {
            var assetSlot = Instantiate(assetSlotPrefab, container.transform);
            assetSlot.CreateAudioClipSlot(upload.fileName, audioClip);
        }));
    }

    public void AddVideoClip(UploadData upload)
    {
        var assetSlot = Instantiate(assetSlotPrefab, container.transform);
        assetSlot.CreateVideoClipSlot(upload.fileName, upload.url);
    }

    private IEnumerator DownloadSprite(string url, Action<Sprite> callback)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100F, 0, SpriteMeshType.FullRect);
        callback(sprite);
    }
    
    private IEnumerator DownloadAudioClip(string url, Action<AudioClip> callback)
    {
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
        yield return request.SendWebRequest();
        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
        callback(audioClip);
    }
    
    private void Awake()
    {
        Instance = this;
    }
}
