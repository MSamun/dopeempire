using UnityEngine;

namespace DopeEmpire
{
    [RequireComponent(typeof(ProductInformation))]
    public class ApplyProductMultiplier : MonoBehavRefScript
    {
        #region Variables

        private ProductInformation prodScript;

        //These values are used to properly apply the multiplier.
        //For example, let's say that the user is earning $100 of Dirty Money, and buys a 50% increase multiplier. The new value should be at $150 Dirty Money.
        //Now, let's say that the user purchases an additional 10% increase multiplier (total of 60% increase). The value will go up to $165 instead of $160.
        //These variables hold the last multipliers that were applied to each source of income, resets the income to its original value (without multiplier), then applies the new multiplier.
        //Using the example above, it'll reset the old 50% increase multiplier, setting Dirty Money back to $100, then apply the new 60% increase multiplier, setting Dirty Money to a new $160.
        private float localTimerReductionMultiplier;

        private float localDirtyMoneyMultiplier;
        private float localCleanMoneyMultiplier;
        private float localReputationMultiplier;

        #endregion Variables

        #region Initialization

        private void Awake()
        {
            prodScript = GetComponent<ProductInformation>();

            localTimerReductionMultiplier = 0;
            localDirtyMoneyMultiplier = 0;
            localCleanMoneyMultiplier = 0;
            localReputationMultiplier = 0;
        }

        private void Start()
        {
            ApplyMultiplierToProducts();
            MultiplierManager.Instance.OnPurchaseMultiplier += ApplyMultiplierToProducts;
        }

        #endregion Initialization

        #region Custom Methods

        private void ApplyMultiplierToProducts()
        {
            if (localTimerReductionMultiplier != MultiplierManager.Instance.TimerReductionMultiplier)
            {
                prodScript.productInformation.timeToSell /= 1 - localTimerReductionMultiplier;
                prodScript.productInformation.timeToSell *= 1 - MultiplierManager.Instance.TimerReductionMultiplier;
                localTimerReductionMultiplier = MultiplierManager.Instance.TimerReductionMultiplier;
            }

            if (localDirtyMoneyMultiplier != MultiplierManager.Instance.DirtyMoneyIncreaseMultiplier)
            {
                prodScript.productInformation.dirtyMoney /= 1 + localDirtyMoneyMultiplier;
                prodScript.productInformation.dirtyMoney *= 1 + MultiplierManager.Instance.DirtyMoneyIncreaseMultiplier;
                localDirtyMoneyMultiplier = MultiplierManager.Instance.DirtyMoneyIncreaseMultiplier;

                prodScript.productInformation.cleanMoney = prodScript.productInformation.dirtyMoney * MultiplierManager.Instance.CleanMoneyIncreaseMultiplier;
                prodScript.productInformation.reputation = prodScript.productInformation.dirtyMoney * MultiplierManager.Instance.ReputationIncreaseMultiplier;
            }

            if (localCleanMoneyMultiplier != MultiplierManager.Instance.CleanMoneyIncreaseMultiplier)
            {
                prodScript.productInformation.cleanMoney = prodScript.productInformation.dirtyMoney * MultiplierManager.Instance.CleanMoneyIncreaseMultiplier;
                localCleanMoneyMultiplier = MultiplierManager.Instance.CleanMoneyIncreaseMultiplier;
            }

            if (localReputationMultiplier != MultiplierManager.Instance.ReputationIncreaseMultiplier)
            {
                prodScript.productInformation.reputation = prodScript.productInformation.dirtyMoney * MultiplierManager.Instance.ReputationIncreaseMultiplier;
                localReputationMultiplier = MultiplierManager.Instance.ReputationIncreaseMultiplier;
            }

            prodScript.UpdateAndDisplayProductInformationValuesAndText();
        }

        #endregion Custom Methods
    }
}