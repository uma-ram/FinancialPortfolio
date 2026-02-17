import { useDataFetching } from '../hooks/useDataFetching.ts';
import { portfolioApi } from '../services/api';
//import type { Portfolio, PortfolioSummary } from '../types';
import { Loader2, AlertCircle, RefreshCw, TrendingUp, TrendingDown } from 'lucide-react';

interface PortfolioDetailsProps {
  portfolioId: number;
}

export const PortfolioDetails = ({ portfolioId }: PortfolioDetailsProps) => {
  // Fetch portfolio data
  const {
    data: portfolio,
    loading: portfolioLoading,
    error: portfolioError,
    refetch: refetchPortfolio,
  } = useDataFetching({
    fetchFn: () => portfolioApi.getPortfolio(portfolioId),
    dependencies: [portfolioId],
  });

  // Fetch portfolio summary
  const {
    data: summary,
    loading: summaryLoading,
    error: summaryError,
    refetch: refetchSummary,
  } = useDataFetching({
    fetchFn: () => portfolioApi.getPortfolioSummary(portfolioId),
    dependencies: [portfolioId],
  });

   const loading = portfolioLoading || summaryLoading;
  const error = portfolioError || summaryError;

  const handleRefresh = async () => {
    await Promise.all([refetchPortfolio(), refetchSummary()]);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-blue-600 mx-auto mb-4" />
          <p className="text-gray-600">Loading portfolio details...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center max-w-md">
          <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-800 mb-2">Error Loading Portfolio</h2>
          <p className="text-gray-600 mb-6">{error}</p>
          <button
            onClick={handleRefresh}
            className="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            <RefreshCw className="w-5 h-5 mr-2" />
            Try Again
          </button>
        </div>
      </div>
    );
  }
  if (!portfolio || !summary) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-600">Portfolio not found</p>
      </div>
    );
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
    }).format(amount);
  };

  const isPositive = summary.totalGainLoss >= 0;
return (
    <div className="container mx-auto px-4 py-8">
      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">
            {portfolio.name}
          </h1>
          {portfolio.description && (
            <p className="text-gray-600">{portfolio.description}</p>
          )}
        </div>
        <button
          onClick={handleRefresh}
          className="flex items-center space-x-2 px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          <RefreshCw className="w-4 h-4" />
          <span>Refresh</span>
        </button>
      </div>

      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-white p-6 rounded-lg shadow-md">
          <p className="text-sm text-gray-600 mb-1">Total Value</p>
          <p className="text-3xl font-bold text-gray-900">
            {formatCurrency(summary.totalValue)}
          </p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <p className="text-sm text-gray-600 mb-1">Total Cost</p>
          <p className="text-3xl font-bold text-gray-900">
            {formatCurrency(summary.totalCost)}
          </p>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-md">
          <p className="text-sm text-gray-600 mb-1">Gain/Loss</p>
          <div className="flex items-center space-x-2">
            {isPositive ? (
              <TrendingUp className="w-6 h-6 text-green-600" />
            ) : (
              <TrendingDown className="w-6 h-6 text-red-600" />
            )}
            <p className={`text-3xl font-bold ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
              {formatCurrency(Math.abs(summary.totalGainLoss))}
            </p>
          </div>
          <p className={`text-sm mt-1 ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
            {summary.totalGainLossPercentage.toFixed(2)}% return
          </p>
        </div>
      </div>

      {/* Holdings Table */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-800">Holdings</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Symbol</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Quantity</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Avg Cost</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Current Price</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Current Value</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Gain/Loss</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Return %</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {summary.holdings.map((holding) => {
                const holdingPositive = holding.gainLoss >= 0;
                return (
                  <tr key={holding.symbol} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="font-semibold text-gray-900">{holding.symbol}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-700">
                      {holding.quantity}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-700">
                      {formatCurrency(holding.averageCost)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-700">
                      {formatCurrency(holding.currentPrice)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap font-semibold text-gray-900">
                      {formatCurrency(holding.currentValue)}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap font-semibold ${holdingPositive ? 'text-green-600' : 'text-red-600'}`}>
                      {formatCurrency(Math.abs(holding.gainLoss))}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap font-semibold ${holdingPositive ? 'text-green-600' : 'text-red-600'}`}>
                      {holdingPositive ? '+' : ''}{holding.gainLossPercentage.toFixed(2)}%
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};