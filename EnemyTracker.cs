using System.Linq;
using CustomTrial.Behaviours;
using HutongGames.PlayMaker;
using UnityEngine;
using System;

namespace CustomTrial
{
    public class EnemyTracker : MonoBehaviour
    {
        private HealthManager _hm;
        private PlayMakerFSM _fsm;
        private float timer;

        private void Awake()
        {
            timer = 0;
            _hm = GetComponent<HealthManager>();
            _fsm = GetComponent<PlayMakerFSM>();
            _hm.OnDeath += () => Destroy(gameObject, 3);
        }
        private void fix_behaviours()
        {
            string goName = gameObject.name;
            Type behaviour = Utils.ReflectionHelper.MatchBehaviourWithName(goName);
            if (behaviour != null)
            {
                gameObject.AddComponent(behaviour);
            }
            else if (goName.Contains("Plant Trap"))
            {
                gameObject.LocateMyFSM("Plant Trap Control").SetState("Init");
            }
            else if (goName.Contains("Moss Knight Fat"))
            {
                Destroy(gameObject.LocateMyFSM("FSM"));
            }
            else if (goName.Contains("Bee Hatchling Ambient"))
            {
                gameObject.LocateMyFSM("Bee").SetState("Pause");
            }
            else if (goName.Contains("Flamebearer Large"))
            {
                gameObject.AddComponent<Grimmkin>().grimmchildLevel = 3;
            }
            else if (goName.Contains("Flamebearer Med"))
            {
                gameObject.AddComponent<Grimmkin>().grimmchildLevel = 2;
            }
            else if (goName.Contains("Flamebearer Small"))
            {
                gameObject.AddComponent<Grimmkin>().grimmchildLevel = 1;
            }
            else
            {
                foreach (FsmState state in _fsm.FsmStates)
                {
                    if (state.Name == "Init")
                    {
                        _fsm.SetState("Init");
                        return;
                    }
                    if (state.Name == "Initialise")
                    {
                        _fsm.SetState("Initialise");
                        return;
                    }
                }
            }
        }
        private void Start()
        {
            gameObject.name = gameObject.name.Replace("(clone)", "").Replace("(Clone)", "");
            
            try
            {
                fix_behaviours();
            }
            catch(NullReferenceException e)
            {
                ColosseumManager.EnemyCount--;
            }
        }
    
        private void Update()
        {
            var bottom = ArenaInfo.BottomY - 2f;
            var top = ArenaInfo.TopY + 2f;
            var left = ArenaInfo.LeftX - 2f;
            var right = ArenaInfo.RightX + 2f;

            //check out of arena
            var pos = transform.position;
            if(pos.x<left || pos.x>right || pos.y>top || pos.y<bottom)
            {
                timer += Time.deltaTime;
                if(timer > 3 && !_hm.isDead)
                {
                    Modding.Logger.LogDebug("outside arena, try kill");
                    transform.position = HeroController.instance.transform.position;
                    _hm.Die(0, AttackTypes.Acid, true);
                    Destroy(gameObject, 3);

                    timer = -10f;
                }
            }
        }
    
        private void OnDestroy()
        {
            ColosseumManager.EnemyCount--;
        }
    }
}