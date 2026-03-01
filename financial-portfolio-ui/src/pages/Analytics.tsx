import { useState, useEffect } from 'react';
import { portfolioApi } from '../services/api';
import type { PortfolioSummary } from '../types';
import { Navbar } from '../components/Navbar';
import { PortfolioPerformanceChart } from '../components/charts/PortfolioPerformanceChart';
import { AssetAllocationChart } from '../components/charts/AssetAllocationChart';
import { HoldingsPerformanceChart } from '../components/charts/HoldingsPerformanceChart';
import { ValueBreakdownChart } from '../components/charts/ValueBreakdownChart';
import { Loader2, AlertCircle, TrendingUp, TrendingDown, RefreshCw } from 'lucide-react';

export const Analytics = () => {
  const [summary, setSummary] = useState<PortfolioSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchAnalytics();
  }, []);

  const fetchAnalytics = async () => {
    try {
      setLoading(true);
      setError(null);

      // Fetch portfolio summary (hardcoded portfolio ID 1 for now)
      const data = await portfolioApi.getPortfolioSummary(1);
      setSummary(data);
    } catch (err: any) {
      console.error('Error fetching analytics:', err);
      setError(err.message || 'Failed to fetch analytics');
    } finally {
      setLoading(false);
    }
  };

  // Generate mock performance data (in real app, this would come from backend)
  const generatePerformanceData = () => {
    if (!summary) return [];

    const days = 30;
    const data = [];
    const baseValue = summary.totalCost;
    const currentValue = summary.totalValue;
    const valueRange = currentValue - baseValue;

    for (let i = 0; i < days; i++) {
      const progress = i / (days - 1);
      const date = new Date();
      date.setDate(date.getDate() - (days - 1 - i));

      data.push({
        date: date.toISOString(),
        value: baseValue + (valueRange * progress) + (Math.random() - 0.5) * 1000,
        cost: baseValue,
      });
    }

    return data;
  };

  // Generate asset allocation data
  const generateAllocationData = () => {
    if (!summary || summary.holdings.length === 0) return [];

    return summary.holdings.map(holding => ({
      name: holding.symbol,
      value: holding.currentValue,
      percentage: (holding.currentValue / summary.totalValue) * 100,
    }));
  };

  // Generate holdings performance data
  const generateHoldingsPerformance = () => {
    if (!summary) return [];

    return summary.holdings.map(holding => ({
      symbol: holding.symbol,
      gainLoss: holding.gainLoss,
      gainLossPercentage: holding.gainLossPercentage,
      currentValue: holding.currentValue,
    }));
  };

  // Generate value breakdown data
  const generateValueBreakdown = () => {
    if (!summary) return [];

    return summary.holdings.map(holding => ({
      symbol: holding.symbol,
      currentValue: holding.currentValue,
      totalCost: holding.quantity * holding.averageCost,
      gainLoss: holding.gainLoss,
    }));
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  if (loading) {
    return (
      <>
        <Navbar />
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="text-center">
            <Loader2 className="w-12 h-12 animate-spin text-blue-600 mx-auto mb-4" />
            <p className="text-gray-600">Loading analytics...</p>
          </div>
        </div>
      </>
    );
  }

  if (error || !summary) {
    return (
      <>
        <Navbar />
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="text-center max-w-md">
            <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Error Loading Analytics</h2>
            <p className="text-gray-600 mb-6">{error || 'No data available'}</p>
            <button
              onClick={fetchAnalytics}
              className="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            >
              <RefreshCw className="w-5 h-5 mr-2" />
              Try Again
            </button>
          </div>
        </div>
      </>
    );
  }

  const isPositive = summary.totalGainLoss >= 0;

  return (
    <>
      <Navbar />
      <div className="min-h-screen bg-gray-50">
        <div className="container mx-auto px-4 py-8">
          {/* Header */}
          <div className="flex items-center justify-between mb-8">
            <div>
              <h1 className="text-3xl font-bold text-gray-800 mb-2">Portfolio Analytics</h1>
              <p className="text-gray-600">Comprehensive performance insights</p>
            </div>
            <button
              onClick={fetchAnalytics}
              className="flex items-center space-x-2 px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
            >
              <RefreshCw className="w-4 h-4" />
              <span>Refresh</span>
            </button>
          </div>

          {/* Summary Cards */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-sm text-gray-600 mb-1">Total Value</p>
              <p className="text-2xl font-bold text-gray-900">
                {formatCurrency(summary.totalValue)}
              </p>
            </div>

            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-sm text-gray-600 mb-1">Total Cost</p>
              <p className="text-2xl font-bold text-gray-900">
                {formatCurrency(summary.totalCost)}
              </p>
            </div>

            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-sm text-gray-600 mb-1">Total Gain/Loss</p>
              <div className="flex items-center space-x-2">
                {isPositive ? (
                  <TrendingUp className="w-5 h-5 text-green-600" />
                ) : (
                  <TrendingDown className="w-5 h-5 text-red-600" />
                )}
                <p className={`text-2xl font-bold ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
                  {formatCurrency(Math.abs(summary.totalGainLoss))}
                </p>
              </div>
            </div>

            <div className="bg-white p-6 rounded-lg shadow-md">
              <p className="text-sm text-gray-600 mb-1">Return %</p>
              <p className={`text-2xl font-bold ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
                {isPositive ? '+' : ''}{summary.totalGainLossPercentage.toFixed(2)}%
              </p>
            </div>
          </div>

          {/* Charts Grid */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
            {/* Performance Line Chart */}
            <PortfolioPerformanceChart data={generatePerformanceData()} />

            {/* Asset Allocation Pie Chart */}
            <AssetAllocationChart data={generateAllocationData()} />

            {/* Holdings Performance Bar Chart */}
            <HoldingsPerformanceChart data={generateHoldingsPerformance()} />

            {/* Value Breakdown Chart */}
            <ValueBreakdownChart data={generateValueBreakdown()} />
          </div>

          {/* Holdings Table */}
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-xl font-bold text-gray-800">Detailed Holdings</h2>
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
      </div>
    </>
  );
};