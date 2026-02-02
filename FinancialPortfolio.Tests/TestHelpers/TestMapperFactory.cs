using AutoMapper;
using FinancialPortfolio.Api.Mappings;
using Microsoft.Extensions.Logging;

namespace FinancialPortfolio.Tests.TestHelpers;

public static class TestMapperFactory
{
    public static IMapper Create()
    {
        var configExpression = new MapperConfigurationExpression();
        configExpression.AddProfile<MappingProfile>();
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { }); // Assign a value to loggerFactory

        var config = new MapperConfiguration(configExpression, loggerFactory);

#if DEBUG
        config.AssertConfigurationIsValid();
#endif

        return config.CreateMapper();
    }
}
