using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class UnlockNewProduct : MonoBehavRefScript
    {
        #region Variables

        private bool canPurchase;

        #region Game Components

        [Header("Purchase Cost")]
        [SerializeField] private float purchaseCost;

        [Header("---------- GAME COMPONENTS ----------", order = 0)]
        [Header("Purchase Cost Text", order = 1)]
        [SerializeField] private Text purchaseCostText = null;

        [Header("Purchase Button")]
        [SerializeField] private Button purchaseButton = null;

        private Text purchaseButtonText = null;

        #endregion Game Components

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            purchaseButtonText = purchaseButton.GetComponentInChildren<Text>();
        }

        private void Start()
        {
            purchaseCostText.text = string.Format(CurrencyManager.Instance.FormatValues(purchaseCost));
            CheckRequirements();
            CurrencyManager.Instance.OnMoneyValuesChanged += CheckRequirements;
        }

        #endregion Initialization

        #region Custom Methods

        private void CheckRequirements()
        {
            if (purchaseButton != null)
            {
                if (CurrencyManager.Instance.DirtyMoneyTotal < purchaseCost || ProductManager.Instance.CurrentNumberOfOwnedProducts >= ProductManager.Instance.TotalNumberOfProductCanSell)
                {
                    canPurchase = false;
                    purchaseButton.interactable = false;
                    purchaseButtonText.color = ProductManager.disabledColor;
                }

                if (CurrencyManager.Instance.DirtyMoneyTotal >= purchaseCost && ProductManager.Instance.CurrentNumberOfOwnedProducts < ProductManager.Instance.TotalNumberOfProductCanSell)
                {
                    canPurchase = true;
                    purchaseButton.interactable = true;
                    purchaseButtonText.color = Color.white;
                }
            }
        }

        public void PurchaseProduct()
        {
            CheckRequirements();

            if (canPurchase)
            {
                CurrencyManager.Instance.TotalIncomeGate(true, Income.DirtyMoney, purchaseCost);
                ProductManager.Instance.UpdateNumberOfProductsValueAndText();
            }
        }

        #endregion Custom Methods
    }
}