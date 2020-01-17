using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public enum Product { Weed, Coke, Shrooms, Heroin, Pills, Meth }

    public enum Grade { A, AA, AAA, AAAA }

    public class ProductManager : MonoBehavRefScript
    {
        //Singleton.
        public static ProductManager Instance { get; private set; }

        public int TotalNumberOfProductCanSell { get; private set; }
        public int CurrentNumberOfOwnedProducts { get; private set; }
        public float TotalProductDirtyMoneyRevenue { get; private set; }
        public float TotalProductCleanMoneyRevenue { get; private set; }
        public float TotalProductReputation { get; private set; }
        public float TotalTimeToSell { get; private set; }
        public int[] ProductLevelValues { get; private set; } = new int[10];
        public float[] ProductDirtyMoneyValues { get; private set; } = new float[10];
        public float[] ProductCleanMoneyValues { get; private set; } = new float[10];
        public float[] ProductReputationValues { get; private set; } = new float[10];
        public float[] ProductTimerValues { get; private set; } = new float[10];

        #region Variables

        public static readonly Color disabledColor = new Color(0.6f, 0.6f, 0.6f, 1);

        #region Game Components

        [Header("---------- PRODUCT COMPONENTS ----------", order = 0)]
        [Header("#/# [Products Can Sell at a Time]", order = 1)]
        [SerializeField] private Text productCanSellText = null;

        [Header("List of Product Purchase Objects")]
        [SerializeField] private List<GameObject> listOfPurchaseProducts = new List<GameObject>();

        [Header("List of Product Objects")]
        [SerializeField] private List<GameObject> listOfProducts = new List<GameObject>();

        [Header("---------- UPGRADE SCREEN COMPONENTS ----------", order = 0)]
        [Header("Product Level/Name Text", order = 1)]
        [SerializeField] private Text productLevelNameText = null;

        [Header("Dirty Money Value")]
        [SerializeField] private Text productDirtyMoneyText = null;

        [Header("Clean Money Value")]
        [SerializeField] private Text productCleanMoneyText = null;

        [Header("Reputation Values")]
        [SerializeField] private Text productReputationText = null;

        [Header("Timer Value")]
        [SerializeField] private Text productTimerText = null;

        [Header("Cost Text")]
        [SerializeField] private Text upgradeCostText = null;

        [Header("Upgrade/Exit Button")]
        [SerializeField] private Button upgradeButton = null;

        [SerializeField] private Button exitUpgradePanelButton = null;

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

        /// <summary>
        /// Grabs the data.json file values that determines the amount of products the player has selling at once and the value that determines the maximum amount of products
        /// the player can sell at once, then sets them to the appropriate Properties.
        /// </summary>
        /// <param name="jsonData_ownedProducts">The amount of products the player is currently selling at once.</param>
        /// <param name="jsonData_totalProducts">The maximum amount of products the player can sell at once.</param>
        public void SetOwnedProductsAndTotalProductsViaJson(int jsonData_ownedProducts, int jsonData_totalProducts)
        {
            CurrentNumberOfOwnedProducts = jsonData_ownedProducts;
            TotalNumberOfProductCanSell = jsonData_totalProducts;

            if (CurrentNumberOfOwnedProducts < 1 || CurrentNumberOfOwnedProducts > 10)
            {
                CurrentNumberOfOwnedProducts = 1;
            }

            if (TotalNumberOfProductCanSell < 3 || TotalNumberOfProductCanSell > 10)
            {
                TotalNumberOfProductCanSell = 3;
            }

            productCanSellText.text = string.Format("{0}/{1}", CurrentNumberOfOwnedProducts, TotalNumberOfProductCanSell);
            AdjustNextProductPurchaseAfterLoadFromJson();
        }

        /// <summary>
        /// Used for the slim chance that the user exits the game before hitting the Product Upgrade button, which results in the Product's Level not being saved.
        /// <para>Originally, only the UpdateUpgradePanelValues() would cause the Product's Level to be saved. As a result, this new method saves the Product Level upon the start of the game.</para>
        /// </summary>
        /// <param name="productID">Which product is being referenced?</param>
        /// <param name="level">The level of said product.</param>
        public void GrabProductIDAndLevelForJson(int productID, int level)
        {
            ProductLevelValues[productID] = level;
        }

        /// <summary>
        /// Grabs the data.json file values that keeps track of all the Product Information values (Dirty Money, Clean Money, Reputation, Level, and Timer), then sets them to the
        /// appropriate Properties.
        /// <para>It also makes a check to see if the loop indexer (i) matches the productID value of every Product to make sure the right Products have the correct values.</para>
        /// </summary>
        /// <param name="jsonData">The entire array of information for every Product.</param>
        public void SetProductInformationViaJson(ProductIncomeData[] jsonData)
        {
            for (int i = 0; i < CurrentNumberOfOwnedProducts; i++)
            {
                try
                {
                    if (i == jsonData[i].productID)
                    {
                        ProductLevelValues[i] = jsonData[i].level;
                        ProductDirtyMoneyValues[i] = jsonData[i].dirtyMoney;
                        ProductCleanMoneyValues[i] = jsonData[i].cleanMoney;
                        ProductReputationValues[i] = jsonData[i].reputation;
                        ProductTimerValues[i] = jsonData[i].timer;
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
        ///
        /// </summary>
        /// <param name="incomeType"></param>
        /// <param name="value"></param>
        /// <param name="productID"></param>
        public void ValidateProductIncomeValues(Income incomeType, float value, int productID)
        {
            if (incomeType == Income.DirtyMoney)
            {
                ProductDirtyMoneyValues[productID] = value;
                CalculateTotalDirtyMoneyRevenue();
                return;
            }

            if (incomeType == Income.CleanMoney)
            {
                ProductCleanMoneyValues[productID] = value;
                CalculateTotalCleanMoneyRevenue();
                return;
            }
            if (incomeType == Income.Reputation)
            {
                ProductReputationValues[productID] = value;
                CalculateTotalReputation();
                return;
            }

            if (incomeType == Income.Timer)
            {
                ProductTimerValues[productID] = value;
                CalculateTotalTimeToSell();
                return;
            }
        }

        #region Calculate Product Values

        private void CalculateTotalDirtyMoneyRevenue()
        {
            float tmp = 0;

            for (int i = 0; i < ProductDirtyMoneyValues.Length; i++)
            {
                tmp += ProductDirtyMoneyValues[i];
            }

            TotalProductDirtyMoneyRevenue = Mathf.RoundToInt(tmp);
            CurrencyManager.Instance.SetWeeklyIncomeValues();
        }

        private void CalculateTotalCleanMoneyRevenue()
        {
            float tmp = 0;

            for (int i = 0; i < ProductCleanMoneyValues.Length; i++)
            {
                tmp += ProductCleanMoneyValues[i];
            }

            TotalProductCleanMoneyRevenue = Mathf.RoundToInt(tmp);
            CurrencyManager.Instance.SetWeeklyIncomeValues();
        }

        private void CalculateTotalReputation()
        {
            float tmp = 0;

            for (int i = 0; i < ProductReputationValues.Length; i++)
            {
                tmp += ProductReputationValues[i];
            }

            TotalProductReputation = Mathf.RoundToInt(tmp);
            CurrencyManager.Instance.SetWeeklyIncomeValues();
        }

        private void CalculateTotalTimeToSell()
        {
            float tmp = 0;

            for (int i = 0; i < ProductTimerValues.Length; i++)
            {
                tmp += ProductTimerValues[i];
            }

            TotalTimeToSell = tmp;
            GameManager.Instance.SetTotalTimeToSell();
        }

        #endregion Calculate Product Values

        #region Upgrade Product Values

        public void UpdateUpgradePanelTextValues(int productID, int level, float dmIncrease, float cmIncrease, float timerIncrease, float repIncrease, float upgradeCost)
        {
            productLevelNameText.text = string.Format("PRODUCT #{0} - LEVEL {1}", productID + 1, level);

            productDirtyMoneyText.text = string.Format("[{0} -> <color=#00C800>{1}</color>]", CurrencyManager.Instance.FormatValues(ProductDirtyMoneyValues[productID]),
                CurrencyManager.Instance.FormatValues(dmIncrease));
            productCleanMoneyText.text = string.Format("[{0} -> <color=#00C800>{1}</color>]", CurrencyManager.Instance.FormatValues(ProductCleanMoneyValues[productID]),
                CurrencyManager.Instance.FormatValues(cmIncrease));
            productReputationText.text = string.Format("[{0} -> <color=#00C800>{1}</color>]", CurrencyManager.Instance.FormatValues(ProductReputationValues[productID]),
                CurrencyManager.Instance.FormatValues(repIncrease));
            productTimerText.text = string.Format("[{0:#0.00}s -> <color=#00C800>{1:#0.00}s</color>]", ProductTimerValues[productID], timerIncrease);

            upgradeCostText.text = string.Format(CurrencyManager.Instance.FormatValues(upgradeCost));

            ProductLevelValues[productID] = level;
        }

        #endregion Upgrade Product Values

        private void AdjustNextProductPurchaseAfterLoadFromJson()
        {
            if (CurrentNumberOfOwnedProducts > 1)
            {
                for (int i = 1; i < CurrentNumberOfOwnedProducts; i++)
                {
                    CalculateAndDisplayAppropriateProductObjects();
                }
            }

            CheckTotalNumberOfSellableProducts();
        }

        public void UpdateNumberOfProductsValueAndText()
        {
            CalculateAndDisplayAppropriateProductObjects();

            CurrentNumberOfOwnedProducts++;
            productCanSellText.text = string.Format("{0}/{1}", CurrentNumberOfOwnedProducts, TotalNumberOfProductCanSell);

            CheckTotalNumberOfSellableProducts();
        }

        private void CalculateAndDisplayAppropriateProductObjects()
        {
            listOfPurchaseProducts[0].gameObject.SetActive(false);
            listOfPurchaseProducts.RemoveAt(0);
            listOfPurchaseProducts.TrimExcess();

            listOfProducts[0].gameObject.SetActive(true);
            listOfProducts.RemoveAt(0);
            listOfProducts.TrimExcess();
        }

        public void CheckTotalNumberOfSellableProducts()
        {
            if (listOfPurchaseProducts.Count > 0 && CurrentNumberOfOwnedProducts < TotalNumberOfProductCanSell)
            {
                listOfPurchaseProducts[0].gameObject.SetActive(true);
                return;
            }

            if (listOfPurchaseProducts.Count > 0 && CurrentNumberOfOwnedProducts >= TotalNumberOfProductCanSell)
            {
                listOfPurchaseProducts[0].gameObject.SetActive(false);
                return;
            }
        }

        public void IncrementTotalNumberOfProductsCanSell()
        {
            TotalNumberOfProductCanSell++;
            productCanSellText.text = string.Format("{0}/{1}", CurrentNumberOfOwnedProducts, TotalNumberOfProductCanSell);
            CheckTotalNumberOfSellableProducts();
        }

        public Button UpgradeButtonReference()
        {
            return upgradeButton;
        }

        public Button UpgradeExitButtonReference()
        {
            return exitUpgradePanelButton;
        }

        #endregion Custom Methods
    }
}