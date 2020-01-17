using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class ResearchManager : MonoBehavRefScript
    {
        public static ResearchManager Instance { get; private set; }

        #region Variables

        //Used to reset the position of the Research Screen to the center after the user exits the screen.
        private RectTransform researchScreenTransform;

        private Vector2 defaultOffSetMin = new Vector2(-300f, -1375f);
        private Vector2 defaultOffSetMax = new Vector2(300f, 125f);

        //Local references from Research Skills. Used to check current state of Upgrade Button, and enable/disable accordingly.
        private float l_skillCost;

        private int l_currentLevel;
        private int l_maxLevel;

        #region Research Summaries

        private string productQualityDesc = "Research the next quality of Product, increasing all Product income.";
        private string allProductIncomeDesc = "Increase all Product's <color=#DC7800FF>Dirty Money</color>, <color=#009600FF>Clean Money</color>, and <color=#00A0FFFF>Reputation</color>.";
        private string adBonusDesc = "Increase the bonus amount you get from watching ads.";
        private string adLengthDesc = "Increase the length of bonuses you get from watching ads.";
        private string experienceDesc = "Increase the amount of <color=#DCB400>Experience</color> you earn from all sources.";
        private string launderRateDesc = "Increases the conversion rates for both <color=#009600FF>Clean Money</color> and <color=#00A0FFFF>Reputation</color>.";
        private string secondsBetweenWeeksDesc = "Reduce the amount of time for an in-game week to pass by.";
        private string moneyButtonDesc = "Earn an additional amount of Income from the Money Button.";

        #endregion Research Summaries

        #region Game Components

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Research Screen", order = 1)]
        [SerializeField] private GameObject researchScreen = null;

        [Header("Reputation Total Text")]
        [SerializeField] private Text reputationTotalText = null;

        [Header("---------- UPGRADE COMPONENTS ----------", order = 0)]
        [Header("Research Skill Description Object", order = 1)]
        [SerializeField] private GameObject skillDescObject = null;

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

            researchScreenTransform = researchScreen.GetComponent<RectTransform>();
            upgradeSkillButtonText = upgradeSkillButton.GetComponentInChildren<Text>(true);

            l_skillCost = 0;
            l_maxLevel = 0;
            l_currentLevel = 0;
        }

        private void Start()
        {
            UpdateReputationTotalText();
            CurrencyManager.Instance.OnMoneyValuesChanged += UpdateReputationTotalText;
        }

        private void Update()
        {
            if (researchScreen.activeInHierarchy && Input.touchCount > 0)
            {
                skillDescObject.gameObject.SetActive(false);
            }
        }

        private void UpdateReputationTotalText()
        {
            reputationTotalText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.ReputationTotal));
            CheckIfCanPurchaseResearchSkillUpgrade();
        }

        #endregion Initialization

        #region Custom Methods

        public void ResetResearchScreenPosition()
        {
            if (researchScreenTransform.offsetMin != defaultOffSetMin || researchScreenTransform.offsetMax != defaultOffSetMax)
            {
                researchScreenTransform.offsetMin = defaultOffSetMin;
                researchScreenTransform.offsetMax = defaultOffSetMax;
                return;
            }
        }

        /// <summary>
        /// Each Research Skill gives a specific increase (or decrease) for a certain income (or value) in the game.
        /// </summary>
        /// <param name="type">What type of multiplier is this skill affecting?</param>
        /// <param name="cost">What's the cost to purchase this upgrade?</param>
        /// <param name="bonus">How much does this upgrade increase the multiplier by?</param>
        /// <param name="currentLevel">How many times have you already purchased this upgrade?</param>
        /// <param name="maxLevel">What's the maximum amount of times you can purchase this upgrade?</param>
        public void ChangeResearchSkillDescTextComponents(MultiplierTypes type, float cost, float bonus, int currentLevel, int maxLevel)
        {
            EnableSkillDescTextGameObjects(true);

            l_currentLevel = currentLevel;
            l_maxLevel = maxLevel;
            l_skillCost = cost;

            string flexDesc = string.Empty;
            float currentBonus = Mathf.Round(bonus * 100f) * currentLevel;
            float nextBonus = currentBonus + Mathf.Round(bonus * 100f);
            string oper = "%";

            if (type == MultiplierTypes.ProductQuality)
            {
                skillTitleText.text = string.Format("PRODUCT QUALITY - {0}[{1}]", MultiplierManager.Instance.NextResearchedProduct.ToString().ToUpper(), MultiplierManager.Instance.NextResearchedGrade.ToString().ToUpper());
                flexDesc = productQualityDesc;
            }

            if (type == MultiplierTypes.AllProductIncomeIncrease)
            {
                skillTitleText.text = string.Format("INCREASE PRODUCT INCOME");
                flexDesc = allProductIncomeDesc;
            }

            if (type == MultiplierTypes.SecondsBetweenWeeksReduction)
            {
                skillTitleText.text = string.Format("REDUCE SECONDS BETWEEN WEEKS");
                flexDesc = secondsBetweenWeeksDesc;
                oper = "s";
                currentBonus = GameManager.Instance.SecondsBetweenWeeks;
                nextBonus = currentBonus - bonus;
            }

            if (type == MultiplierTypes.LaunderRateIncrease)
            {
                skillTitleText.text = string.Format("INCREASE LAUNDER RATES");
                flexDesc = launderRateDesc;
            }

            if (type == MultiplierTypes.ExperienceIncrease)
            {
                skillTitleText.text = string.Format("INCREASE EXPERIENCE");
                flexDesc = experienceDesc;
            }

            if (type == MultiplierTypes.MoneyButtonClickIncrease)
            {
                skillTitleText.text = string.Format("INCREASE MONEY BUTTON BONUS");
                flexDesc = moneyButtonDesc;
            }

            if (type == MultiplierTypes.AdBonusIncrease)
            {
                skillTitleText.text = string.Format("INCREASE AD BONUS");
                flexDesc = adBonusDesc;
            }

            if (type == MultiplierTypes.AdLengthIncrease)
            {
                skillTitleText.text = string.Format("INCREASE AD BONUS LENGTH");
                flexDesc = adLengthDesc;
            }

            skillDescText.text = currentLevel != maxLevel ? string.Format("{0}\n[{1}{2} -> <color=#FF0000>{3}{4}</color>]", flexDesc, currentBonus, oper, nextBonus, oper) : string.Format("{0}\n[{1}{2}]", flexDesc, currentBonus, oper);
            skillCostText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(cost));
            skillCapText.text = string.Format("{0}/{1}", currentLevel, maxLevel);
            CheckIfCanPurchaseResearchSkillUpgrade();
        }

        private void CheckIfCanPurchaseResearchSkillUpgrade()
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
                upgradeSkillButtonText.text = "Research";

                if (CurrencyManager.Instance.ReputationTotal >= l_skillCost)
                {
                    upgradeSkillButton.interactable = true;
                    upgradeSkillButtonText.color = Color.white;
                    //hasSkillPointsIndicator.gameObject.SetActive(true);
                    return;
                }

                if (CurrencyManager.Instance.ReputationTotal <= l_skillCost)
                {
                    upgradeSkillButton.interactable = false;
                    upgradeSkillButtonText.color = ProductManager.disabledColor;
                    //hasSkillPointsIndicator.gameObject.SetActive(false);
                    return;
                }
            }
        }

        //Attached to the Research Screen Exit Button via Unity Hierarchy.
        public void ResetSkillDescText()
        {
            EnableSkillDescTextGameObjects(false);
        }

        //Disables/Enables appropriate text/button components.
        private void EnableSkillDescTextGameObjects(bool isEnable)
        {
            skillTitleText.gameObject.SetActive(isEnable);
            skillDescText.gameObject.SetActive(isEnable);
            upgradeSkillButton.gameObject.SetActive(isEnable);
            skillCostText.gameObject.SetActive(isEnable);
            skillCapText.gameObject.SetActive(isEnable);
        }

        //Referenced by ResearchSkill.cs to add skill-specific listeners (UpgradeSkill method.)
        public Button UpgradeButtonReference()
        {
            return upgradeSkillButton;
        }

        #endregion Custom Methods
    }
}