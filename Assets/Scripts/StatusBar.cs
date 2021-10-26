using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public static StatusBar Instance;

    private readonly Dictionary<long, long> inProgressUploads = new Dictionary<long, long>();

    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text alertText;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private int inProgressUploadCount = 0;

    public void DisplayAlert(string message)
    {
        alertText.text = message;
        alertText.gameObject.SetActive(true);
        this.Invoke(() => alertText.gameObject.SetActive(false), 5f);
    }

    public void ProgressChanged(string progressJson)
    {
        var progressData = JsonUtility.FromJson<ProgressData>(progressJson);

        if (inProgressUploads.ContainsKey(progressData.totalKilobytes))
        {
            inProgressUploads[progressData.totalKilobytes] = progressData.loadedKilobytes;
        }
        else
        {
            inProgressUploads.Add(progressData.totalKilobytes, progressData.loadedKilobytes);
            inProgressUploadCount++;
        }

        UpdateStatusBar();
    }

    public void ProgressCompleted()
    {
        inProgressUploadCount--;

        if (inProgressUploadCount <= 0)
        {
            ResetProgress();
            return;
        }

        UpdateStatusBar();
    }

    private void Awake()
    {
        Instance = this;
        ResetProgress();
    }

    private void UpdateStatusBar()
    {
        if (canvasGroup.alpha < 1f)
            canvasGroup.alpha = 1f;
        
        var loadedMegabytes = Math.Round(KilobytesToMegabytes(GetLoadedKilobytes()), 2);
        var totalMegabytes = Math.Round(KilobytesToMegabytes(GetTotalKilobytes()), 2);
        var truePercent = (float)Math.Floor(loadedMegabytes * 100f / totalMegabytes);
        
        // There is a delay between when the data finishes sending and the server returns a response.
        var displayedPercent = Mathf.Lerp(0, 99, truePercent);
        
        var assetKeyword = inProgressUploadCount > 1 ? "Assets" : "Asset";
        
        progressText.text = $"Uploading {inProgressUploadCount} {assetKeyword}\n{loadedMegabytes} mb of {totalMegabytes} mb | {displayedPercent}% ";
        progressBarImage.fillAmount = (float)truePercent * 0.01f;
    }
    
    private double GetLoadedKilobytes()
    {
        double loadedKilobytes = 0;

        foreach (var upload in inProgressUploads)
            loadedKilobytes += upload.Value;

        return loadedKilobytes;
    }
    
    private double GetTotalKilobytes()
    {
        double totalKilobytes = 0;

        foreach (var upload in inProgressUploads)
            totalKilobytes += upload.Key;

        return totalKilobytes;
    }

    private double KilobytesToMegabytes(double kilobytes)
    {
        return kilobytes * 0.000001;
    }
    
    private void ResetProgress()
    {
        canvasGroup.alpha = 0f;
        alertText.gameObject.SetActive(false);
        progressText.text = string.Empty;
        progressBarImage.fillAmount = 0f;
    }
}
