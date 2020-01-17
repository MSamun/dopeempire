using System.IO;
using UnityEngine;

namespace DopeEmpire
{
    public class DataManager : MonoBehavRefScript
    {
        public static DataManager Instance { get; private set; }

        public SaveIncomeData IncomeData { get; private set; }
        public SavePlayerData PlayerData { get; private set; }
        public SaveMultiplierData MultiplierData { get; private set; }

        #region Variables

        private string incomeDataPath = string.Empty;
        private string playerDataPath = string.Empty;
        private string multiplierDataPath = string.Empty;

        private string incomeDataFileName = "incomeData.json";
        private string playerDataFileName = "playerData.json";
        private string multiplierDataFileName = "multiplierData.json";

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            //Grabs the folder/file that has all the saved data.
            incomeDataPath = Application.persistentDataPath + "/" + incomeDataFileName;
            playerDataPath = Application.persistentDataPath + "/" + playerDataFileName;
            multiplierDataPath = Application.persistentDataPath + "/" + multiplierDataFileName;

            LoadFromJson();
        }

        #endregion Initialization

        #region Save Data To Json

        public void SaveToJson()
        {
            //Creates a new SaveData class, which resets everything to default values of 0.
            IncomeData = new SaveIncomeData();
            PlayerData = new SavePlayerData();
            MultiplierData = new SaveMultiplierData();

            //Grabs all the necessary information and shoves it into the SaveData class.
            SavePlayerData();
            SaveIncomeData();
            SaveMultiplierData();

            //Shoves all the SaveData information into one long ass string.
            string incomeContents = JsonUtility.ToJson(IncomeData, true);
            string playerContents = JsonUtility.ToJson(PlayerData, true);
            string multiplierContents = JsonUtility.ToJson(MultiplierData, true);

            //Writes it all to the dataFileName (data.json) and stores that file into the dataPath (Application.persistentDataPath, or the folder that the phone or computers allows us to store.)
            File.WriteAllText(incomeDataPath, incomeContents);
            File.WriteAllText(playerDataPath, playerContents);
            File.WriteAllText(multiplierDataPath, multiplierContents);
        }

        private void SavePlayerData()
        {
            PlayerData.playerLevelData = new PlayerLevelData(PlayerLevelManager.Instance.CurrentPlayerLevel, PlayerLevelManager.Instance.ExperienceEarned,
                PlayerLevelManager.Instance.RequiredExperienceToNextLevel);

            PlayerData.characterData = new CharacterData(CharacterManager.Instance.SkillPoints, CharacterManager.Instance.currentCharID);

            PlayerData.characterSkillsData = new CharacterSkillsData[6];

            for (int i = 0; i < PlayerData.characterSkillsData.Length; i++)
            {
                PlayerData.characterSkillsData[i] = new CharacterSkillsData(i, CharacterManager.Instance.CharacterSkillLevels[i]);
            }
        }

        private void SaveIncomeData()
        {
            IncomeData.totalIncomeData = new TotalIncomeData(CurrencyManager.Instance.DirtyMoneyTotal, CurrencyManager.Instance.CleanMoneyTotal, CurrencyManager.Instance.ReputationTotal,
                CurrencyManager.Instance.DirtyMoneyWeekly, CurrencyManager.Instance.CleanMoneyWeekly, CurrencyManager.Instance.ReputationWeekly);

            IncomeData.numberOfOwnedProducts = ProductManager.Instance.CurrentNumberOfOwnedProducts;
            IncomeData.totalNumberOfProductsCanSell = ProductManager.Instance.TotalNumberOfProductCanSell;

            IncomeData.productIncomeData = new ProductIncomeData[IncomeData.numberOfOwnedProducts];
            for (int i = 0; i < IncomeData.numberOfOwnedProducts; i++)
            {
                IncomeData.productIncomeData[i] = new ProductIncomeData(i, ProductManager.Instance.ProductLevelValues[i], ProductManager.Instance.ProductDirtyMoneyValues[i],
                    ProductManager.Instance.ProductCleanMoneyValues[i], ProductManager.Instance.ProductReputationValues[i], ProductManager.Instance.ProductTimerValues[i]);
            }
        }

