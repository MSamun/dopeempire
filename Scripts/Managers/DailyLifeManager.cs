using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class DailyLifeManager : MonoBehavRefScript
    {
        //Singleton.
        public static DailyLifeManager Instance { get; private set; }

        #region Variables

        //Local references from Daily Life Skills. Used to check current state of Upgrade Button, and enable/disable accordingly.
        private float l_skillCost;

        private int l_currentLevel;
        private int l_maxLevel;

        #region Daily Life Summaries

        private string officialsSkillDesc = "Reduces the time required to wait when tapping the Money Button.";
        private string businessSkillDesc = "Converts a percentage of <color=#DC7800FF>Dirty Money</color> to <color=#009600FF>Clean Money</color> every week.";
        private string partnersSkillDesc = "Increases the amount of <color=#DC7800FF>Dirty Money</color> earned through Products.";
        private string estatesSkillDesc = "Converts a percentage of <color=#DC7800FF>Dirty Money</color> to <color=#00A0FFFF>Reputation</color> every week.";
        private string vehiclesSkillDesc = "Increases the number of Products you can have active at once.";
        private string weaponsSkillDesc = "Increases the conversion rates for both <color=#009600FF>Clean Money</color> and <color=#00A0FFFF>Reputation</color>.";

        #endregion Daily Life Summaries

        #region Game Components

        [SerializeField] private GameObject hasSkillPointsIndicator = null;

        [Header("---------- UPGRADE COMPONENTS ----------", order = 0)]
        [Header("Tap Skill Desc Text", order = 1)]
        [SerializeField] private Text tapSkillDescText = null;

        [Header("Multiplier Text")]
        [SerializeField] private Text multiplierText = null;

        [Header("Skill Title Text")]
        [SerializeField] private Text skillTitleText = null;

        [Header("Skill Description Text")]
        [SerializeField] private Text skillDescText = null;

        [Header("Upgrade Skill Button")]
        [SerializeField] private Button upgradeSkillButton = null;

        private Text upgradeSkillButtonText = null;

        [Header("Skill Cost")]
        [SerializeField] private Text skillCostText = null;

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
            l_skillCost = 0;
            l_currentLevel = 0;
            l_maxLevel = 0;
        }

        private void Start()
        {
            CurrencyManager.Instance.OnMoneyValuesChanged += CheckIfCanPurchaseDailyLifeSkillUpgrade;
        }

        #endregion Initialization

        #region Custom Methods

        /// <summary>
        /// Each Daily Life Skill gives a specific increase (or decrease) for a certain income (or value) in the game.
        /// </summary>
        /// <param name="type">What type of multiplier is this skill affecting?</param>
        /// <param name="cost">What's the cost to purchase this upgrade?</param>
        /// <param name="bonus">How much does this upgrade increase the multiplier by?</param>
        /// <param name="currentLevel">How many times have you already purchased this upgrade?</param>
        /// <param name="maxLevel">What's the maximum amount of times you can purchase this upgrade?</param>
        public void ChangeDailyLifeSkillDescTextComponents(MultiplierTypes type, float cost, float bonus, int currentLevel, int maxLevel)
        {
            EnableSkillDescTextGameObjects(true);

            l_currentLevel = currentLevel;
            l_maxLevel = maxLevel;
            l_skillCost = cost;

            string flexDesc = string.Empty;
            string flexMultiplierDesc = string.Empty;
            float currentBonus = Mathf.Round(bonus * 100f) * currentLevel;
            float nextBonus = currentBonus + Mathf.Round(bonus * 100f);
            string oper = "%";

            //Officials.
            if (type == MultiplierTypes.TimerReduction)
            {
                skillTitleText.text = string.Format("OFFICIALS");
                flexDesc = officialsSkillDesc;
                flexMultiplierDesc = "TIMER REDUCTION:";
            }

            //Business.
            if (type == MultiplierTypes.CleanMoneyIncrease)
            {
                skillTitleText.text = string.Format("BUSINESSES");
                flexDesc = businessSkillDesc;
                flexMultiplierDesc = "<color=#009600FF>CLEAN MONEY</color> INCREASE:";
            }

            //Partners.
            if (type == MultiplierTypes.DirtyMoneyIncrease)
            {
                skillTitleText.text = string.Format("PARTNERS");
                flexDesc = partnersSkillDesc;
                flexMultiplierDesc = "<color=#DC7800FF>DIRTY MONEY</color> INCREASE:";
            }

            //Estates.
            if (type == MultiplierTypes.ReputationIncrease)
            {
                skillTitleText.text = string.Format("ESTATES");
                flexDesc = estatesSkillDesc;
                flexMultiplierDesc = "<color=#00A0FFFF>REPUTATION</color> INCREASE:";
            }

            //Vehicles.
            if (type == MultiplierTypes.NumberOfProductsIncrease)
            {
                oper = string.Empty;
                currentBonus = ProductManager.Instance.TotalNumberOfProductCanSell;
                nextBonus = currentBonus + bonus;

                skillTitleText.text = string.Format("VEHICLES");
                flexDesc = vehiclesSkillDesc;
                flexMultiplierDesc = "NUMBER OF PRODUCTS:";
            }

            //Weapons.
            if (type == MultiplierTypes.LaunderRateIncrease)
            {
                skillTitleText.text = string.Format("WEAPONS");
                flexDesc = weaponsSkillDesc;
                flexMultiplierDesc = "LAUNDER RATE INCREASE:";
            }

            multiplierText.text = string.Format("{0} {1}{2}", flexMultiplierDesc, currentBonus, oper);
            skillDescText.text = currentLevel != maxLevel ? string.Format("{0}\n\n[{1}{2} -> <color=#FF0000>{3}{4}</color>]", flexDesc, currentBonus, oper, nextBonus, oper) : string.Format("{0}", flexDesc);
            skillCostText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(cost));
            skillCapText.text = string.Format("{0}/{1}", currentLevel, maxLevel);
            CheckIfCanPurchaseDailyLifeSkillUpgrade();
        }

        private void CheckIfCanPurchaseDailyLifeSkillUpgrade()
        {
            if (l_currentLevel == l_maxLevel)
            {
                upgradeSkillButton.interactable = false;
                upgradeSkillButtonText.color = ProductManager.disabledColor;
                upgradeSkillButtonText.text = "MAXED";
                return;
            }
            else
            {
                upgradeSkillButtonText.text = "Upgrade";

                if (CurrencyManager.Instance.CleanMoneyTotal >= l_skillCost)
                {
                    upgradeSkillButton.interactable = true;
                    upgradeSkillButtonText.color = Color.white;
                    hasSkillPointsIndicator.gameObject.SetActive(true);
                    return;
                }

                if (CurrencyManager.Instance.CleanMoneyTotal <= l_skillCost)
                {
                    upgradeSkillButton.interactable = false;
                    upgradeSkillButtonText.color = ProductManager.disabledColor;
                    hasSkillPointsIndicator.gameObject.SetActive(false);
                    return;
                }
            }
        }

        //Attached to the Daily Life Overlay Exit Button via Unity Hierarchy.
        public void ResetSkillDescText()
        {
            EnableSkillDescTextGameObjects(false);
        }

        //Disables default text ("Tap Skill to upgrade [...]") and enables appropriate text/button components.
        private void EnableSkillDescTextGameObjects(bool isEnable)
        {
            tapSkillDescText.gameObject.SetActive(!isEnable);

            multiplierText.gameObject.SetActive(isEnable);
            skillTitleText.gameObject.SetActive(isEnable);
            skillDescText.gameObject.SetActive(isEnable);
            upgradeSkillButton.gameObject.SetActive(isEnable);
            skillCostText.gameObject.SetActive(isEnable);
            skillCapText.gameObject.SetActive(isEnable);
        }

        //Referenced by DailyLifeSkill.cs to add skill-specific listeners (UpgradeSkill method.)
        public Button UpgradeButtonReference()
        {
            return upgradeSkillButton;
        }

        #endregion Custom Methods
    }
}