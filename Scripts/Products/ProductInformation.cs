using System;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class ProductInformation : MonoBehavRefScript
    {
        [Serializable]
        public struct Information
        {
            public int productID;
            public float dirtyMoney;
            public float cleanMoney;
            public float reputation;
            public float timeToSell;
            public int level;
        }

        #region Variables

        private bool isSubscribed = false;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [Header("Current Product/Grade", order = 1)]
        [SerializeField] private Product currentProduct;

        [SerializeField] private Grade currentGrade;
        [SerializeField] private Text currentProductAndGradeText = null;

        [Header("Income Information")]
        public Information productInformation;

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Income Text Values", order = 1)]
        [SerializeField] private Text dirtyMoneyText = null;

        [SerializeField] private Text cleanMoneyText = null;
        [SerializeField] private Text reputationText = null;

        [Header("Time To Sell Value")]
        [SerializeField] private Text timeToSellText = null;

        [Header("Current Level")]
        [SerializeField] private Text currentLevelText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Start()
        {
            SetProductInformationValuesViaJson();
            ProductManager.Instance.GrabProductIDAndLevelForJson(productInformation.productID, productInformation.level);
        }

        public void SetProductInformationValuesViaJson()
        {
            // Level 0 indicates that the Product hasn't been purchased yet, so this is to update all Product income values with a level that does not equal to 0.
            if (ProductManager.Instance.ProductLevelValues[productInformation.productID] != 0)
            {
                productInformation.level = ProductManager.Instance.ProductLevelValues[productInformation.productID];
                productInformation.dirtyMoney = ProductManager.Instance.ProductDirtyMoneyValues[productInformation.productID];
                productInformation.cleanMoney = ProductManager.Instance.ProductCleanMoneyValues[productInformation.productID];
                productInformation.reputation = ProductManager.Instance.ProductReputationValues[productInformation.productID];
                productInformation.timeToSell = ProductManager.Instance.ProductTimerValues[productInformation.productID];
            }

            UpdateAndDisplayProperGradeAndProduct();
            UpdateAndDisplayProductInformationValuesAndText();
        }

        #endregion Initialization

        #region Custom Methods

        private void UpdateAndDisplayProperGradeAndProduct()
        {
            Color textColor = new Color(0, 0.78f, 0, 1);    //Green (default).

            if (currentGrade != MultiplierManager.Instance.CurrentResearchedGrade || currentProduct != MultiplierManager.Instance.CurrentResearchedProduct)
            {
                currentGrade = MultiplierManager.Instance.CurrentResearchedGrade;
                currentProduct = MultiplierManager.Instance.CurrentResearchedProduct;
            }

            if (currentProduct == Product.Coke)
            {
                textColor = new Color(0, 0.75f, 1, 1);      //Blue.
            }

            if (currentProduct == Product.Shrooms)
            {
                textColor = new Color(0.5f, 0.5f, 1, 1);    //Purple.
            }

            if (currentProduct == Product.Heroin)
            {
                textColor = new Color(1f, 0.4f, 0, 1);      //Orange.
            }

            if (currentProduct == Product.Pills)
            {
                textColor = new Color(1f, 0.8f, 0, 1);      //Yellow.
            }

            if (currentProduct == Product.Meth)
            {
                textColor = new Color(1f, 0, 0, 1);         //Red.
            }

            currentProductAndGradeText.color = textColor;
            currentProductAndGradeText.text = string.Format("{0} [{1}]", currentProduct.ToString().ToUpper(), currentGrade.ToString().ToUpper());
        }

        public void UpdateAndDisplayProductInformationValuesAndText()
        {
            ProductManager.Instance.ValidateProductIncomeValues(Income.DirtyMoney, productInformation.dirtyMoney, productInformation.productID);
            ProductManager.Instance.ValidateProductIncomeValues(Income.Reputation, productInformation.reputation, productInformation.productID);
            ProductManager.Instance.ValidateProductIncomeValues(Income.CleanMoney, productInformation.cleanMoney, productInformation.productID);
            ProductManager.Instance.ValidateProductIncomeValues(Income.Timer, productInformation.timeToSell, productInformation.productID);

            dirtyMoneyText.text = string.Format(CurrencyManager.Instance.FormatValues(productInformation.dirtyMoney));
            cleanMoneyText.text = string.Format(CurrencyManager.Instance.FormatValues(productInformation.cleanMoney));
            reputationText.text = string.Format(CurrencyManager.Instance.FormatValues(productInformation.reputation));
            timeToSellText.text = string.Format("{0:#0.00}s", productInformation.timeToSell);
            currentLevelText.text = string.Format("{0}", productInformation.level);
        }

        #endregion Custom Methods

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            if (!isSubscribed)
            {
                GameManager.Instance.OnWeekPassed += UpdateAndDisplayProductInformationValuesAndText;
                isSubscribed = true;
            }
        }

        #endregion OnEnable/OnDisable
    }
}