        private void SaveMultiplierData()
        {
            MultiplierData.productMultiplierData = new ProductMultiplierData(MultiplierManager.Instance.DirtyMoneyIncreaseMultiplier, MultiplierManager.Instance.CleanMoneyIncreaseMultiplier,
                MultiplierManager.Instance.ReputationIncreaseMultiplier, MultiplierManager.Instance.InfluenceIncreaseMultiplier, MultiplierManager.Instance.TimerReductionMultiplier);

            MultiplierData.productQualityData = new ProductQualityData(MultiplierManager.Instance.CurrentResearchedGrade, MultiplierManager.Instance.CurrentResearchedProduct,
                MultiplierManager.Instance.NextResearchedGrade, MultiplierManager.Instance.NextResearchedProduct);

            MultiplierData.researchMultiplierData = new ResearchMultiplierData(MultiplierManager.Instance.LaunderRateIncreaseMultiplier, MultiplierManager.Instance.ExperienceIncreaseMultiplier,
                MultiplierManager.Instance.SecondsBetweenWeeksReductionMultiplier, MultiplierManager.Instance.MoneyButtonClickIncreaseMultiplier, MultiplierManager.Instance.ResearchCostReductionMultiplier,
                MultiplierManager.Instance.AdBonusLengthIncreaseMultiplier, MultiplierManager.Instance.AdBonusRateIncreaseMultiplier);
        }

        #endregion Save Data To Json

        #region Load Data From Json

        public void LoadFromJson()
        {
            if (File.Exists(incomeDataPath))
            {
                //Writes all the data.json values into one long ass string.
                string incomeContents = File.ReadAllText(incomeDataPath);

                //Shoves all the content from the string "contents" into the SaveData information.
                IncomeData = JsonUtility.FromJson<SaveIncomeData>(incomeContents);

                //Loads up and replaces all the necessary information.
                LoadIncomeData();
            }
            else
            {
                ResetGameAndJson();
                Start();
            }

            if (File.Exists(playerDataPath))
            {
                //Writes all the data.json values into one long ass string.
                string playerContents = File.ReadAllText(playerDataPath);

                //Shoves all the content from the string "contents" into the SaveData information.
                PlayerData = JsonUtility.FromJson<SavePlayerData>(playerContents);

                //Loads up and replaces all the necessary information.
                LoadPlayerData();
            }
            else
            {
                ResetGameAndJson();
                Start();
            }

            if (File.Exists(multiplierDataPath))
            {
                //Writes all the data.json values into one long ass string.
                string multiplierContents = File.ReadAllText(multiplierDataPath);

                //Shoves all the content from the string "contents" into the SaveData information.
                MultiplierData = JsonUtility.FromJson<SaveMultiplierData>(multiplierContents);

                //Loads up and replaces all the necessary information.
                LoadMultiplierData();
            }
            else
            {
                ResetGameAndJson();
                Start();
            }
        }

        private void LoadPlayerData()
        {
            PlayerLevelManager.Instance.SetPlayerValuesViaJson(PlayerData.playerLevelData);
            CharacterManager.Instance.SetEarnedSkillPointsViaJson(PlayerData.characterData);
            CharacterManager.Instance.SetCharacterSkillLevelsViaJson(PlayerData.characterSkillsData);
        }

        private void LoadIncomeData()
        {
            CurrencyManager.Instance.SetIncomeValuesViaJson(IncomeData.totalIncomeData);
            ProductManager.Instance.SetOwnedProductsAndTotalProductsViaJson(IncomeData.numberOfOwnedProducts, IncomeData.totalNumberOfProductsCanSell);
            ProductManager.Instance.SetProductInformationViaJson(IncomeData.productIncomeData);
        }

        private void LoadMultiplierData()
        {
            MultiplierManager.Instance.SetAllMultiplierValuesViaJson(MultiplierData);
        }

        #endregion Load Data From Json

        #region Reset Game and Json

        public void ResetGameAndJson()
        {
            IncomeData = new SaveIncomeData
            {
                numberOfOwnedProducts = 1,
                totalNumberOfProductsCanSell = 3,
                productIncomeData = new ProductIncomeData[1]
            };

            PlayerData = new SavePlayerData
            {
                playerLevelData = new PlayerLevelData(1, 0, 200),
                characterData = new CharacterData(0, 7),
            };

            MultiplierData = new SaveMultiplierData
            {
            };

            IncomeData.productIncomeData[0] = new ProductIncomeData(0, 1, 15, 0, 0, 0.10f);

            //Shoves all the SaveData information into one long ass string.
            string incomeContents = JsonUtility.ToJson(IncomeData, true);
            string playerContents = JsonUtility.ToJson(PlayerData, true);
            string multiplierContents = JsonUtility.ToJson(MultiplierData, true);

            //Writes it all to the dataFileName (data.json) and stores that file into the dataPath (Application.persistentDataPath, or the folder that the phone or computers allows us to store.)
            File.WriteAllText(incomeDataPath, incomeContents);
            File.WriteAllText(playerDataPath, playerContents);
            File.WriteAllText(multiplierDataPath, multiplierContents);

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        #endregion Reset Game and Json
    }
}