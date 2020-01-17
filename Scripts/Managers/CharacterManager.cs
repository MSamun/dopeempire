using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class CharacterManager : MonoBehavRefScript
    {
        public static CharacterManager Instance { get; private set; }
        public int SkillPoints { get; private set; }
        public int[] CharacterSkillLevels { get; private set; } = new int[6];

        #region Variables

        //Local references from Character Skills. Used to check current state of Upgrade Button, and enable/disable accordingly.
        private int l_currentLevel;

        private int l_maxLevel;

        [HideInInspector] public int currentCharID;

        #region Skill Descriptions

        private string dirtyMoneySkillDesc = "Increase the amount of <color=#FF9600>Dirty Money</color> earned from Products";
        private string cleanMoneySkillDesc = "Increase the amount of <color=#009600FF>Clean Money</color> earned from Products";
        private string reputationSkillDesc = "Increase the amount of <color=#00A0FFFF>Reputation</color> earned from Products";
        private string timerSkillDesc = "Reduce the amount of Time caused by Products";
        private string influenceSkillDesc = "Increases the amount of <color=#C800FF>Influence</color> earned from Cities";
        private string researchSkillDesc = "Reduces the costs of all <color=#FF0000>Research Skills</color>";

        #endregion Skill Descriptions

        #region Game Components

        [Header("---------- CHOOSE CHAR COMPONENTS ----------", order = 0)]
        [Header("Current Character Icon", order = 1)]
        [SerializeField] private Image currentCharIcon = null;

        [SerializeField] private Image currentCharIcon_MM = null;
        [SerializeField] private GameObject hasSkillPointsIndicator = null;

        [Header("Skill Points Text")]
        [SerializeField] private Text skillPointsText = null;

        [Header("Array of Character Icon Images")]
        [SerializeField] private Sprite[] arrayOfIcons = new Sprite[7];

        [Header("---------- UPGRADE COMPONENTS ----------", order = 0)]
        [Header("Tap Skill Desc Text", order = 1)]
        [SerializeField] private Text tapSkillDescText = null;

        [Header("Skill Title Text")]
        [SerializeField] private Text skillTitleText = null;

        [Header("Skill Description Text")]
        [SerializeField] private Text skillDescText = null;

        [Header("Upgrade Skill Button")]
        [SerializeField] private Button upgradeSkillButton = null;

        private Text upgradeSkillButtonText = null;

        [Header("Skill Cap Text")]
        [SerializeField] private Text skillCapText = null;

        #endregion Game Components

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

            upgradeSkillButtonText = upgradeSkillButton.GetComponentInChildren<Text>(true);
            l_currentLevel = 0;
            l_maxLevel = 1;
        }

        public void GrabCharacterSkillLevelForJson(int ID, int level)
        {
            CharacterSkillLevels[ID] = level;
        }

        public void SetEarnedSkillPointsViaJson(CharacterData jsonData)
        {
            SkillPoints = jsonData.ownedSkillPoints;
            skillPointsText.text = string.Format("SKILL POINTS: {0}", SkillPoints);

            SetCharacterIcon(jsonData.charIconID);
            CheckIfCanUpgradeCharacterSkill();
        }

        public void SetCharacterSkillLevelsViaJson(CharacterSkillsData[] jsonData)
        {
            for (int i = 0; i < CharacterSkillLevels.Length; i++)
            {
                try
                {
                    if (i == jsonData[i].skillID)
                    {
                        CharacterSkillLevels[i] = jsonData[i].currentSkillLevel;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    DataManager.Instance.SaveToJson();
                }
            }
        }

        #endregion Initialization

        #region Custom Methods

        /// <summary>
        /// Each Character Skill gives a specific increase (or decrease) for a certain income (or value) in the game.
        /// </summary>
        /// <param name="type">What type of multiplier is this skill affecting?</param>
        /// <param name="bonus">How much does this upgrade increase the multiplier by?</param>
        /// <param name="currentLevel">How many times have you already purchased this upgrade?</param>
        /// <param name="maxLevel">What's the maximum amount of times you can purchase this upgrade?</param>
        public void ChangeCharacterSkillDescTextComponents(MultiplierTypes type, float bonus, int currentLevel, int maxLevel)
        {
            EnableSkillDescTextGameObjects(true);

            l_currentLevel = currentLevel;
            l_maxLevel = maxLevel;

            string flexDesc = string.Empty;
            float currentBonus = Mathf.Round(bonus * 100f) * currentLevel;
            float nextBonus = currentBonus + Mathf.Round(bonus * 100f);

            if (type == MultiplierTypes.DirtyMoneyIncrease)
            {
                skillTitleText.text = string.Format("INCREASE DIRTY MONEY");
                flexDesc = dirtyMoneySkillDesc;
            }

            if (type == MultiplierTypes.CleanMoneyIncrease)
            {
                skillTitleText.text = string.Format("INCREASE CLEAN MONEY");
                flexDesc = cleanMoneySkillDesc;
            }

            if (type == MultiplierTypes.ReputationIncrease)
            {
                skillTitleText.text = string.Format("INCREASE REPUTATION");
                flexDesc = reputationSkillDesc;
            }

            if (type == MultiplierTypes.TimerReduction)
            {
                skillTitleText.text = string.Format("REDUCE PRODUCT TIMER");
                flexDesc = timerSkillDesc;
            }

            if (type == MultiplierTypes.InfluenceIncrease)
            {
                skillTitleText.text = string.Format("INCREASE INFLUENCE");
                flexDesc = influenceSkillDesc;
            }

            if (type == MultiplierTypes.ResearchCostReduction)
            {
                skillTitleText.text = string.Format("REDUCE RESEARCH COST");
                flexDesc = researchSkillDesc;
            }

            skillDescText.text = currentLevel != maxLevel ? string.Format("{0}.\n[{1}% -> <color=#FF0000>{2}%</color>]", flexDesc, currentBonus, nextBonus) : string.Format("{0} by <color=#FF0000>{1}%</color>.", flexDesc, currentBonus);
            skillCapText.text = string.Format("{0}/{1}", currentLevel, maxLevel);
            CheckIfCanUpgradeCharacterSkill();
        }

        private void CheckIfCanUpgradeCharacterSkill()
        {
            upgradeSkillButtonText.text = "Upgrade";

            if (SkillPoints > 0)
            {
                upgradeSkillButton.interactable = true;
                upgradeSkillButtonText.color = Color.white;
                hasSkillPointsIndicator.gameObject.SetActive(true);
            }
            else
            {
                upgradeSkillButton.interactable = false;
                upgradeSkillButtonText.color = ProductManager.disabledColor;
                hasSkillPointsIndicator.gameObject.SetActive(false);
            }

            if (l_currentLevel == l_maxLevel)
            {
                upgradeSkillButton.interactable = false;
                upgradeSkillButtonText.color = ProductManager.disabledColor;
                upgradeSkillButtonText.text = "MAXED";
                return;
            }
        }

        //Attached to the Characters Overlay Exit Button via Unity Hierarchy.
        public void ResetSkillDescText()
        {
            EnableSkillDescTextGameObjects(false);
        }

        //Disables default text ("Tap Skill to upgrade [...]") and enables appropriate text/button components.
        private void EnableSkillDescTextGameObjects(bool isEnable)
        {
            tapSkillDescText.gameObject.SetActive(!isEnable);

            skillTitleText.gameObject.SetActive(isEnable);
            skillDescText.gameObject.SetActive(isEnable);
            upgradeSkillButton.gameObject.SetActive(isEnable);
            skillCapText.gameObject.SetActive(isEnable);
        }

        //Attached to the Choose Character Buttons via Unity Hierarchy.
        public void SetCharacterIcon(int charID)
        {
            currentCharIcon.sprite = arrayOfIcons[charID];
            currentCharIcon_MM.sprite = arrayOfIcons[charID];
            currentCharID = charID;
        }

        public void DecrementSkillPoints()
        {
            SkillPoints--;
            skillPointsText.text = string.Format("SKILL POINTS: {0}", SkillPoints);
            CheckIfCanUpgradeCharacterSkill();
        }

        public void IncrementSkillPoints()
        {
            SkillPoints++;
            skillPointsText.text = string.Format("SKILL POINTS: {0}", SkillPoints);
            CheckIfCanUpgradeCharacterSkill();
        }

        //Referenced by CharacterSkill.cs to add skill-specific listeners (UpgradeSkill method.)
        public Button UpgradeButtonReference()
        {
            return upgradeSkillButton;
        }

        #endregion Custom Methods
    }
}