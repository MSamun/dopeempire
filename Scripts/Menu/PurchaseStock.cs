using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    [RequireComponent(typeof(Stock))]
    public class PurchaseStock : MonoBehavRefScript
    {
        #region Variables

        private Stock stockScript;
        private UpdateStockPortfolio stockPortfolioScript;

        private string stockName;
        private int numberOfStocksToPurchase;
        private int maxStocksThatCanBePurchased;
        private float currentStockPrice;
        private float totalCostToPurchase;

        private bool canPurchase;
        private bool hasOpenedPurchasePanel;

        #region Game Components

        [Header("---------- STOCK PURCHASE COMPONENTS ----------", order = 0)]
        [Header("Stock Name Desc Text", order = 1)]
        [SerializeField] private Text stockNameText = null;

        [Header("Owned Stocks Desc Text")]
        [SerializeField] private Text ownedStocksText = null;

        [Header("Number of Stocks Text")]
        [SerializeField] private Text numberOfStocksToPurchaseText = null;

        [Header("Cost Text")]
        [SerializeField] private Text totalCostToPurchaseText = null;

        [Header("Add One Stock Button")]
        [SerializeField] private Button addOneStockButton = null;

        [Header("Minus One Stock Button")]
        [SerializeField] private Button minusOneStockButton = null;

        [Header("Add Five Stocks Button")]
        [SerializeField] private Button addFiveStocksButton = null;

        private Text addFiveStocksButtonText = null;

        [Header("Add Ten Stocks Button")]
        [SerializeField] private Button addTenStocksButton = null;

        private Text addTenStocksButtonText = null;

        [Header("Add Max Stocks Button")]
        [SerializeField] private Button addMaxStocksButton = null;

        private Text addMaxStocksButtonText = null;

        [Header("Purchase Stock Exit Button")]
        [SerializeField] private Button exitPurchasePanelButton = null;

        [Header("Purchase Button")]
        [SerializeField] private Button purchaseButton = null;

        private Text purchaseButtonText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            stockScript = GetComponent<Stock>();
            stockPortfolioScript = GetComponent<UpdateStockPortfolio>();
            stockName = stockScript.gameObject.name.ToUpper();

            purchaseButtonText = purchaseButton.GetComponentInChildren<Text>(true);
            addFiveStocksButtonText = addFiveStocksButton.GetComponentInChildren<Text>(true);
            addTenStocksButtonText = addTenStocksButton.GetComponentInChildren<Text>(true);
            addMaxStocksButtonText = addMaxStocksButton.GetComponentInChildren<Text>(true);

            numberOfStocksToPurchase = 0;
            hasOpenedPurchasePanel = false;
        }

        private void Start()
        {
            CheckRequirements();
            CalculateAndUpdateTotalStocksToPurchase();

            CurrencyManager.Instance.OnMoneyValuesChanged += CalculateAndUpdateTotalStocksToPurchase;
        }

        #endregion Initialization

        #region Custom Methods

        public void UpdatePurchaseStocksPanelTextComponents()
        {
            hasOpenedPurchasePanel = true;
            stockNameText.text = string.Format("How many <color=#0096C8>{0}</color> stocks would you like to purchase?", stockName);
            ownedStocksText.text = string.Format("You currently own <color=#FFC800>{0}</color> stocks.", stockScript.NumberOfOwnedStocks);

            CalculateAndUpdateTotalStocksToPurchase();

            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(() => PurchaseStocks());

            exitPurchasePanelButton.onClick.RemoveAllListeners();
            exitPurchasePanelButton.onClick.AddListener(() => ResetExchangeStockPanelBool());

            addOneStockButton.onClick.RemoveAllListeners();
            addOneStockButton.onClick.AddListener(() => IntentToBuyFewStocks(1));

            minusOneStockButton.onClick.RemoveAllListeners();
            minusOneStockButton.onClick.AddListener(() => IntentToBuyFewStocks(-1));

            addFiveStocksButton.onClick.RemoveAllListeners();
            addFiveStocksButton.onClick.AddListener(() => IntentToBuyFewStocks(5));

            addTenStocksButton.onClick.RemoveAllListeners();
            addTenStocksButton.onClick.AddListener(() => IntentToBuyFewStocks(10));

            addMaxStocksButton.onClick.RemoveAllListeners();
            addMaxStocksButton.onClick.AddListener(() => IntentToBuyMaxStocks());
        }

        private void ResetExchangeStockPanelBool()
        {
            hasOpenedPurchasePanel = false;
            numberOfStocksToPurchase = 0;
        }

        private void CheckRequirements()
        {
            if (hasOpenedPurchasePanel)
            {
                if (CurrencyManager.Instance.CleanMoneyTotal >= totalCostToPurchase && numberOfStocksToPurchase > 0)
                {
                    canPurchase = true;
                    purchaseButton.interactable = true;
                    purchaseButtonText.color = Color.white;
                }

                if (numberOfStocksToPurchase <= 0 || CurrencyManager.Instance.CleanMoneyTotal < totalCostToPurchase)
                {
                    canPurchase = false;
                    purchaseButton.interactable = false;
                    purchaseButtonText.color = ProductManager.disabledColor;
                }
            }
        }

        private void PurchaseStocks()
        {
            CalculateAndUpdateTotalStocksToPurchase();

            if (canPurchase)
            {
                CurrencyManager.Instance.TotalIncomeGate(true, Income.CleanMoney, totalCostToPurchase);
                stockScript.SetPurchasedStockAndNumberOfOwnedStocksValues(currentStockPrice, numberOfStocksToPurchase);
                stockPortfolioScript.UpdatePortfolioTotalSpentOnStocks(totalCostToPurchase);
                hasOpenedPurchasePanel = false;
                numberOfStocksToPurchase = 0;
            }
        }

        #region Calculate Stock Purchase

        public void CalculateAndUpdateTotalStocksToPurchase()
        {
            if (hasOpenedPurchasePanel)
            {
                currentStockPrice = stockScript.currentStockValue;
                totalCostToPurchase = currentStockPrice * numberOfStocksToPurchase;

                numberOfStocksToPurchaseText.text = string.Format("{0}", numberOfStocksToPurchase);
                totalCostToPurchaseText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(totalCostToPurchase));

                CheckRequirements();
                CheckIfStockPurchaseButtonsShouldBeDisabled();
            }
        }

        public void IntentToBuyFewStocks(int numberToPurchase)
        {
            numberOfStocksToPurchase += numberToPurchase;
            CalculateAndUpdateTotalStocksToPurchase();
        }

        public void IntentToBuyMaxStocks()
        {
            CalculateMaxStocksThatCanBePurchased();

            numberOfStocksToPurchase = maxStocksThatCanBePurchased;

            CalculateAndUpdateTotalStocksToPurchase();
        }

        private void CalculateMaxStocksThatCanBePurchased()
        {
            maxStocksThatCanBePurchased = Mathf.FloorToInt(CurrencyManager.Instance.CleanMoneyTotal / currentStockPrice);
        }

        private void CheckIfStockPurchaseButtonsShouldBeDisabled()
        {
            CalculateMaxStocksThatCanBePurchased();
            int stocksThatCanBePurchased = maxStocksThatCanBePurchased - numberOfStocksToPurchase;      //i.e. (20 stocks that can be purchased - 0 stocks that you wanna purchase).

            //The amount of stocks that you can buy is equal to the maximum stocks that can be purchased (meaning you got 0 stocks in the cart).
            minusOneStockButton.interactable = stocksThatCanBePurchased == maxStocksThatCanBePurchased ? false : true;

            //If you can buy at least one stock, enable button. Otherwise, disable it.
            addOneStockButton.interactable = stocksThatCanBePurchased >= 1 ? true : false;

            if (stocksThatCanBePurchased >= 5)
            {
                addFiveStocksButton.interactable = true;
                addFiveStocksButtonText.color = Color.white;
            }
            else
            {
                addFiveStocksButton.interactable = false;
                addFiveStocksButtonText.color = ProductManager.disabledColor;
            }

            if (stocksThatCanBePurchased >= 10)
            {
                addTenStocksButton.interactable = true;
                addTenStocksButtonText.color = Color.white;
            }
            else
            {
                addTenStocksButton.interactable = false;
                addTenStocksButtonText.color = ProductManager.disabledColor;
            }
        }

        #endregion Calculate Stock Purchase

        #endregion Custom Methods
    }
}