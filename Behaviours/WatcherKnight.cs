using System.Collections;
using UnityEngine;

namespace CustomTrial.Behaviours
{
    [MatchNameAttribue("Black Knight 1")]
    public class WatcherKnight : MonoBehaviour
    {
        private PlayMakerFSM _knight;

        private void Awake()
        {
            _knight = gameObject.LocateMyFSM("Black Knight");
        }

        private IEnumerator Start()
        {
            _knight.SetState("Init");

            GetComponent<Rigidbody2D>().isKinematic = false;

            yield return new WaitUntil(() => _knight.ActiveStateName == "Rest");
            Modding.Logger.LogDebug("Wake Watcher Knight");

            _knight.SetState("Roar End");
        }
    }
}