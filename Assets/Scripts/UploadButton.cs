using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AssetUploadType
{
    Image,
    Audio,
    Video
}

public class Gallery
{
    public List<UploadData> UploadDatas;
}

public class UploadData
{
    public string Id;
    public string Filename;
    public string Url;
}

public class ProgressData
{
    public long loadedKilobytes;
    public long totalKilobytes;
}

public class UploadButton : MonoBehaviour, IPointerDownHandler
{
    private const string IMAGE_UPLOAD_URL = "https://api.cloudinary.com/v1_1/cloudkeeper/image/upload";
    private const string AUDIO_VIDEO_UPLOAD_URL = "https://api.cloudinary.com/v1_1/cloudkeeper/video/upload";
    
    private const string IMAGE_FILTER = "image/*";
    private const string AUDIO_FILTER = "audio/*,.aifc";
    private const string VIDEO_FILTER = "video/*";
    
    // Free plan values:
    // 10 Megabytes.
    private const long MAX_IMAGE_BYTES = 10000000;
    // 100 Megabytes.
    private const long MAX_AUDIO_VIDEO_BYTES = 100000000;
    
    // Values for Hellosaurus Cloudinary Server:
    // 20 Megabytes.
    // private const long MAX_IMAGE_BYTES = 20000000;
    
    // 2 Gigabytes.
    // private const long MAX_AUDIO_VIDEO_BYTES = 2147483648;
    
    [SerializeField] private AssetUploadType assetUploadType;
    
    private string uploadUrl;
    private string assetSelectionFilter;
    private long maxBytes;

    [DllImport("__Internal")] private static extern void UploadAsset(
        string gameObjectName, 
        string uploadUrl,
        string assetFilter,
        string maxBytes,
        bool isMultiSelect);
    
    // Method called by WebGLAssetUpload.jslib plugin.
    public void AssetUploaded(string responseJson)
    {
        StatusBar.Instance.ProgressCompleted();

        var upload = JsonUtility.FromJson<UploadData>(responseJson);

        switch (assetUploadType)
        {
            case AssetUploadType.Image:
                AssetGallery.Instance.AddSprite(upload);
                break;
            case AssetUploadType.Audio:
                StartCoroutine(AssetGallery.Instance.AddAudioClip(upload));
                break;
            case AssetUploadType.Video:
                AssetGallery.Instance.AddVideoClip(upload);
                break;
        }
    }
    
    // Method called by WebGLAssetUpload.jslib plugin.
    public void UploadProgressChanged(string progress)
    {
        StatusBar.Instance.ProgressChanged(progress);
    }

    // Method called by WebGLAssetUpload.jslib plugin.
    public void UploadFailed(string error)
    {
        StatusBar.Instance.DisplayAlert(error);
        StatusBar.Instance.ProgressCompleted();
    }

    // Method called by WebGLAssetUpload.jslib plugin.
    public void FileTooBig(string fileName)
    {
        var message = $"{fileName} is too large. File must be {GetFileSizeDescriptor(maxBytes)} or less.";
        StatusBar.Instance.DisplayAlert(message);
    }
    
    // A PointerDown event must be used (as opposed to PointerClick or Button.onClick)
    // to supersede browser security and intercept the input event.
    public void OnPointerDown(PointerEventData eventData)
    {
#if !UNITY_WEBGL || UNITY_EDITOR
    return;
#endif
        UploadAsset(gameObject.name, uploadUrl, assetSelectionFilter, maxBytes.ToString(), true);
    }

    private string GetFileSizeDescriptor(long bytes)
    {
        var megabytes = bytes / 1000000;
        if (megabytes < 1000)
            return $"{Math.Round((float)megabytes, 2)} MB";

        var gigabytes = bytes / 1000000000;
        return $"{Math.Round((float)gigabytes, 2)} GB";
    }
    
    private void Awake()
    {
        SetUploadParameters();
        SetAssetFilter();
    }

    private void SetUploadParameters()
    {
        uploadUrl = assetUploadType == AssetUploadType.Image ? IMAGE_UPLOAD_URL : AUDIO_VIDEO_UPLOAD_URL;
        maxBytes = assetUploadType == AssetUploadType.Image ? MAX_IMAGE_BYTES : MAX_AUDIO_VIDEO_BYTES;
    }

    private void SetAssetFilter()
    {
        assetSelectionFilter = assetUploadType switch
        {
            AssetUploadType.Image => IMAGE_FILTER,
            AssetUploadType.Audio => AUDIO_FILTER,
            AssetUploadType.Video => VIDEO_FILTER,
            _ => string.Empty
        };
    }
}
