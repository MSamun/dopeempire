using System;

namespace DopeEmpire
{
    [Serializable]
    public class SaveIncomeData
    {
        public TotalIncomeData totalIncomeData;
        public int numberOfOwnedProducts;
        public int totalNumberOfProductsCanSell;
        public ProductIncomeData[] productIncomeData;

        public SaveIncomeData()
        {
        }
    }

    #region Income Information

    //NOTE: JSON, for some reason, does not take in properties or private variables. It only takes public variables.
    [Serializable]
    public class TotalIncomeData
    {
        public float totalDirtyMoney;
        public float totalCleanMoney;
        public float totalReputation;

        public float weeklyDirtyMoney;
        public float weeklyCleanMoney;
        public float weeklyReputation;

        public TotalIncomeData(float totalDM, float totalCM, float totalREP, float weeklyDM, float weeklyCM, float weeklyREP)
        {
            totalDirtyMoney = totalDM;
            totalCleanMoney = totalCM;
            totalReputation = totalREP;

            weeklyDirtyMoney = weeklyDM;
            weeklyCleanMoney = weeklyCM;
            weeklyReputation = weeklyREP;
        }
    }

    [Serializable]
    public class ProductIncomeData
    {
        public int productID;
        public int level;
        public float dirtyMoney;
        public float cleanMoney;
        public float reputation;
        public float timer;

        public ProductIncomeData(int productID, int level, float dirtyMoney, float cleanMoney, float reputation, float timer)
        {
            this.productID = productID;
            this.level = level;
            this.dirtyMoney = dirtyMoney;
            this.cleanMoney = cleanMoney;
            this.reputation = reputation;
            this.timer = timer;
        }
    }

    #endregion Income Information
}