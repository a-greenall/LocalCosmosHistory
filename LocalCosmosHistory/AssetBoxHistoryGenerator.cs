using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace LocalCosmosHistory
{
    public static class AssetBoxHistoryGenerator
    {
        private static readonly List<AssetBoxDefinitionType> AssetBoxDefinitionTypes = new()
            { AssetBoxDefinitionType.FundsGia, AssetBoxDefinitionType.FundsSipp, AssetBoxDefinitionType.CashBoxIsa };

        private static readonly List<string> AssetIsins = new()
            { "GB00BPFJCF57", "GB00B41XG308", "GB00BJS8SJ34" };
        
        public static List<AssetBoxHistoryDto> GetAssetBoxHistoryItems(int amount, DateTime date)
        {
            var testTotals = ConfigureTestTotals();
            var testAssetHistory = ConfigureTestAssetHistory();

            var testHistory = new Faker<AssetBoxHistoryDto>("en_GB")
                .StrictMode(true)
                .RuleFor(ab => ab.HistoryFor, _ => date)
                .RuleFor(ab => ab.Type, (_, ab) => ab.GetType().Name)
                .RuleFor(ab => ab.Id, f => Guid.NewGuid().ToString())
                .RuleFor(ab => ab.Source, f => f.PickRandom<AssetBoxHistorySource>())
                .RuleFor(ab => ab.CreatedOn, f => f.Date.Recent())
                .RuleFor(ab => ab.ModifiedOn, (_, o) => o.CreatedOn)
                .RuleFor(ab => ab.AssetBoxId, f => f.Random.Long(1, 999))
                .RuleFor(ab => ab.AssetBoxGlobalId, _ => Guid.NewGuid())
                .RuleFor(ab => ab.WrapperGlobalId, _ => Guid.NewGuid())
                .RuleFor(ab => ab.InvestorGlobalId, _ => Guid.NewGuid())
                .RuleFor(ab => ab.AssetBoxDefinitionType, f => f.PickRandom(AssetBoxDefinitionTypes))
                .RuleFor(ab => ab.PlanValue, f => f.Finance.Amount())
                .RuleFor(ab => ab.LegacyCash, _ => 0M)
                .RuleFor(ab => ab.AllAssetPricesConfirmed, f => f.Random.Bool())
                .RuleFor(ab => ab.Totals, f => testTotals.Generate(1).Single())
                .RuleFor(ab => ab.AssetHistory, f => testAssetHistory.Generate(f.Random.Int(1, 5)))
                .RuleFor(ab => ab.AssetHistoryAudit, (f, ah) => testAssetHistory.Generate(ah.AssetHistory.Count));

            return testHistory.Generate(amount);
        }
        
        private static Faker<AssetBoxTotals> ConfigureTestTotals()
        {
            var testTotals = new Faker<AssetBoxTotals>("en_GB")
                .StrictMode(true)
                .RuleFor(t => t.DepositCount, f => f.Random.Long(1, 999))
                .RuleFor(t => t.Deposit, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.MoneyboxPlusCount, f => f.Random.Long(1, 999))
                .RuleFor(t => t.MoneyboxPlus, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.ExGratiaCount, f => f.Random.Long(1, 999))
                .RuleFor(t => t.ExGratia, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.MoneyboxContributionCount, f => f.Random.Long(1, 999))
                .RuleFor(t => t.MoneyboxContribution, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.WithdrawalsCount, f => f.Random.Long(1, 999))
                .RuleFor(t => t.Withdrawals, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.CashOutCount, f => f.Random.Int(1, 999))
                .RuleFor(t => t.CashOut, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.CashInCount, f => f.Random.Int(1, 999))
                .RuleFor(t => t.CashIn, (f, t) => t.DepositCount * f.Finance.Amount(10))
                .RuleFor(t => t.TransfersIn, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.TransfersOut, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.ProductFeesPaid, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.ManagementFeesPaid, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.InterestPaid, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.InterestAccruedToDate, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.ForecastInterest, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.InternalTransfersOut, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.InternalTransfersIn, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.ReturnValue, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.ReturnPercentage, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.TimeWeightedReturn, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.AnnualisedTimeWeightedReturn, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.TimeWeightedReturnPreviousDay, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.TransfersIn, f => f.Random.Decimal(1, 10000))
                .RuleFor(t => t.AnnualisedTimeWeightedReturnPreviousDay, f => f.Random.Decimal(1, 10000));
            
            return testTotals;
        }
        
        private static Faker<AssetHistoryDto> ConfigureTestAssetHistory()
        {
            return new Faker<AssetHistoryDto>("en_GB")
                .StrictMode(false)
                .RuleFor(ah => ah.AssetIsin, f => f.PickRandom(AssetIsins))
                .RuleFor(ah => ah.Value, f => f.Finance.Amount(1))
                .RuleFor(ah => ah.Units, f => f.Random.Decimal(1, 500))
                .RuleFor(ah => ah.ReservedUnits, (f, ah) => f.Random.Decimal(0, ah.Units))
                .RuleFor(ah => ah.UnitPrice, f => f.Random.Decimal(1, 50))
                .RuleFor(ah => ah.AvailableUnits, (f, ah) => f.Random.Decimal(0, ah.Units))
                .RuleFor(ah => ah.ConfirmedDate, f => f.Date.Recent())
                .RuleFor(ah => ah.CreatedAt, f => f.Date.Recent());
        }
    }
}