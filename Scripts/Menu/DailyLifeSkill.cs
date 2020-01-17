using System;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class DailyLifeSkill : MonoBehavRefScript
    {
        [Serializable]
        public struct SkillInformation
        {
            public MultiplierTypes type;
            [Range(0, 1)] public float bonus;
            public float purchasePrice;
            public int currentLevel;
            public int maxLevel;
        }

        #region Variables

        private bool canPurchase;
        private Button localUpgradeButtonReference = null;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Skill Information", order = 1)]
        [SerializeField] private SkillInformation skillInfo;

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Daily Life Skill Level Text", order = 1)]
        [SerializeField] private Text currentSkillLevelText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            CheckRequirements();
            localUpgradeButtonReference = DailyLifeManager.Instance.UpgradeButtonReference();
        }

        #endregion Initialization

        #region Custom Methods

        private void CheckRequirements()
        {
            //If you have enough Clean Money and the Daily Life Skill upgrade isn't maxed out, then you canPurchase.
            canPurchase = (CurrencyManager.Instance.CleanMoneyTotal >= skillInfo.purchasePrice && skillInfo.currentLevel != skillInfo.maxLevel) ? true : false;
        }

        private void UpgradeSkill()
        {
            CheckRequirements();

            if (canPurchase)
            {
                skillInfo.currentLevel++;
                UpdateCurrentSkillLevelText();

                CurrencyManager.Instance.TotalIncomeGate(true, Income.CleanMoney, skillInfo.purchasePrice);
                skillInfo.purchasePrice *= 1.30f;

                MultiplierManager.Instance.UpdateAndApplyMultiplier(skillInfo.type, skillInfo.bonus);
                DailyLifeManager.Instance.ChangeDailyLifeSkillDescTextComponents(skillInfo.type, skillInfo.purchasePrice, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
                return;
            }
        }

        //Attached to the Skill Button via Unity Hierarchy.
        public void UpdateSkillDescTextAndButton()
        {
            DailyLifeManager.Instance.ChangeDailyLifeSkillDescTextComponents(skillInfo.type, skillInfo.purchasePrice, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
            localUpgradeButtonReference.onClick.RemoveAllListeners();
            localUpgradeButtonReference.onClick.AddListener(() => UpgradeSkill());
        }

        public void UpdateCurrentSkillLevelText()
        {
            if (skillInfo.currentLevel != skillInfo.maxLevel)
            {
                currentSkillLevelText.text = string.Format("{0}", skillInfo.currentLevel);
            }
            else
            {
                currentSkillLevelText.text = "MAX";
            }
        }

        #endregion Custom Methods
    }
}