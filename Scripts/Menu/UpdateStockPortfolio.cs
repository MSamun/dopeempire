using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    [RequireComponent(typeof(Stock))]
    public class UpdateStockPortfolio : MonoBehavRefScript
    {
        public float EarningsFromStock { get; private set; }
        public float TotalSpentOnStock { get; private set; }
        public float TotalValueOfStock { get; private set; }

        #region Variables

        private Stock stockScript;

        private float earningsFromStockPercentage;
        private bool hasStocks;

        #region Game Components

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Stock Portfolio Stock Object", order = 1)]
        [SerializeField] private GameObject stockPortfolioGameObject = null;

        [Header("Earnings Text")]
        [SerializeField] private Text earningsFromStockText = null;

        [Header("Total Spent Text")]
        [SerializeField] private Text totalSpentOnStockText = null;

        [Header("Total Value Text")]
        [SerializeField] private Text totalValueOfStockText = null;

        [Header("Owned Stocks Text")]
        [SerializeField] private Text numberOfStocksOwnedText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            stockScript = GetComponent<Stock>();
        }

        #endregion Initialization

        #region Custom Methods

        public void CalculateStockPortfolioValues()
        {
            CheckIfShouldBeEnabledInStockPortfolio();

            if (hasStocks)
            {
                TotalValueOfStock = stockScript.currentStockValue * stockScript.NumberOfOwnedStocks;
                EarningsFromStock = TotalValueOfStock - TotalSpentOnStock;
                earningsFromStockPercentage = EarningsFromStock / TotalSpentOnStock * 100f;
                UpdateStockPortfolioTextComponents();
                StockMarketManager.Instance.UpdateStockPortfolioTotalEarnings();
            }
        }

        public void UpdatePortfolioTotalSpentOnStocks(float amountSpentOnStocks)
        {
            TotalSpentOnStock += amountSpentOnStocks;
            CalculateStockPortfolioValues();
        }

        private void UpdateStockPortfolioTextComponents()
        {
            if (EarningsFromStock == 0)
            {
                earningsFromStockText.color = Color.white;
            }
            else
            {
                earningsFromStockText.color = EarningsFromStock > 0 ? new Color(0, 0.78f, 0, 1) : new Color(1, 0, 0, 1);     //Green.
            }

            earningsFromStockText.text = string.Format("{0} ({1:#0.00}%)", CurrencyManager.Instance.FormatValues(EarningsFromStock), earningsFromStockPercentage.ToString("F2"));
            totalSpentOnStockText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(TotalSpentOnStock));
            totalValueOfStockText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(TotalValueOfStock));
            numberOfStocksOwnedText.text = string.Format("{0}", stockScript.NumberOfOwnedStocks);
        }

        private void CheckIfShouldBeEnabledInStockPortfolio()
        {
            if (stockScript.NumberOfOwnedStocks > 0)
            {
                hasStocks = true;
                stockPortfolioGameObject.gameObject.SetActive(true);

                if (!StockMarketManager.Instance.listOfOwnedStocks.Contains(this))
                {
                    StockMarketManager.Instance.listOfOwnedStocks.Add(this);
                    StockMarketManager.Instance.UpdateStockPortfolioTotalEarnings();
                }
            }
            else
            {
                hasStocks = false;
                TotalSpentOnStock = 0;
                stockPortfolioGameObject.gameObject.SetActive(false);

                if (StockMarketManager.Instance.listOfOwnedStocks.Contains(this))
                {
                    StockMarketManager.Instance.listOfOwnedStocks.Remove(this);
                    StockMarketManager.Instance.UpdateStockPortfolioTotalEarnings();
                }
            }
        }

        #endregion Custom Methods
    }
}