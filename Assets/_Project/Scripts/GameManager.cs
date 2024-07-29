using System.Collections.Generic;
using System;
using UnityEngine;
using DunGen;
using TMPro;

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

        [Header("Player")]
        public PlayerController player;
        public Transform playerTransform;

        [Header("Level settings")]
        public int levelSeed;
        public int requiredMaterials = 5;
        public TaskList taskList = new TaskList();
        public int roundNumber = 0;
        public int dayNumber = 0;
        public int quota = 100;
        public int coins = 0;
        public float dungeonMultiplier = 1;

        [Header("UI")]
        public GameObject gameOverScreen;
        public TMP_Text gameOverReason;
        public GameObject dayStartingScreen;
        public GameObject nextDayScreen;
        public GameObject pauseMenu;
        public TMP_Text dayNumberDisplay;
        public TMP_Text currentCoins;
        public TMP_Text bills;
        public TMP_Text daysUntilBills;
        public GameObject saleUI;
        public TMP_Text firstRow;
        public TMP_Text secondRow;
        public TMP_Text thirdRow;
        public TMP_Text fourthRow;
        public TMP_Text totalRow;
        public TMP_Text currentCoinsUI;
        bool displayDayInfo = false;
        float dayInfoTimer = 5f;
        bool dayStartingScreenActive = false;
        float dayStartingScreenTimer = 2f;
        bool saleUIActive = false;
        float saleUIActiveTimer = 3f;

        [Header("Audio")]
        public AudioSource backgroundMusic;
        public AudioSource gameOverMusic;
        public AudioSource dungeonBackground;

        public List<GameObject> spawnedItems = new List<GameObject>();
        public List<GameObject> enemies = new List<GameObject>();

        [Header("Item values")]
        public int hotSauceValue = 20;
        public int manaPotionValue = 30;
        public int elixirOfProtectionValue = 40;
        public int healthPotionValue = 30;
        public float taskBonus = 0.5f;

        [Header("Enemies")]
        public GameObject enemyPrefab;
        public int enemyAmountAtStart = 2;

        void Awake()
        {
            DateTime dateTimeCompareTo = new DateTime(2024, 01, 10);
            DateTime dateTimeToday = DateTime.Today;
            seedGeneratorInt = (int)((dateTimeToday - dateTimeCompareTo).TotalDays);
            dungeon = FindAnyObjectByType<RuntimeDungeon>();
            dungeon.Generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;
        }

        private void Start()
        {
            StartLevel();
            Time.timeScale = 1;
        }

        private void Update()
        {
            if(displayDayInfo)
            {
                dayInfoTimer -= Time.deltaTime;
                if(dayInfoTimer < 0)
                {
                    displayDayInfo = false;
                    dayStartingScreen.SetActive(false);
                }
            }
            if(dayStartingScreenActive)
            {
                dayStartingScreenTimer -= Time.deltaTime;
                if(dayStartingScreenTimer < 0)
                {
                    dayStartingScreenActive = false;
                    nextDayScreen.SetActive(false);
                    dayStartingScreenTimer = 2f;
                    player.LevelLoaded();
                }
            }
            if(saleUIActive)
            {
                saleUIActiveTimer -= Time.deltaTime;
                if (saleUIActiveTimer < 0)
                {
                    saleUIActive = false;
                    saleUI.SetActive(false);
                    saleUIActiveTimer = 3f;
                }
            }
        }

        public void StartLevel()
        {
            dayNumber++;
            roundNumber++;
            ShowDayInfo();
            levelRandom = new System.Random(seedGeneratorInt);
            levelSeed = levelRandom.Next(0, 100000000);

            GenerateTaskList();
            GenerateDungeon();
        }

        void ShowDayInfo()
        {
            dayInfoTimer = 5;
            displayDayInfo = true;
            dayNumberDisplay.text = "Day " + dayNumber;
            bills.text = "Bills To Pay: " + quota;
            daysUntilBills.text = "Bills Due: " + (3 - roundNumber) + " days";
            currentCoins.text = "Current Coins: " + coins;
            dayStartingScreen.SetActive(true);
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
                SpawnItems();

                SpawnEnemies();
            }
        }

        private void SpawnItems()
        {
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

        public void DeliverItems(int hotSauce, int manaPotion, int elixirOfProtection, int healthPotion)
        {

            // update delivered items in tasklist
            /*taskList.deliveredHotSauce += hotSauce;
            taskList.deliveredManaPotion += manaPotion;
            taskList.deliveredElixirOfProtection += elixirOfProtection;
            taskList.deliveredHealthPotions += healthPotion;*/
            firstRow.text = "";
            secondRow.text = "";
            thirdRow.text = "";
            fourthRow.text = "";
            totalRow.text = "";

            int itemCount = 0;
            if (hotSauce > 0)
            {
                firstRow.text = "Hot Sauce: " + hotSauce + " x " + hotSauceValue;
                itemCount++;
            }
            if (manaPotion > 0)
            {
                if(itemCount == 1)
                {
                    secondRow.text = "Mana Potion: " + manaPotion + " x " + manaPotionValue;
                }
                else
                {
                    firstRow.text = "Mana Potion: " + manaPotion + " x " + manaPotionValue;
                }
                itemCount++;
            }
            if (elixirOfProtection > 0)
            {
                if(itemCount == 2)
                {
                    thirdRow.text = "Elixir of Protection: " + elixirOfProtection + " x " + elixirOfProtectionValue;
                }
                if (itemCount == 1)
                {
                    secondRow.text = "Elixir of Protection: " + elixirOfProtection + " x " + elixirOfProtectionValue;
                }
                else
                {
                    firstRow.text = "Elixir of Protection: " + elixirOfProtection + " x " + elixirOfProtectionValue;
                }
                itemCount++;
            }
            if (healthPotion > 0)
            {
                if (itemCount == 3)
                {
                    fourthRow.text = "Health Potion: " + healthPotion + " x " + healthPotionValue;
                }
                if (itemCount == 2)
                {
                    thirdRow.text = "Health Potion: " + healthPotion + " x " + healthPotionValue;
                }
                if (itemCount == 1)
                {
                    secondRow.text = "Health Potion: " + healthPotion + " x " + healthPotionValue;
                }
                else
                {
                    firstRow.text = "Health Potion: " + healthPotion + " x " + healthPotionValue;
                }
            }

            // give coins to player
            int reward = hotSauce * hotSauceValue;
            reward += manaPotion * manaPotionValue;
            reward += elixirOfProtection * elixirOfProtectionValue;
            reward += healthPotion * healthPotionValue;
            coins += reward;

            totalRow.text = "Total: " + reward;
            currentCoinsUI.text = "Current Coins: " + coins;

            saleUI.SetActive(true);
            saleUIActive = true;
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

        public void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Unpause()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void EndDay()
        {
            // give bonus points for completing the task
            /*int bonus = 0;
            if (taskList.deliveredBlueCaps >= taskList.blueCapCount) bonus += (int) Mathf.Floor(taskList.blueCapCount * blueCapValue * taskBonus);
            if (taskList.deliveredCrystals >= taskList.crystalCount) bonus += (int) Mathf.Floor(taskList.crystalCount * crystalValue * taskBonus);
            if (taskList.deliveredGlowBuds >= taskList.glowBudCount) bonus += (int) Mathf.Floor(taskList.glowBudCount * glowBudValue * taskBonus);
            if (taskList.deliveredPaleShrooms >= taskList.paleShroomCount) bonus += (int) Mathf.Floor(taskList.paleShroomCount * paleShroomValue * taskBonus);
            coins += bonus;*/

            // show bonus
           // Debug.Log("Bonus! " + bonus);

            // destroy leftover items and enemies
            foreach (GameObject item in spawnedItems)
            {
                Destroy(item);
            }
            spawnedItems.Clear();

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null) Destroy(enemy);
            }
            enemies.Clear();

            if (roundNumber == 3)
            {
                if (coins < quota)
                {
                    GameOver(false);
                    return; 
                }
                coins -= quota;
                quota = (int)Mathf.Floor(quota + quota * 0.33f);
                dungeonMultiplier += 0.2f;
                requiredMaterials++;
                enemyAmountAtStart++;
                roundNumber = 0;
            }
            // if quota met, start next level
            nextDayScreen.SetActive(true);
            dayStartingScreenActive = true;
            player.DaySwitched();
            StartLevel();
        }

        public void GameOver(bool playerDied)
        {
            // show game over UI
            gameOverScreen.SetActive(true);
            gameOverReason.text = playerDied ? "You died.." : "You couldn't pay the bills..";

            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            // stop game from running
            backgroundMusic.volume = 0;
            dungeonBackground.volume = 0;
            gameOverMusic.volume = 100;
            gameOverMusic.Play();
            // thank the player
            // move to a diffrent scene?
        }
    }

}
