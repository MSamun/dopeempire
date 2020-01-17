using System;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class CharacterSkill : MonoBehavRefScript
    {
        [Serializable]
        public struct SkillInformation
        {
            public int skillID;
            public MultiplierTypes type;
            [Range(0, 1)] public float bonus;
            public float purchasePrice;
            public int currentLevel;
            public int maxLevel;
        }

        #region Variables

        private bool canPurchase;
        private Button localButtonReference = null;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Skill Information", order = 1)]
        public SkillInformation skillInfo;

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Skill Points Invested Text", order = 1)]
        [SerializeField] private Text currentSkillLevelText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            CheckRequirements();
            localButtonReference = CharacterManager.Instance.UpgradeButtonReference();
        }

        private void Start()
        {
            SetSkillInformationValuesViaJson();
            CharacterManager.Instance.GrabCharacterSkillLevelForJson(skillInfo.skillID, skillInfo.currentLevel);
        }

        private void SetSkillInformationValuesViaJson()
        {
            skillInfo.currentLevel = CharacterManager.Instance.CharacterSkillLevels[skillInfo.skillID];
            MultiplierManager.Instance.UpdateAndApplyMultiplier(skillInfo.type, skillInfo.bonus * skillInfo.currentLevel);
            UpdateCurrentSkillLevelText();
        }

        #endregion Initialization

        #region Custom Methods

        private void CheckRequirements()
        {
            //If you have enough Influence and the World Skill upgrade isn't maxed out, then you canPurchase.
            canPurchase = (CharacterManager.Instance.SkillPoints >= skillInfo.purchasePrice && skillInfo.currentLevel != skillInfo.maxLevel) ? true : false;
        }

        private void UpgradeSkill()
        {
            CheckRequirements();

            if (canPurchase)
            {
                skillInfo.currentLevel++;
                UpdateCurrentSkillLevelText();
                CharacterManager.Instance.DecrementSkillPoints();
                MultiplierManager.Instance.UpdateAndApplyMultiplier(skillInfo.type, skillInfo.bonus);
                CharacterManager.Instance.ChangeCharacterSkillDescTextComponents(skillInfo.type, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
                CharacterManager.Instance.GrabCharacterSkillLevelForJson(skillInfo.skillID, skillInfo.currentLevel);
                return;
            }
        }

        //Attached to the Skill Button via Unity Hierarchy.
        public void UpdateSkillDescTextAndButton()
        {
            CharacterManager.Instance.ChangeCharacterSkillDescTextComponents(skillInfo.type, skillInfo.bonus, skillInfo.currentLevel, skillInfo.maxLevel);
            localButtonReference.onClick.RemoveAllListeners();
            localButtonReference.onClick.AddListener(() => UpgradeSkill());
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