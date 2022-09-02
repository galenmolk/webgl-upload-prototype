using System;
using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Ebla.API
{
    public class ApiUtils : MonoBehaviour
    {
        public const string GALLERY_ROUTE = BASE_URL + "/gallery";
        
        private const string BASE_URL = "https://shielded-hollows-53494.herokuapp.com";
        private const string ACCEPT_HEADER_KEY = "Accept";
        private const string CONTENT_TYPE_HEADER_KEY = "Content-Type";
        private const string JSON_HEADER_VALUE = "application/json";
                
        public static IEnumerator DownloadConfigs(string route, Action<Gallery> callback)
        {
            using UnityWebRequest request = NewGetRequest(route);
            SetDefaultHeaders(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"DownloadConfigs Error: {request.error}");
                yield break;
            }

            Debug.Log(request.downloadHandler.text);
            var configs = JsonUtility.FromJson<Gallery>(request.downloadHandler.text);
            if (configs != null)
            {
                callback?.Invoke(configs);
            }
        }
        
        public static void CreateConfig(string route, UploadData baseConfig)
        {
            RequestManager.Instance.AddToQueue(CreateConfigCoroutine(route, baseConfig));
        }

        public static void UpdateConfig(string route, UploadData baseConfig)
        {
            RequestManager.Instance.AddToQueue(UpdateConfigCoroutine(route, baseConfig));
        }

        public static void DeleteConfig(string route, UploadData baseConfig)
        {
            RequestManager.Instance.AddToQueue(DeleteConfigCoroutine(route, baseConfig));
        }

        private static IEnumerator CreateConfigCoroutine(string route, UploadData baseConfig)
        {
            using UnityWebRequest request = NewPostRequest(route, baseConfig);

            SetDefaultHeaders(request);
            SetAcceptHeader(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"PostConfig Error: {request.error}");
                yield break;
            }

            Debug.Log($"PostConfig Success: {request.downloadHandler.text} {request.responseCode}");
        }

        private static IEnumerator UpdateConfigCoroutine(string route, UploadData baseConfig)
        {
            route = IdentifyRoute(route, baseConfig);

            using UnityWebRequest request = NewPutRequest(route, baseConfig);
            
            SetDefaultHeaders(request);
            SetAcceptHeader(request);
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"UpdateConfig Error: {request.error}");
                yield break;
            }

            Debug.Log($"UpdateConfig Success: {request.downloadHandler.text} {request.responseCode}");
        }

        private static IEnumerator DeleteConfigCoroutine(string route, UploadData baseConfig)
        {
            route = IdentifyRoute(route, baseConfig);

            using UnityWebRequest request = NewDeleteRequest(route);
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"DeleteConfig Error: {request.error}");
                yield break;
            }

            Debug.Log($"DeleteConfig Success: {request.downloadHandler.text} {request.responseCode}");
        }

        private static void SetDefaultHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader(CONTENT_TYPE_HEADER_KEY, JSON_HEADER_VALUE);
            request.SetRequestHeader("Access-Control-Allow-Headers", "Origin, X-Requested, Content-Type, Accept Authorization");
        }

        private static void SetAcceptHeader(UnityWebRequest request)
        {
            request.SetRequestHeader(ACCEPT_HEADER_KEY, JSON_HEADER_VALUE);
        }

        private static string IdentifyRoute(string route, UploadData baseConfig)
        {
            return $"{route}/{baseConfig.Id}";
        }
        
        private static UnityWebRequest NewPostRequest(string route, UploadData baseConfig)
        {
            return new UnityWebRequest(
                route, 
                UnityWebRequest.kHttpVerbPOST,
                new DownloadHandlerBuffer(),
                GetUploadHandlerForConfig(baseConfig));
        }
        
        private static UnityWebRequest NewGetRequest(string route)
        {
            return new UnityWebRequest(
                route,
                UnityWebRequest.kHttpVerbGET,
                new DownloadHandlerBuffer(),
                null);
        }

        private static UnityWebRequest NewPutRequest(string route, UploadData baseConfig)
        {
            return new UnityWebRequest(
                route, 
                UnityWebRequest.kHttpVerbPUT,
                new DownloadHandlerBuffer(),
                GetUploadHandlerForConfig(baseConfig));
        }

        private static UnityWebRequest NewDeleteRequest(string route)
        {
            return new UnityWebRequest(
                route, 
                UnityWebRequest.kHttpVerbDELETE, 
                new DownloadHandlerBuffer(), 
                null);
        }

        private static UploadHandlerRaw GetUploadHandlerForConfig(UploadData baseConfig)
        {
            string json = JsonUtility.ToJson(baseConfig);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return new UploadHandlerRaw(bytes);
        }
    }
}
