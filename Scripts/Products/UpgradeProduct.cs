using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    [RequireComponent(typeof(ProductInformation))]
    public class UpgradeProduct : MonoBehavRefScript
    {
        #region Variables

        private ProductInformation prodInfoScript;

        private float timerNextValue;
        private float dirtyMoneyNextValue;
        private float cleanMoneyNextValue;
        private float reputationNextValue;
        private bool canPurchase;
        private bool hasOpenedUpgradePanel;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("DM Upgrade Multiplier", order = 1)]
        [SerializeField] private float dirtyMoneyMultiplier = 1.07f;

        [Header("Timer Upgrade Multiplier")]
        [SerializeField] private float timerMultiplier = 1.035f;

        [Header("Upgrade Cost", order = 1)]
        [SerializeField] private float upgradeCost = 100;

        [Header("Upgrade Cost Multiplier", order = 1)]
        [SerializeField] private float upgradeCostMultiplier = 1.07f;

        [Header("Upgrade Indicator")]
        [SerializeField] private GameObject upgradeIndicator = null;

        private Button upgradeButton = null;
        private Text upgradeButtonText = null;
        private Button upgradeExitButton = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            prodInfoScript = GetComponent<ProductInformation>();
            upgradeButton = ProductManager.Instance.UpgradeButtonReference();
            upgradeButtonText = upgradeButton.GetComponentInChildren<Text>();
            upgradeExitButton = ProductManager.Instance.UpgradeExitButtonReference();
        }

        private void Start()
        {
            SetProductInformationUpgradeCostViaJson();
            CheckRequirements();
            CurrencyManager.Instance.OnMoneyValuesChanged += CheckRequirements;
        }

        private void SetProductInformationUpgradeCostViaJson()
        {
            int level = prodInfoScript.productInformation.level;

            for (int i = 1; i < level; i++)
            {
                upgradeCost = Mathf.RoundToInt(upgradeCost * upgradeCostMultiplier);
            }
        }

        #endregion Initialization

        #region Custom Methods

        //This method is executed through the Product #[NUMBER]'s Level button. The name is self-explanatory.
        public void ConfirmUpgradeProductValues()
        {
            hasOpenedUpgradePanel = true;

            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => LevelUp());

            upgradeExitButton.onClick.RemoveAllListeners();
            upgradeExitButton.onClick.AddListener(() => ResetUpgradePanelBool());

            CalculateProductUpgradeValues();
            CheckRequirements();
        }

        public void LevelUp()
        {
            CheckRequirements();

            if (canPurchase)
            {
                CurrencyManager.Instance.TotalIncomeGate(true, Income.DirtyMoney, upgradeCost);

                prodInfoScript.productInformation.timeToSell = timerNextValue;
                prodInfoScript.productInformation.dirtyMoney = dirtyMoneyNextValue;
                prodInfoScript.productInformation.cleanMoney = cleanMoneyNextValue;
                prodInfoScript.productInformation.reputation = reputationNextValue;
                prodInfoScript.productInformation.level++;

                upgradeCost = Mathf.RoundToInt(upgradeCost * upgradeCostMultiplier);

                prodInfoScript.UpdateAndDisplayProductInformationValuesAndText();
                CalculateProductUpgradeValues();
                CheckRequirements();
                return;
            }
        }

        private void ResetUpgradePanelBool()
        {
            hasOpenedUpgradePanel = false;
        }

        private void CheckRequirements()
        {
            if (hasOpenedUpgradePanel)
            {
                if (CurrencyManager.Instance.DirtyMoneyTotal >= upgradeCost)
                {
                    canPurchase = true;
                    upgradeButton.interactable = true;
                    upgradeButtonText.color = Color.white;
                }

                if (CurrencyManager.Instance.DirtyMoneyTotal < upgradeCost)
                {
                    canPurchase = false;
                    upgradeButton.interactable = false;
                    upgradeButtonText.color = ProductManager.disabledColor;
                }
            }

            if (CurrencyManager.Instance.DirtyMoneyTotal >= upgradeCost)
            {
                upgradeIndicator.gameObject.SetActive(true);
            }

            if (CurrencyManager.Instance.DirtyMoneyTotal < upgradeCost)
            {
                upgradeIndicator.gameObject.SetActive(false);
            }
        }

        private void CalculateProductUpgradeValues()
        {
            //If current Product Level is less than 10, use the base multiplier.
            if (prodInfoScript.productInformation.level < 25)
            {
                dirtyMoneyNextValue = prodInfoScript.productInformation.dirtyMoney * dirtyMoneyMultiplier;
            }

            //If current Product Level is between 25 - 50, increase the multiplier by 0.01.
            if (prodInfoScript.productInformation.level >= 25 && prodInfoScript.productInformation.level < 50)
            {
                dirtyMoneyNextValue = prodInfoScript.productInformation.dirtyMoney * (dirtyMoneyMultiplier + 0.01f);
            }

            //If current Product Level is between 50 - 100, increase the multiplier by 0.02.
            if (prodInfoScript.productInformation.level >= 50 && prodInfoScript.productInformation.level < 100)
            {
                dirtyMoneyNextValue = prodInfoScript.productInformation.dirtyMoney * (dirtyMoneyMultiplier + 0.02f);
            }

            //If current Product Level is between 100 - 200, increase the multiplier by 0.03.
            if (prodInfoScript.productInformation.level >= 100 && prodInfoScript.productInformation.level < 200)
            {
                dirtyMoneyNextValue = prodInfoScript.productInformation.dirtyMoney * (dirtyMoneyMultiplier + 0.03f);
            }

            //If current Product Level is greater than 200, increase the multiplier by 0.04
            if (prodInfoScript.productInformation.level >= 200)
            {
                dirtyMoneyNextValue = prodInfoScript.productInformation.dirtyMoney * (dirtyMoneyMultiplier + 0.04f);
            }

            timerNextValue = prodInfoScript.productInformation.timeToSell * timerMultiplier;
            cleanMoneyNextValue = dirtyMoneyNextValue * MultiplierManager.Instance.CleanMoneyIncreaseMultiplier;
            reputationNextValue = dirtyMoneyNextValue * MultiplierManager.Instance.ReputationIncreaseMultiplier;

            ProductManager.Instance.UpdateUpgradePanelTextValues(prodInfoScript.productInformation.productID, prodInfoScript.productInformation.level,
            dirtyMoneyNextValue, cleanMoneyNextValue, timerNextValue, reputationNextValue, upgradeCost);
        }

        #endregion Custom Methods
    }
}