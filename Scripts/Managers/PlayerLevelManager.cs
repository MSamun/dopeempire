using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class PlayerLevelManager : MonoBehavRefScript
    {
        public static PlayerLevelManager Instance { get; private set; }
        public int CurrentPlayerLevel { get; private set; }
        public int MaxPlayerLevel { get; private set; }
        public float ExperienceEarned { get; private set; }
        public int RequiredExperienceToNextLevel { get; private set; }
        public float LifetimeExperienceEarned { get; private set; }

        #region Game Components

        [Header("---------- PLAYER LEVEL COMPONENTS ----------", order = 0)]
        [Header("Level EXP Bar", order = 1)]
        [SerializeField] private Slider levelSlider = null;

        [Header("Current Level")]
        [SerializeField] private Text playerLevelText = null;

        [Header("Current/Max EXP")]
        [SerializeField] private Text experienceEarnedText = null;

        #endregion Game Components

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

        public void SetPlayerValuesViaJson(PlayerLevelData jsonData)
        {
            MaxPlayerLevel = 150;

            try
            {
                CurrentPlayerLevel = jsonData.currentLevel;
                ExperienceEarned = jsonData.experienceEarned;
                RequiredExperienceToNextLevel = jsonData.requiredExperienceToNextLevel;
            }
            catch (System.NullReferenceException)
            {
                DataManager.Instance.SaveToJson();
            }

            SetLevelAndExpSliderValues();
        }

        #endregion Initialization

        #region Custom Methods

        private void SetLevelAndExpSliderValues()
        {
            if (levelSlider.maxValue != RequiredExperienceToNextLevel)
            {
                levelSlider.maxValue = RequiredExperienceToNextLevel;
            }

            float percentage = ExperienceEarned / levelSlider.maxValue;
            levelSlider.minValue = 0;
            levelSlider.value = ExperienceEarned;

            playerLevelText.text = string.Format("{0}", CurrentPlayerLevel);
            experienceEarnedText.text = string.Format("{0}/{1}", CurrencyManager.Instance.FormatValues(ExperienceEarned), CurrencyManager.Instance.FormatValues(RequiredExperienceToNextLevel));
        }

        public void AddExperience(float value)
        {
            if (CurrentPlayerLevel != MaxPlayerLevel)
            {
                ExperienceEarned += value * (1 + MultiplierManager.Instance.ExperienceIncreaseMultiplier);
                LifetimeExperienceEarned += value * (1 + MultiplierManager.Instance.ExperienceIncreaseMultiplier);
                SetLevelAndExpSliderValues();
                CheckIfLevelUp();
            }
            else
            {
                LifetimeExperienceEarned += value * (1 + MultiplierManager.Instance.ExperienceIncreaseMultiplier);
            }
        }

        private void CheckIfLevelUp()
        {
            if (ExperienceEarned >= RequiredExperienceToNextLevel)
            {
                CurrentPlayerLevel++;

                if (CurrentPlayerLevel != MaxPlayerLevel)
                {
                    ExperienceEarned = 0;
                    RequiredExperienceToNextLevel = Mathf.RoundToInt(RequiredExperienceToNextLevel * 1.20f);
                    CharacterManager.Instance.IncrementSkillPoints();
                    SetLevelAndExpSliderValues();
                }

                //If player has reached max level of 150.
                else
                {
                    levelSlider.minValue = 0;
                    levelSlider.maxValue = 1;
                    levelSlider.value = levelSlider.maxValue;
                    ExperienceEarned = 0;
                    RequiredExperienceToNextLevel = 0;
                    playerLevelText.text = string.Format("{0}", MaxPlayerLevel);
                    experienceEarnedText.text = string.Format("MAX LEVEL");
                    CharacterManager.Instance.IncrementSkillPoints();
                }
            }
        }

        #endregion Custom Methods
    }
}