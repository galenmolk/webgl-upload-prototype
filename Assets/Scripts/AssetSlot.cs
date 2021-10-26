using TMPro;
using UnityEngine;

public class AssetSlot : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text fileNameText;
    
    private Sprite sprite;
    private string videoUrl;

    public void CreateSpriteSlot(string fileName, Sprite _sprite)
    {
        fileNameText.text = fileName;
        sprite = _sprite;
    }

    public void CreateAudioClipSlot(string fileName, AudioClip _audioClip)
    {
        fileNameText.text = fileName;
        audioSource.clip = _audioClip;
    }

    public void CreateVideoClipSlot(string fileName, string _videoUrl)
    {
        fileNameText.text = fileName;
        videoUrl = _videoUrl;
    }

    public void Select()
    {
        if (sprite != null)
        {
            AssetDisplay.Instance.DisplaySprite(sprite);
            return;
        }

        if (!string.IsNullOrWhiteSpace(videoUrl))
        {
            AssetDisplay.Instance.DisplayVideo(videoUrl);
            return;
        }

        if (audioSource.clip != null)
            SetIsAudioSourcePlaying();
    }

    private void SetIsAudioSourcePlaying()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        else
            audioSource.Play();
    }
}
