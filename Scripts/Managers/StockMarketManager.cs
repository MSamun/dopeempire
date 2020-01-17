using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class StockMarketManager : MonoBehavRefScript
    {
        public static StockMarketManager Instance { get; private set; }

        #region Variables

        public event Action OnReadyToRandomizeStocks;

        private int currentWeeks;

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Weeks To Randomize Stocks", order = 1)]
        [SerializeField] private int weeksToRandomizeStocks;

        #region Game Components

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Clean Money Text", order = 1)]
        [SerializeField] private Text cleanMoneyText = null;

        [Header("---------- STOCK PORTFOLIO ----------", order = 0)]
        [Header("Total Earnings Text", order = 1)]
        [SerializeField] private Text totalPortfolioEarningsText = null;

        [Header("No Stocks Owned Text")]
        [SerializeField] private Text noOwnedStocksText = null;

        [Header("List of Owned Stocks")]
        public List<UpdateStockPortfolio> listOfOwnedStocks = new List<UpdateStockPortfolio>();

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
        }

        private void Start()
        {
            UpdateCleanMoneyText();
            UpdateStockPortfolioTotalEarnings();

            CurrencyManager.Instance.OnMoneyValuesChanged += UpdateCleanMoneyText;
            GameManager.Instance.OnWeekPassed += ManageCountdownToRandomizeStocks;
            OnReadyToRandomizeStocks += UpdateStockPortfolioTotalEarnings;
        }

        private void UpdateCleanMoneyText()
        {
            cleanMoneyText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.CleanMoneyTotal));
        }

        #endregion Initialization

        #region Custom Methods

        private void ManageCountdownToRandomizeStocks()
        {
            currentWeeks++;

            if (currentWeeks >= weeksToRandomizeStocks)
            {
                OnReadyToRandomizeStocks?.Invoke();
                currentWeeks = 0;
                return;
            }
        }

        public float CalculateAdjustForStockPrices()
        {
            int adjustCounter = 0;
            float newStockPriceAdjust = 0;

            while (adjustCounter < 5)
            {
                newStockPriceAdjust += UnityEngine.Random.Range(-2f, 0);
                adjustCounter++;
            }

            adjustCounter = 0;

            while (adjustCounter < 5)
            {
                newStockPriceAdjust += UnityEngine.Random.Range(0, 2f);
                adjustCounter++;
            }

            adjustCounter = 0;

            while (adjustCounter < 10)
            {
                newStockPriceAdjust += UnityEngine.Random.Range(-10f, 10f);
                adjustCounter++;
            }

            adjustCounter = 0;

            while (adjustCounter < 3)
            {
                newStockPriceAdjust += UnityEngine.Random.Range(-12f, 12f);
                adjustCounter++;
            }

            return newStockPriceAdjust;
        }

        public void UpdateStockPortfolioTotalEarnings()
        {
            float portfolioEarnings = 0;

            if (listOfOwnedStocks.Count == 0)
            {
                noOwnedStocksText.gameObject.SetActive(true);
                totalPortfolioEarningsText.color = Color.white;
                totalPortfolioEarningsText.text = string.Format("{0}", portfolioEarnings);
                return;
            }

            if (listOfOwnedStocks.Count > 0)
            {
                noOwnedStocksText.gameObject.SetActive(false);

                for (int i = 0; i < listOfOwnedStocks.Count; i++)
                {
                    portfolioEarnings += listOfOwnedStocks[i].EarningsFromStock;
                }
            }

            if (portfolioEarnings == 0)
            {
                totalPortfolioEarningsText.color = Color.white;
            }
            else
            {
                totalPortfolioEarningsText.color = portfolioEarnings > 0 ? new Color(0, 0.78f, 0, 1) : new Color(1, 0, 0, 1);
            }

            totalPortfolioEarningsText.text = string.Format("{0}", CurrencyManager.Instance.FormatValues(portfolioEarnings));
        }

        #endregion Custom Methods
    }
}