using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DunGen;
using UnityEngine.SceneManagement;

namespace keijo
{
    public struct TaskList
    {
        public int blueCapCount;
        public int deliveredBlueCaps;
        public int glowBudCount;
        public int deliveredGlowBuds;
        public int crystalCount;
        public int deliveredCrystals;
        public int paleShroomCount;
        public int deliveredPaleShrooms;
    }

    public class GameManager : MonoBehaviour
    {
        public RuntimeDungeon dungeon;
        public int seedGeneratorInt;
        public System.Random levelRandom;
        public Transform playerTransform;

        [Header("Level settings")]
        public int levelSeed;
        public int requiredMaterials = 5;
        public TaskList taskList = new TaskList();
        public int roundNumber = 0;
        public int dayNumber = 0;
        public int quota = 100;
        public int coins = 0;
        public float dungeonMultiplier = 2;

        public List<GameObject> spawnedItems = new List<GameObject>();
        public List<GameObject> enemies = new List<GameObject>();

        [Header("Item values")]
        public int blueCapValue = 4;
        public int glowBudValue = 10;
        public int crystalValue = 18;
        public int paleShroomValue = 8;
        public float taskBonus = 0.5f;

        [Header("Enemies")]
        public GameObject enemyPrefab;
        public int enemyAmountAtStart = 1;

        void Awake()
        {
            DateTime dateTimeCompareTo = new DateTime(2024, 01, 10);
            DateTime dateTimeToday = DateTime.Today;
            seedGeneratorInt = (int)((dateTimeToday - dateTimeCompareTo).TotalDays);
        }

        private void Start()
        {
            StartLevel();
            dungeon = FindAnyObjectByType<RuntimeDungeon>();
            dungeon.Generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;
        }

        public void StartLevel()
        {
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
            levelRandom = new System.Random(seedGeneratorInt);
            levelSeed = levelRandom.Next(0, 100000000);
            
            dayNumber++;

            GenerateTaskList();
            GenerateDungeon();
        }


        void GenerateTaskList()
        {
            int itemCount = 0;
            int blueCapCount = levelRandom.Next(0, requiredMaterials - itemCount);
            itemCount += blueCapCount;
            int glowBudCount = levelRandom.Next(0, requiredMaterials - itemCount);
            itemCount += glowBudCount;
            int crystalCount = levelRandom.Next(0, requiredMaterials - itemCount);
            itemCount += crystalCount;
            int paleShroomCount = requiredMaterials - itemCount;
            itemCount += paleShroomCount;

            Debug.Log("blueCapCount " + blueCapCount);
            Debug.Log("glowBudCount " + glowBudCount);
            Debug.Log("crystalCount " + crystalCount);
            Debug.Log("paleShroom " + paleShroomCount);
            Debug.Log("itemCount " + itemCount);

            Debug.Log("test " + taskList.blueCapCount);

            // clear previous day
            taskList.blueCapCount = 0;
            taskList.glowBudCount = 0;
            taskList.crystalCount = 0;
            taskList.paleShroomCount = 0;

            // add new tasks
            taskList.blueCapCount = blueCapCount;
            taskList.glowBudCount = glowBudCount;
            taskList.crystalCount = crystalCount;
            taskList.paleShroomCount = paleShroomCount;
        }

        private void GenerateDungeon()
        {
            dungeon.Generator.LengthMultiplier = dungeonMultiplier;
            dungeon.Generator.Seed = levelSeed;
            dungeon.Generate();
        }

        public void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
        {
            if(status == GenerationStatus.Complete)
            {
                Debug.Log("Triggered rebuild");
                SpawnItems();

                SpawnEnemies();
            }
        }

