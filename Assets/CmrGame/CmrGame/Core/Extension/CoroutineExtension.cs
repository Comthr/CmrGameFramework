using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public static class CoroutineExtension
    {
        public static IEnumerator WhenAll(this MonoBehaviour mono, params IEnumerator[] coroutines)
        {
            List<bool> doneFlags = new List<bool>();

            for (int i = 0; i < coroutines.Length; i++)
            {
                doneFlags.Add(false);
            }

            for (int i = 0; i < coroutines.Length; i++)
            {
                int index = i;
                mono.StartCoroutine(WrapCoroutine(coroutines[i], () => doneFlags[index] = true));
            }

            while (doneFlags.Exists(done => done == false))
                yield return null;
        }
        private static IEnumerator WrapCoroutine(IEnumerator coroutine, System.Action onComplete)
        {
            yield return coroutine;
            onComplete?.Invoke();
        }
    }
}

