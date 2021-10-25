﻿using System.Collections;
using UnityEngine;

namespace CustomTrial.Behaviours
{
    [MatchNameAttribue("Mantis Flyer Child")]
    public class MantisYouth : MonoBehaviour
    {
        private PlayMakerFSM _flyer;

        private void Awake()
        {
            _flyer = gameObject.LocateMyFSM("Mantis Flyer");
        }

        private IEnumerator Start()
        {
            _flyer.Fsm.GetFsmBool("Start Idle").Value = true;
            
            _flyer.SetState("Init");

            yield return null;
        }
    }
}