        private void SpawnItems()
        {
            Debug.Log("Spawning Items!");
            RandomLootSpawner[] spawnerList = FindObjectsByType<RandomLootSpawner>(0);

            Shuffle(spawnerList);

            for (int i = 0; i < taskList.blueCapCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                if (!spawner) return;
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[0]);
                item.transform.position = spawner.transform.position;
                spawnedItems.Add(item);
            }
            for (int i = 0; i < taskList.glowBudCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                if (!spawner) return;
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[1]);
                item.transform.position = spawner.transform.position;
                spawnedItems.Add(item);
            }
            for (int i = 0; i < taskList.crystalCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                if (!spawner) return;
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[2]);
                item.transform.position = spawner.transform.position;
                spawnedItems.Add(item);
            }
            for (int i = 0; i < taskList.paleShroomCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                if (!spawner) return;
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[3]);
                item.transform.position = spawner.transform.position;
                spawnedItems.Add(item);
            }
            Debug.Log("Added items from tasklist " + spawnedItems.Count);
            // add some additional items too
            for(int i=0; i < requiredMaterials * 0.5; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                {
                    if (!spawner) return;
                    spawner.itemSpawned = true;
                    int itemNum = levelRandom.Next(0, spawner.spawnableItemList.Count);

                    GameObject item = Instantiate(spawner.spawnableItemList[itemNum]);
                    item.transform.position = spawner.transform.position;
                    spawnedItems.Add(item);
                }
            }
        }

        public RandomLootSpawner FindAvailableSpawner(RandomLootSpawner[] spawnerList)
        {
            for(int i=0; i<spawnerList.Length; i++)
            {
                if(!spawnerList[i].itemSpawned)
                {
                    return spawnerList[i];
                }
            }
            return null;
        }

        public void Shuffle(RandomLootSpawner[] spawnerList)
        {
            for (int t = 0; t < spawnerList.Length; t++)
            {
                RandomLootSpawner tmp = spawnerList[t];
                int r = levelRandom.Next(t, spawnerList.Length);
                spawnerList[t] = spawnerList[r];
                spawnerList[r] = tmp;
            }
        }

        public void DeliverItems(int bluecaps, int glowbuds, int crystals, int paleShrooms)
        {
            Debug.Log("Received bluecaps:" + bluecaps);
            Debug.Log("Received glowbuds:" + glowbuds);
            Debug.Log("Received crystals:" + crystals);
            Debug.Log("Received paleShrooms:" + paleShrooms);

            // update delivered items in tasklist
            taskList.deliveredBlueCaps += bluecaps;
            taskList.deliveredCrystals += crystals;
            taskList.deliveredGlowBuds += glowbuds;
            taskList.deliveredPaleShrooms += paleShrooms;

            // give coins to player
            int reward = bluecaps * blueCapValue;
            reward += glowbuds * glowBudValue;
            reward += crystals * crystalValue;
            reward += paleShrooms * paleShroomValue;

            coins += reward;
            Debug.Log("coins :" + coins);
            Debug.Log("quota :" + quota);

            // update UI
            // show received coins

            // show a little text to the player that they've finished their tasks and could end the day
        }

        private void SpawnEnemies()
        {
            EnemySpawnPoint[] spawnerList = FindObjectsByType<EnemySpawnPoint>(0);
            Shuffle(spawnerList);

            for (int i=0; i < enemyAmountAtStart; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.GetComponent<EnemyAI>().WarpToPosition(spawnerList[i].transform.position);
                enemy.GetComponent<EnemyAI>().SetTargetPlayer(playerTransform);
                enemies.Add(enemy);
            }
        }

        public void Shuffle(EnemySpawnPoint[] spawnerList)
        {
            for (int t = 0; t < spawnerList.Length; t++)
            {
                EnemySpawnPoint tmp = spawnerList[t];
                int r = levelRandom.Next(t, spawnerList.Length);
                spawnerList[t] = spawnerList[r];
                spawnerList[r] = tmp;
            }
        }

        public void EndDay()
        {
            if (dayNumber == 3)
            {
                if(coins < quota)
                {
                    GameOver();
                }
                coins -= quota;
                quota = (int)Mathf.Floor(quota + quota * 0.33f);
                dungeonMultiplier += 0.2f;
                requiredMaterials++;
                enemyAmountAtStart++;
                dayNumber = 0;
            }

            // give bonus points for completing the task
            int bonus = 0;
            if (taskList.deliveredBlueCaps >= taskList.blueCapCount) bonus += (int) Mathf.Floor(taskList.blueCapCount * blueCapValue * taskBonus);
            if (taskList.deliveredCrystals >= taskList.crystalCount) bonus += (int) Mathf.Floor(taskList.crystalCount * crystalValue * taskBonus);
            if (taskList.deliveredGlowBuds >= taskList.glowBudCount) bonus += (int) Mathf.Floor(taskList.glowBudCount * glowBudValue * taskBonus);
            if (taskList.deliveredPaleShrooms >= taskList.paleShroomCount) bonus += (int) Mathf.Floor(taskList.paleShroomCount * paleShroomValue * taskBonus);
            coins += bonus;

            // show bonus
            Debug.Log("Bonus! " + bonus);

            // destroy leftover items and enemies
            foreach (GameObject item in spawnedItems)
            {
                Debug.Log(item);
                Destroy(item);
            }
            Debug.Log(spawnedItems.Count);
            spawnedItems.Clear();
            Debug.Log(spawnedItems.Count);

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null) Destroy(enemy);
            }
            enemies.Clear();
            Debug.Log(enemies.Count);

            // if quota met, start next level
            StartLevel();
        }

        public void GameOver()
        {
            Debug.Log("Game Over");
            // show game over UI
            // stop game from running
            // thank the player
        }
    }

}
