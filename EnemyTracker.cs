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

        private void Awake()
        {
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
                Modding.Logger.LogDebug($"Add Behaviour {behaviour.Name} to {goName}");
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
                WatchDog.Create(gameObject, () => ColosseumManager.EnemyCount--);
                ColosseumManager.EnemyCount++;
            }
            catch
            {
                gameObject?.SetActive(false);
                DestroyImmediate(gameObject);
                return;
            }

        }
        private class WatchDog : MonoBehaviour
        {
            GameObject go;
            Action dead;
            float tick = 0;
            float arena_tick = 0;
            const float max_inactive_time = 5f;
            bool called = false;
            HealthManager _hm;

            const float bottom = ArenaInfo.DefaultBottomY - 2f;
            const float top = ArenaInfo.DefaultTopY + 2f;
            const float left = ArenaInfo.DefaultLeftX - 2f;
            const float right = ArenaInfo.DefaultRightX + 2f;
            public static GameObject Create(GameObject go,Action dead)
            {
                if (go == null || dead == null)
                {
                    return null;
                }

                var watcherGo = new GameObject($"{go.name}_watcher");
                var watcher = watcherGo.AddComponent<WatchDog>();
                watcher.go = go;
                watcher.dead = dead;
                watcher._hm = go.GetComponent<HealthManager>();
                
                return watcherGo;
            }
            void health_ckeck()
            {
                if (go.activeSelf == false || _hm.hp < 0)
                {
                    tick += Time.deltaTime;

                    if (tick > max_inactive_time)
                    {
                        Modding.Logger.LogDebug($"{go?.name} unhealth, try kill");
                        Call();
                    }
                }
                else
                {
                    tick = 0;
                }
            }
            void arena_check()
            {
                var pos = go.transform.position;
                if (pos.x < left || pos.x > right || pos.y > top || pos.y < bottom)
                {
                    arena_tick += Time.deltaTime;
                    if (arena_tick > 5 && !_hm.isDead)
                    {
                        Modding.Logger.LogDebug($"{go?.name} outside arena, try kill");
                        go.transform.position = HeroController.instance.transform.position;
                        _hm.Die(0, AttackTypes.Acid, true);
                        Destroy(go, 3);

                        arena_tick = -10f;
                    }
                }
                else
                {
                    arena_tick = 0;
                }
            }

            void Call()
            {
                if (!called)
                {
                    tick = 0;
                    dead?.Invoke();
                    called = true;
                    Destroy(go, 3);
                }
            }
            void Update()
            {
                try
                {
                    health_ckeck();
                    arena_check();
                }
                catch(NullReferenceException e)
                {
                    Call();
                    Modding.Logger.LogWarn(e.StackTrace);
                    Destroy(gameObject);
                    Destroy(go);
                }
               
            }

            void OnDestroy()
            {
                Call();
            }
        }

    }
}