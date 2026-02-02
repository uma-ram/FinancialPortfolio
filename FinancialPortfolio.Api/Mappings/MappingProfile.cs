namespace FinancialPortfolio.Api.Mappings;
using AutoMapper;
using FinancialPortfolio.Api.Models.DTOs.Responses;
using FinancialPortfolio.Api.Models;
public class MappingProfile : Profile
{
    public MappingProfile()
    {

        //user mappings
        CreateMap<User, UserResponse>();

        // Portfolio
        CreateMap<Portfolio, PortfolioResponse>();

        // Account
        CreateMap<Account, AccountResponse>();

        // Transaction
        CreateMap<Transaction, TransactionResponse>();

        // Holding
        CreateMap<Holding, HoldingResponse>()
            .ForMember(d => d.TotalCost, opt => opt.MapFrom(s => s.TotalCost))
            .ForMember(d => d.CurrentValue, opt => opt.MapFrom(s => s.CurrentValue))
            .ForMember(d => d.GainLoss, opt => opt.MapFrom(s => s.GainLoss))
            .ForMember(d => d.GainLossPercentage, opt => opt.MapFrom(s => s.GainLossPercentage));

        // Holding → HoldingSummary
        CreateMap<Holding, HoldingSummary>()
            .ForMember(d => d.CurrentValue, opt => opt.MapFrom(s => s.CurrentValue))
            .ForMember(d => d.GainLoss, opt => opt.MapFrom(s => s.GainLoss))
            .ForMember(d => d.GainLossPercentage, opt => opt.MapFrom(s => s.GainLossPercentage));

        // Portfolio → PortfolioSummaryResponse
        CreateMap<Portfolio, PortfolioSummaryResponse>()
            .ForMember(d => d.PortfolioId, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.PortfolioName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Holdings, opt => opt.MapFrom(s => s.Holdings))

            // These will be calculated after mapping
            .ForMember(d => d.TotalValue, opt => opt.Ignore())
            .ForMember(d => d.TotalCost, opt => opt.Ignore())
            .ForMember(d => d.TotalGainLoss, opt => opt.Ignore())
            .ForMember(d => d.TotalGainLossPercentage, opt => opt.Ignore())
            .ForMember(d => d.TotalHoldings, opt => opt.Ignore())

            // Post-mapping business logic
            .AfterMap((src, dest) =>
            {
                dest.TotalValue = dest.Holdings.Sum(h => h.CurrentValue);
                dest.TotalCost = dest.Holdings.Sum(h => h.Quantity * h.AverageCost);
                dest.TotalGainLoss = dest.TotalValue - dest.TotalCost;
                dest.TotalGainLossPercentage = dest.TotalCost > 0
                    ? (dest.TotalGainLoss / dest.TotalCost) * 100
                    : 0;
                dest.TotalHoldings = dest.Holdings.Count;
            });
    }
}
