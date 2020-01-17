using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class Stock : MonoBehavRefScript
    {
        public float ValueOfLastPurchasedStock { get; private set; }
        public int NumberOfOwnedStocks { get; private set; }
        public float StockChangePercentage { get; private set; }
        public float StockChangeValue { get; private set; }

        #region Variables

        [HideInInspector] public UpdateStockPortfolio stockPortfolioScript;
        private PurchaseStock purchaseStockScript;

        private bool hasStocks;

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Current/Min/Max Stock Value", order = 1)]
        public float currentStockValue;

        [SerializeField] private float minStockValue;
        [SerializeField] private float maxStockValue;

        #region Game Components

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Sell Stock Script", order = 1)]
        [SerializeField] private SellStock sellStockScript;

        [Header("Stock Value Text")]
        [SerializeField] private Text currentStockValueText = null;

        [Header("Stock Change Panel")]
        [SerializeField] private GameObject stockChangePanel = null;

        [Header("Stock Change Percentage")]
        [SerializeField] private Text stockChangePercentageText = null;

        [Header("Stock Change Value Text")]
        [SerializeField] private Text stockChangeValueText = null;

        [Header("Owned Stocks Text")]
        [SerializeField] private Text ownedStocksText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            stockPortfolioScript = GetComponent<UpdateStockPortfolio>();
            purchaseStockScript = GetComponent<PurchaseStock>();

            currentStockValue = 1000;
            StockChangePercentage = 0;
            StockChangeValue = 0;
            NumberOfOwnedStocks = 0;
        }

        private void Start()
        {
            UpdateStockValueTextComponents();
            StockMarketManager.Instance.OnReadyToRandomizeStocks += AdjustStockPrices;
        }

        #endregion Initialization

        #region Custom Methods

        public void UpdateStockValueTextComponents()
        {
            currentStockValueText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(currentStockValue));
            CheckIfShouldEnableStockChangePanel();

            if (hasStocks)
            {
                CalculateStockChangePercentageAndRawValue();
                stockChangeValueText.text = string.Format(CurrencyManager.Instance.FormatValues(StockChangeValue));
                stockChangePercentageText.text = string.Format("{0:#0.00}%", StockChangePercentage.ToString("F2"));
                ownedStocksText.text = string.Format("{0}", NumberOfOwnedStocks);
            }
        }

        public void SetPurchasedStockAndNumberOfOwnedStocksValues(float purchasedStock, int ownedStocks)
        {
            ValueOfLastPurchasedStock = purchasedStock;
            NumberOfOwnedStocks += ownedStocks;
            UpdateStockValueTextComponents();
            return;
        }

        public void SetSoldStockAndNumberOfOwnedStockValues(int ownedStocks)
        {
            NumberOfOwnedStocks -= ownedStocks;
            UpdateStockValueTextComponents();
            return;
        }

        private void AdjustStockPrices()
        {
            currentStockValue += StockMarketManager.Instance.CalculateAdjustForStockPrices();
            currentStockValue = Mathf.RoundToInt(currentStockValue);

            if (currentStockValue < minStockValue)
            {
                currentStockValue = minStockValue;
            }

            if (currentStockValue > maxStockValue)
            {
                currentStockValue = maxStockValue;
            }

            if (hasStocks)
            {
                CalculateStockChangePercentageAndRawValue();
                stockPortfolioScript.CalculateStockPortfolioValues();
            }

            UpdateStockValueTextComponents();
            purchaseStockScript.CalculateAndUpdateTotalStocksToPurchase();
            sellStockScript.CalculateAndUpdateTotalStocksToSell();
        }

        private void CheckIfShouldEnableStockChangePanel()
        {
            if (NumberOfOwnedStocks > 0)
            {
                hasStocks = true;
                stockChangePanel.gameObject.SetActive(true);
            }
            else
            {
                hasStocks = false;
                stockChangePanel.gameObject.SetActive(false);
            }
        }

        private void CalculateStockChangePercentageAndRawValue()
        {
            StockChangeValue = currentStockValue - ValueOfLastPurchasedStock;
            StockChangePercentage = StockChangeValue / ValueOfLastPurchasedStock * 100f;

            if (StockChangeValue == 0)
            {
                stockChangeValueText.color = Color.white;
                stockChangePercentageText.color = Color.white;
            }
            else if (StockChangeValue > 0)
            {
                stockChangeValueText.color = new Color(0, 0.78f, 0, 1);     //Green.
                stockChangePercentageText.color = new Color(0, 0.78f, 0, 1);
            }
            else
            {
                stockChangeValueText.color = new Color(1, 0, 0, 1);         //Red.
                stockChangePercentageText.color = new Color(1, 0, 0, 1);
            }
        }

        #endregion Custom Methods
    }
}