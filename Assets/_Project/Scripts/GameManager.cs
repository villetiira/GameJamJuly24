using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DunGen;

namespace keijo
{
    public struct TaskList
    {
        public int blueCapCount;
        public int glowBudCount;
        public int crystalCount;
        public int paleShroomCount;
    }

    public class GameManager : MonoBehaviour
    {
        public RuntimeDungeon dungeon;
        public int seedGeneratorInt;
        public System.Random levelRandom;
        public int levelSeed;
        public int requiredMaterials = 5;
        public TaskList taskList;


        void Awake()
        {
            dungeon.Generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;

            DateTime dateTimeCompareTo = new DateTime(2024, 01, 10);
            DateTime dateTimeToday = DateTime.Today;
            seedGeneratorInt = (int)((dateTimeToday - dateTimeCompareTo).TotalDays);
        }

        private void Start()
        {
            StartLevel();
        }

        void StartLevel()
        {
            levelRandom = new System.Random(seedGeneratorInt);
            levelSeed = levelRandom.Next(0, 100000000);
            GenerateTaskList();
            GenerateDungeon();
        }

        void GenerateTaskList()
        {
            int itemCount = 0;
            int blueCapCount = levelRandom.Next(0, requiredMaterials - itemCount +1);
            itemCount += blueCapCount;
            int glowBudCount = levelRandom.Next(0, requiredMaterials - itemCount + 1);
            itemCount += glowBudCount;
            int crystalCount = levelRandom.Next(0, requiredMaterials - itemCount + 1);
            itemCount += crystalCount;
            int paleShroomCount = requiredMaterials - itemCount;
            itemCount += paleShroomCount;

            Debug.Log("blueCapCount " + blueCapCount);
            Debug.Log("glowBudCount " + glowBudCount);
            Debug.Log("crystalCount " + crystalCount);
            Debug.Log("paleShroom " + paleShroomCount);
            Debug.Log("itemCount " + itemCount);

            taskList = new TaskList();
            taskList.blueCapCount = blueCapCount;
            taskList.glowBudCount = glowBudCount;
            taskList.crystalCount = crystalCount;
            taskList.paleShroomCount = paleShroomCount;
        }

        private void GenerateDungeon()
        {
            dungeon.Generator.Seed = levelSeed;
            dungeon.Generate();
        }

        public void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
        {

            if(status == GenerationStatus.Complete)
            {
                SpawnItems();

                SpawnEnemies();
            }
        }

        private void SpawnItems()
        {
            Debug.Log("Spawning Items!");
            RandomLootSpawner[] spawnerList = FindObjectsByType<RandomLootSpawner>(0);


            for (int i = 0; i < taskList.blueCapCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[0]);
                item.transform.position = spawner.transform.position;
            }
            for (int i = 0; i < taskList.glowBudCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[1]);
                item.transform.position = spawner.transform.position;
            }
            for (int i = 0; i < taskList.crystalCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[2]);
                item.transform.position = spawner.transform.position;
            }
            for (int i = 0; i < taskList.paleShroomCount; i++)
            {
                RandomLootSpawner spawner = FindAvailableSpawner(spawnerList);
                spawner.itemSpawned = true;

                GameObject item = Instantiate(spawner.spawnableItemList[3]);
                item.transform.position = spawner.transform.position;
            }
        }

        public RandomLootSpawner FindAvailableSpawner(RandomLootSpawner[] spawnerList)
        {
            RandomLootSpawner spawner;
            int randomIndex = levelRandom.Next(0, spawnerList.Length - 1);
            spawner = spawnerList[randomIndex];
            while (spawner.itemSpawned)
            {
                randomIndex = levelRandom.Next(0, spawnerList.Length - 1);
                spawner = spawnerList[randomIndex];
            }
            return spawner;
        }

        public void DeliverItems(int bluecaps, int glowbuds, int crystals, int paleShrooms)
        {
            Debug.Log("Received :" + bluecaps);
            Debug.Log("Received :" + glowbuds);
            Debug.Log("Received :" + crystals);
            Debug.Log("Received :" + paleShrooms);
        }

        private void SpawnEnemies()
        {

        }
    }

}
