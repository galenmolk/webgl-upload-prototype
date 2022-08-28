using System;
using System.Collections;
using Ebla.API;
using MolkExtras;
using UnityEngine;
using UnityEngine.Networking;

public class AssetGallery : Singleton<AssetGallery>
{
    [SerializeField] private GameObject container;
    [SerializeField] private AssetSlot assetSlotPrefab;
    
    public void AddSprite(UploadData upload)
    {
        ApiUtils.CreateConfig(ApiUtils.GALLERY_ROUTE, upload);
        StartCoroutine(CreateSpriteSlot(upload));
    }

    public IEnumerator AddAudioClip(UploadData upload)
    {
        yield return StartCoroutine(DownloadAudioClip(upload.Url, audioClip =>
        {
            var assetSlot = GetSlot(upload);
            assetSlot.CreateAudioClipSlot(upload.Filename, audioClip);
        }));
    }

    public void AddVideoClip(UploadData upload)
    {
        var assetSlot = GetSlot(upload);
        assetSlot.CreateVideoClipSlot(upload.Filename, upload.Url);
    }

    private IEnumerator CreateSpriteSlot(UploadData uploadData)
    {
        yield return StartCoroutine(DownloadSprite(uploadData.Url, sprite =>
        {
            var assetSlot = GetSlot(uploadData);
            assetSlot.CreateSpriteSlot(uploadData.Filename, sprite);
        }));
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

    private AssetSlot GetSlot(UploadData uploadData)
    {
        AssetSlot slot = Instantiate(assetSlotPrefab, container.transform);
        slot.Configure(uploadData);
        slot.OnDelete += HandleDelete;
        return slot;
    }

    private static void HandleDelete(UploadData uploadData)
    {
        ApiUtils.DeleteConfig(ApiUtils.GALLERY_ROUTE, uploadData);
    }

    private void Start()
    {
        StartCoroutine(ApiUtils.DownloadConfigs(ApiUtils.GALLERY_ROUTE, gallery =>
        {
            if (gallery != null)
            {
                Debug.Log(JsonUtility.ToJson(gallery));
            }
            return;
            foreach (var uploadData in gallery.UploadDatas)
            {
                StartCoroutine(CreateSpriteSlot(uploadData));
            }
        }));
    }
}
