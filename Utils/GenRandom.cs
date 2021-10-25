using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTrial.Classes;
using UnityEngine;
using Random = UnityEngine.Random;
namespace CustomTrial.Utils
{
    public static class GenRandom
    {
        public static int random_health(string name = null)
        {
            int hitdmg = 21;
            int minhit = 3;
            int maxhit = 10;

            return UnityEngine.Random.Range(hitdmg * minhit, hitdmg * maxhit);
        }
        public static Vector2 random_spawn_position()
        {
            float x, y;

            x = UnityEngine.Random.Range(87f, 118f);
            y = UnityEngine.Random.Range(7f, 15f);

            return new Vector2(x, y);
        }
        public static Enemy RandomEnemy()
        {
            string name;
            int health = random_health();
            Vector2 pos = random_spawn_position();

            int cnt = CustomTrial.EnimiesNameList.Count;
            int idx = UnityEngine.Random.Range(0, cnt );

            name = CustomTrial.EnimiesNameList[idx];

            return new Enemy(name, health, pos);
        }
        public static string RandomMusicLevel()
        {
            string []enum_string = { "SILENT","1", "2" ,"3","4","5","6"};
            return enum_string[UnityEngine.Random.Range(0, enum_string.Length)];
        }
        public static float RandomWallC()
        {
            return Random.Range(0, 0f);
        }
        public static float RandomWallL() => RandomWallC();
        public static float RandomWallR() => RandomWallC();

        public static string RandomClowdAct()
        {
            string[] enum_string = { "Cheer", "Laugh" , "Gasp" };
            return enum_string[Random.Range(0, enum_string.Length)];
        }
        public static Vector2 RandomPlatform() => random_spawn_position();
        public static float RandomSpawnDelay() => Random.Range(0, 2f);
        public static float RandomCooldown() => Random.Range(1, 3f);
        public static Wave RandomWave()
        {
            int enemies_cnt = Random.Range(1, 6);
            int platform_cnt = Random.Range(0, 3);
            List<Enemy> enemies = new();
            List<Vector2> plats = new();
            string crowd = RandomClowdAct();
            string musiclv = RandomMusicLevel();
            float cooldown = RandomCooldown();
            float delay = RandomSpawnDelay();
            float wallc = RandomWallC();
            float walll = RandomWallL();
            float wallr = RandomWallR();
            bool spike = Random.Range(0, 2) == 1 ? true : false;

            for(int i=0;i<enemies_cnt;i++)
            {
                enemies.Add(RandomEnemy());
            }
            for(int i=0;i<platform_cnt;i++)
            {
                plats.Add(RandomPlatform());
            }

            return new(enemies,plats,crowd,musiclv,cooldown,delay,wallc,walll,wallr,spike);
        }
    
        public static GlobalSettings RandomPlay()
        {
            int wave_cnt = Random.Range(5, 30);
            int geo = wave_cnt * 50;

            var setting = new GlobalSettings();
            for(int i=0;i<wave_cnt;i++)
            {
                setting.AddWave(RandomWave());
            }
            setting.SetGeoReward(geo);
            setting.EnableRandom = true;

            Modding.Logger.Log($"Generate random play! waves count:{wave_cnt},reward:{geo}");
            return setting;
        }
        public static GlobalSettings AllPlay()
        {
            var setting = new GlobalSettings();
            int cnt = CustomTrial.EnimiesNameList.Count;
            for(int i=0;i<cnt;i++)
            {
                Wave wave = new();
                var enemy = RandomEnemy();
                enemy.Name = CustomTrial.EnimiesNameList[i];
                wave.Enemies.Add(enemy);
                setting.AddWave(wave);
            }
            setting.SetGeoReward(50);
            setting.EnableRandom = true;
            Modding.Logger.Log($"All Play!!");
            return setting;
        }
    }
}
