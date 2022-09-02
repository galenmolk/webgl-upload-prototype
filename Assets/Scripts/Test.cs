using Ebla.API;
using UnityEngine;

namespace WebGallery.Editor
{
    public class Test : MonoBehaviour
    {
        [ContextMenu("Test")]
        public void GetConfigs()
        {
            StartCoroutine(ApiUtils.DownloadConfigs(ApiUtils.GALLERY_ROUTE, gallery =>
            {
                Debug.Log(JsonUtility.ToJson(gallery));
            }));
        }
    }
}
