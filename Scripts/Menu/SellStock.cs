using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class SellStock : MonoBehavRefScript
    {
        #region Variables

        private string stockName;
        private int numberOfStocksToSell;
        private int maxStocksThatCanBeSold;
        private float currentStockPrice;
        private float totalSellCost;

        private bool canSell;
        private bool hasOpenedSellPanel;

        #region Game Components

        [Header("---------- STOCK SELL COMPONENTS ----------", order = 0)]
        [Header("Stock To Sell Script", order = 1)]
        [SerializeField] private Stock stockScript;

        [Header("Stock Name Desc Text")]
        [SerializeField] private Text stockNameText = null;

        [Header("Owned Stocks Desc Text")]
        [SerializeField] private Text ownedStocksText = null;

        [Header("Number of Stocks Text")]
        [SerializeField] private Text numberOfStocksToSellText = null;

        [Header("Cost Text")]
        [SerializeField] private Text totalSellCostText = null;

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

        [Header("Sell Stock Exit Button")]
        [SerializeField] private Button exitSellPanelButton = null;

        [Header("Sell Button")]
        [SerializeField] private Button sellButton = null;

        private Text sellButtonText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            stockName = stockScript.gameObject.name.ToUpper();

            sellButtonText = sellButton.GetComponentInChildren<Text>(true);
            addFiveStocksButtonText = addFiveStocksButton.GetComponentInChildren<Text>(true);
            addTenStocksButtonText = addTenStocksButton.GetComponentInChildren<Text>(true);
            addMaxStocksButtonText = addMaxStocksButton.GetComponentInChildren<Text>(true);

            numberOfStocksToSell = 0;
            hasOpenedSellPanel = false;
        }

        private void Start()
        {
            CheckRequirements();
            CalculateAndUpdateTotalStocksToSell();

            CurrencyManager.Instance.OnMoneyValuesChanged += CalculateAndUpdateTotalStocksToSell;
        }

        #endregion Initialization

        public void UpdateSellStocksPanelTextComponents()
        {
            hasOpenedSellPanel = true;
            stockNameText.text = string.Format("How many <color=#0096C8>{0}</color> stocks would you like to sell?", stockName);
            ownedStocksText.text = string.Format("You currently own <color=#FFC800>{0}</color> stocks.", stockScript.NumberOfOwnedStocks);

            CalculateAndUpdateTotalStocksToSell();

            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(() => SellStocks());

            exitSellPanelButton.onClick.RemoveAllListeners();
            exitSellPanelButton.onClick.AddListener(() => ResetExchangeStockPanelBool());

            addOneStockButton.onClick.RemoveAllListeners();
            addOneStockButton.onClick.AddListener(() => IntentToSellFewStocks(1));

            minusOneStockButton.onClick.RemoveAllListeners();
            minusOneStockButton.onClick.AddListener(() => IntentToSellFewStocks(-1));

            addFiveStocksButton.onClick.RemoveAllListeners();
            addFiveStocksButton.onClick.AddListener(() => IntentToSellFewStocks(5));

            addTenStocksButton.onClick.RemoveAllListeners();
            addTenStocksButton.onClick.AddListener(() => IntentToSellFewStocks(10));

            addMaxStocksButton.onClick.RemoveAllListeners();
            addMaxStocksButton.onClick.AddListener(() => IntentToSellMaxStocks());
        }

        private void ResetExchangeStockPanelBool()
        {
            hasOpenedSellPanel = false;
        }

        private void CheckRequirements()
        {
            if (hasOpenedSellPanel)
            {
                if (stockScript.NumberOfOwnedStocks >= numberOfStocksToSell && numberOfStocksToSell > 0)
                {
                    canSell = true;
                    sellButton.interactable = true;
                    sellButtonText.color = Color.white;
                }

                if (stockScript.NumberOfOwnedStocks < numberOfStocksToSell || numberOfStocksToSell <= 0)
                {
                    canSell = false;
                    sellButton.interactable = false;
                    sellButtonText.color = ProductManager.disabledColor;
                }
            }
        }

        private void SellStocks()
        {
            CalculateAndUpdateTotalStocksToSell();

            if (canSell)
            {
                CurrencyManager.Instance.TotalIncomeGate(false, Income.CleanMoney, totalSellCost);
                stockScript.SetSoldStockAndNumberOfOwnedStockValues(numberOfStocksToSell);
                stockScript.stockPortfolioScript.UpdatePortfolioTotalSpentOnStocks(totalSellCost * -1);
                hasOpenedSellPanel = false;
                numberOfStocksToSell = 0;
            }
        }

        public void CalculateAndUpdateTotalStocksToSell()
        {
            if (hasOpenedSellPanel)
            {
                currentStockPrice = stockScript.currentStockValue;
                totalSellCost = currentStockPrice * numberOfStocksToSell;

                numberOfStocksToSellText.text = string.Format("{0}", numberOfStocksToSell);
                totalSellCostText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(totalSellCost));

                CheckRequirements();
                CheckIfStockSellButtonsShouldBeDisabled();
            }
        }

        public void IntentToSellFewStocks(int numberToSell)
        {
            numberOfStocksToSell += numberToSell;
            CalculateAndUpdateTotalStocksToSell();
        }

        public void IntentToSellMaxStocks()
        {
            CalculateMaxStocksThatCanBePurchased();

            numberOfStocksToSell = maxStocksThatCanBeSold;

            CalculateAndUpdateTotalStocksToSell();
        }

        private void CalculateMaxStocksThatCanBePurchased()
        {
            maxStocksThatCanBeSold = stockScript.NumberOfOwnedStocks;
        }

        private void CheckIfStockSellButtonsShouldBeDisabled()
        {
            CalculateMaxStocksThatCanBePurchased();
            int stocksThatCanBeSold = maxStocksThatCanBeSold - numberOfStocksToSell;      //i.e. (20 stocks that can be purchased - 0 stocks that you wanna purchase).

            //The amount of stocks that you can buy is equal to the maximum stocks that can be purchased (meaning you got 0 stocks in the cart).
            minusOneStockButton.interactable = stocksThatCanBeSold == maxStocksThatCanBeSold ? false : true;

            //If you can buy at least one stock, enable button. Otherwise, disable it.
            addOneStockButton.interactable = stocksThatCanBeSold >= 1 ? true : false;

            if (stocksThatCanBeSold >= 5)
            {
                addFiveStocksButton.interactable = true;
                addFiveStocksButtonText.color = Color.white;
            }
            else
            {
                addFiveStocksButton.interactable = false;
                addFiveStocksButtonText.color = ProductManager.disabledColor;
            }

            if (stocksThatCanBeSold >= 10)
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
    }
}