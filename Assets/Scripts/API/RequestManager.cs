using System.Collections;
using System.Collections.Generic;
using MolkExtras;
using UnityEngine;

namespace Ebla.API
{
    public class RequestManager : Singleton<RequestManager>
    {
        private readonly Queue<IEnumerator> coroutines = new Queue<IEnumerator>();

        private Coroutine activeRequest;
        
        public void AddToQueue(IEnumerator coroutine)
        {
            EnqueueRequest(StartLinkedRequest(coroutine));
            
            if (activeRequest == null)
            {
                DequeueRequest();
            }
        }
        
        private void EnqueueRequest(IEnumerator coroutine)
        {
            coroutines.Enqueue(coroutine);
        }

        private void DequeueRequest()
        {
            if (coroutines.Count == 0)
            {
                activeRequest = null;
                return;
            }

            IEnumerator coroutine = coroutines.Dequeue();
            activeRequest = StartCoroutine(coroutine);
        }

        private IEnumerator StartLinkedRequest(IEnumerator coroutine)
        {
            yield return StartCoroutine(this.ExecuteAfterCoroutine(coroutine, DequeueRequest));
        }
    }
}
