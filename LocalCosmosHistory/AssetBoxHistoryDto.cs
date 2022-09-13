using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LocalCosmosHistory
{
    public class AssetBoxHistoryDto : CosmosItem
    {
        public override string PartitionKey =>  $"{HistoryFor:yyyy-MM-dd}|{AssetBoxDefinitionType}";
        public AssetBoxHistorySource? Source { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        
        public long AssetBoxId { get; set; }
        public Guid AssetBoxGlobalId { get; set; }
        public Guid WrapperGlobalId { get; set; }
        public Guid InvestorGlobalId { get; set; }
        public DateTime HistoryFor { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AssetBoxDefinitionType AssetBoxDefinitionType { get; set; }
        public decimal PlanValue { get; set; }
        public decimal LegacyCash { get; set; }
        public bool AllAssetPricesConfirmed { get; set; }
        
        public AssetBoxTotals Totals { get; set; }

        public List<AssetHistoryDto> AssetHistory { get; set; } = new();

        public List<AssetHistoryDto> AssetHistoryAudit { get; set; } = new();
    }
    
    public class AssetHistoryDto
    {
        // TODO [DW] delete after transition
        [JsonIgnore]
        public long AssetDefinitionId { get; set; }
        
        [JsonProperty(PropertyName = "I")]
        public string AssetIsin { get; set; }
        
        [JsonProperty(PropertyName = "V")]
        public decimal Value { get; set; }
        
        [JsonProperty(PropertyName = "U")]
        public decimal Units { get; set; }
        
        [JsonProperty(PropertyName = "RU")]
        public decimal ReservedUnits { get; set; }
        
        [JsonProperty(PropertyName = "UP")]
        public decimal UnitPrice { get; set; }
        
        [JsonProperty(PropertyName = "AU")]
        public decimal AvailableUnits { get; set; }
        
        [JsonProperty(PropertyName = "CD")]
        public DateTime? ConfirmedDate { get; set; }
        [JsonProperty(PropertyName = "CA")]
        public DateTime CreatedAt { get; set; }

        public bool IsConfirmed => ConfirmedDate != null;
        
        public AssetHistoryDto() { }

        public AssetHistoryDto(string isin, decimal value)
        {
            AssetIsin = isin;
            Value = value;
        }

        internal void ConfirmUnitPrice(decimal unitPrice)
        {
            ConfirmedDate = DateTime.UtcNow;
            UnitPrice = unitPrice;
            Value = (Units * unitPrice).RoundDown(2);
        }
        
        public AssetHistoryDto Clone()
        {
            return new AssetHistoryDto
            {
                AssetIsin = AssetIsin, 
                Value = Value, 
                Units = Units, 
                ReservedUnits = ReservedUnits, 
                UnitPrice = UnitPrice, 
                AvailableUnits = AvailableUnits,
                ConfirmedDate = ConfirmedDate,
                CreatedAt = CreatedAt,
            };
        }
    }
    
    public class AssetBoxTotals
    {
        public long? DepositCount { get; set; }
        public decimal? Deposit { get; set; }
        public long? MoneyboxPlusCount { get; set; }
        public decimal? MoneyboxPlus { get; set; }
        public long? ExGratiaCount { get; set; }
        public decimal? ExGratia { get; set; }
        public long? MoneyboxContributionCount { get; set; }
        public decimal? MoneyboxContribution { get; set; }
        public long? WithdrawalsCount { get; set; }
        public decimal? Withdrawals { get; set; }
        public decimal? CashOut { get; set; }
        public int? CashOutCount { get; set; }
        public decimal? CashIn { get; set; }
        public int? CashInCount { get; set; }
        public decimal? TransfersIn { get; set; }
        public decimal? TransfersOut { get; set; }
        public decimal? ProductFeesPaid { get; set; }
        public decimal? ManagementFeesPaid { get; set; }
        public decimal? InterestPaid { get; set; }
        public decimal? InterestAccruedToDate { get; set; }
        public decimal? ForecastInterest { get; set; }
        public decimal? InternalTransfersOut { get; set; }
        public decimal? InternalTransfersIn { get; set; }
        public decimal? ReturnValue { get; set; }
        public decimal? ReturnPercentage { get; set; }
        public decimal? TimeWeightedReturn { get; set; }
        public decimal? AnnualisedTimeWeightedReturn { get; set; }
        public decimal? TimeWeightedReturnPreviousDay { get; set; }
        public decimal? AnnualisedTimeWeightedReturnPreviousDay { get; set; }
    }
    
    public enum AssetBoxHistorySource
    {
        Sycamore = 1,
        Moneybox = 2
    }
    
    public enum AssetBoxDefinitionType
    {
        CashBoxIsa,
        CashBoxLisa,
        CashBoxSipp,
        CashGia,
        CashLisa,
        Cash95,
        Cash45,
        Cash120,
        CashEasyAccess,
        CashEasyAccess2,
        FundsIsa,
        FundsJisa,
        FundsSipp,
        FundsGia,
        FundsLisa,
        InternalGia,
        SundryCashGia,
        SundryCashLisa,
        SundryCash95,
        SundryCash45,
        SundryCash120,
        SundryCashEasyAccess,
        SundryCashEasyAccess2,
        SundryCashBoxIsa,
        SundryCashBoxLisa,
        SundryCashBoxSipp,
        SundryFundsCore,
        SundryFundsSipp,
    }
}