using System;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class Research : MonoBehavRefScript
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

        private float localCostReductionMultiplier;
        private bool canPurchase;
        private Button localUpgradeButtonReference = null;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Skill Info", order = 1)]
        [SerializeField] private SkillInformation skillInfo;

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Research Upgrade Level Text", order = 1)]
        [SerializeField] private Text currentSkillLevelText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            CheckRequirements();
            localUpgradeButtonReference = ResearchManager.Instance.UpgradeButtonReference();

            localCostReductionMultiplier = 0;
        }

        #endregion Initialization

        #region Custom Methods

        private void CheckRequirements()
        {
            ApplyResearchCostReductionMultiplier();
            //If you have enough Reputation and the Research Skill upgrade isn't maxed out, then you canPurchase.
            canPurchase = (CurrencyManager.Instance.ReputationTotal >= skillInfo.purchasePrice && skillInfo.currentLevel != skillInfo.maxLevel) ? true : false;
        }

        private void UpgradeSkill()
        {
            CheckRequirements();

            if (canPurchase)
            {
                skillInfo.currentLevel++;
                UpdateCurrentSkillLevelText();

                CurrencyManager.Instance.TotalIncomeGate(true, Income.Reputation, skillInfo.purchasePrice);
                skillInfo.purchasePrice *= 1.30f;

                MultiplierManager.Instance.UpdateAndApplyMultiplier(skillInfo.type, skillInfo.bonus);
                ResearchManager.Instance.ChangeResearchSkillDescTextComponents(skillInfo.type, skillInfo.purchasePrice, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
                return;
            }
        }

        //Attached to the Skill Button via Unity Hierarchy.
        public void UpdateSkillDescTextAndButton()
        {
            ApplyResearchCostReductionMultiplier();
            ResearchManager.Instance.ChangeResearchSkillDescTextComponents(skillInfo.type, skillInfo.purchasePrice, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
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

        private void ApplyResearchCostReductionMultiplier()
        {
            if (localCostReductionMultiplier != MultiplierManager.Instance.ResearchCostReductionMultiplier)
            {
                skillInfo.purchasePrice /= 1 - localCostReductionMultiplier;
                skillInfo.purchasePrice *= 1 - MultiplierManager.Instance.ResearchCostReductionMultiplier;
                localCostReductionMultiplier = MultiplierManager.Instance.ResearchCostReductionMultiplier;
            }
        }

        #endregion Custom Methods
    }
}