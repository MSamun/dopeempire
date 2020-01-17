using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class LaunderIncome : MonoBehavRefScript
    {
        #region Variables

        // These two variables are used to calculate the actual conversion rates using the base conversion rates (line 20 and line 23)
        // plus the launder income rate increase multiplier.

        private float cleanMoneyCalculatedConversionRate;
        private float reputationCalculatedConversionRate;
        private float totalConversionAmount;
        private bool isConvertingToCleanMoney;

        [Header("----------VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Clean Money Base Conversion Rate", order = 1)]
        [SerializeField] private float cleanMoneyBaseConversionRate = 0.20f;

        [Header("Reputation BaseConversion Rate", order = 1)]
        [SerializeField] private float reputationBaseConversionRate = 0.15f;

        #region Game Components

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Conversion Rate Text", order = 1)]
        [SerializeField] private Text conversionRateText = null;

        [Header("Dirty Money Text")]
        [SerializeField] private Text dirtyMoneyText = null;

        [Header("Clean Money Text/Object")]
        [SerializeField] private Text cleanMoneyText = null;

        [SerializeField] private GameObject cleanMoneyAmount = null;

        [Header("Reputation Text/Object")]
        [SerializeField] private Text reputationText = null;

        [SerializeField] private GameObject reputationAmount = null;

        [Header("Launder Button")]
        [SerializeField] private Button launderButton = null;

        private Text launderButtonText = null;

        [Header("Change Type of Conversion Button")]
        [SerializeField] private Image conversionTypeButtonImage = null;

        [Header("Type of Conversion Sprites")]
        [SerializeField] private Sprite convertToRepSprite = null;

        [SerializeField] private Sprite convertToCleanMoneySprite = null;
        [SerializeField] private GameObject hasSkillPointsIndicator = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            isConvertingToCleanMoney = true;
            launderButtonText = launderButton.GetComponentInChildren<Text>();
        }

        private void Start()
        {
            CalculateLaunderIncomeValues();
            ApplyConversionRateMultiplier();
            CurrencyManager.Instance.OnMoneyValuesChanged += CalculateLaunderIncomeValues;
            MultiplierManager.Instance.OnPurchaseMultiplier += ApplyConversionRateMultiplier;
        }

        private void InitializeConversionRateTextValues()
        {
            if (isConvertingToCleanMoney)
            {
                conversionRateText.text = string.Format("{0}%", Mathf.Round(cleanMoneyCalculatedConversionRate * 100f));
                return;
            }

            conversionRateText.text = string.Format("{0}%", Mathf.Round(reputationCalculatedConversionRate * 100f));
        }

        #endregion Initialization

        #region Custom Methods

        public void ExchangeIncomeValues()
        {
            CheckIfCanLaunder();
            CalculateLaunderIncomeValues();

            if (totalConversionAmount > 0)
            {
                if (isConvertingToCleanMoney)
                {
                    CurrencyManager.Instance.LaunderIncomeGate(Income.CleanMoney, totalConversionAmount);
                }
                else
                {
                    CurrencyManager.Instance.LaunderIncomeGate(Income.Reputation, totalConversionAmount);
                }

                CurrencyManager.Instance.TotalIncomeGate(true, Income.DirtyMoney, CurrencyManager.Instance.DirtyMoneyTotal);
                CalculateLaunderIncomeValues();
            }
        }

        public void ChangeConversionType()
        {
            isConvertingToCleanMoney = !isConvertingToCleanMoney;

            //If the current conversion type is Clean Money, change conversion type to Reputation.
            if (isConvertingToCleanMoney)
            {
                conversionTypeButtonImage.sprite = convertToRepSprite;
                cleanMoneyAmount.gameObject.SetActive(true);
                reputationAmount.gameObject.SetActive(false);
            }
            else
            {
                conversionTypeButtonImage.sprite = convertToCleanMoneySprite;
                cleanMoneyAmount.gameObject.SetActive(false);
                reputationAmount.gameObject.SetActive(true);
            }

            InitializeConversionRateTextValues();
            CalculateLaunderIncomeValues();
        }

        private void CalculateLaunderIncomeValues()
        {
            dirtyMoneyText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.DirtyMoneyTotal));

            if (isConvertingToCleanMoney)
            {
                totalConversionAmount = Mathf.RoundToInt(CurrencyManager.Instance.DirtyMoneyTotal * cleanMoneyCalculatedConversionRate);
                cleanMoneyText.text = string.Format(CurrencyManager.Instance.FormatValues(totalConversionAmount));
            }
            else
            {
                totalConversionAmount = Mathf.RoundToInt(CurrencyManager.Instance.DirtyMoneyTotal * reputationCalculatedConversionRate);
                reputationText.text = string.Format(CurrencyManager.Instance.FormatValues(Mathf.FloorToInt(totalConversionAmount)));
            }

            CheckIfCanLaunder();
        }

        private void CheckIfCanLaunder()
        {
            if (totalConversionAmount > 0)
            {
                launderButton.interactable = true;
                launderButtonText.color = Color.white;
                hasSkillPointsIndicator.gameObject.SetActive(true);
                return;
            }
            else
            {
                launderButton.interactable = false;
                launderButtonText.color = ProductManager.disabledColor;
                hasSkillPointsIndicator.gameObject.SetActive(false);
                return;
            }
        }

        private void ApplyConversionRateMultiplier()
        {
            // Default launder rates for Clean Money and Reputation are 20% and 15%, respectively.
            // I'm using the addiditive route rather than the multiplicative route, hence the code below.

            cleanMoneyCalculatedConversionRate = cleanMoneyBaseConversionRate + MultiplierManager.Instance.LaunderRateIncreaseMultiplier;
            reputationCalculatedConversionRate = reputationBaseConversionRate + MultiplierManager.Instance.LaunderRateIncreaseMultiplier;

            InitializeConversionRateTextValues();
            CalculateLaunderIncomeValues();
            CheckIfCanLaunder();
            return;
        }

        #endregion Custom Methods
    }
}