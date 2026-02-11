import { useState, useEffect } from 'react';
import type { Portfolio, PortfolioSummary } from '../types';
import { portfolioApi } from '../services/api';
import { Navbar } from '../components/Navbar';
import { StatCard } from '../components/StatCard';
import { PortfolioCard } from '../components/PortfolioCard';
import { 
  Loader2, 
  AlertCircle, 
  PoundSterling,
  DollarSign, 
  TrendingUp, 
  Briefcase, 
  PieChart,
  RefreshCw, 
  TrendingDown
} from 'lucide-react';

export const Dashboard = () => {
  const [portfolios, setPortfolios] = useState<Portfolio[]>([]);
  const [summaries, setSummaries] = useState<Map<number, PortfolioSummary>>(new Map());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Fetch portfolios for user ID 1 (hardcoded for now)
      const portfolioData = await portfolioApi.getUserPortfolios(1);
      setPortfolios(portfolioData);

      // Fetch summary for each portfolio
      const summaryMap = new Map<number, PortfolioSummary>();
      for (const portfolio of portfolioData) {
        try {
          const summary = await portfolioApi.getPortfolioSummary(portfolio.id);
          summaryMap.set(portfolio.id, summary);
        } catch (err) {
          console.error(`Failed to fetch summary for portfolio ${portfolio.id}`, err);
        }
      }
      setSummaries(summaryMap);
    } catch (err: any) {
      console.error('Error fetching dashboard data:', err);
      setError(err.message || 'Failed to fetch dashboard data');
    } finally {
      setLoading(false);
    }
  };

  // Calculate overall stats
  const calculateOverallStats = () => {
    let totalValue = 0;
    let totalCost = 0;
    let totalHoldings = 0;

    summaries.forEach((summary) => {
      totalValue += summary.totalValue;
      totalCost += summary.totalCost;
      totalHoldings += summary.totalHoldings;
    });

    const totalGainLoss = totalValue - totalCost;
    const totalGainLossPercentage = totalCost > 0 ? (totalGainLoss / totalCost) * 100 : 0;

    return {
      totalValue,
      totalCost,
      totalGainLoss,
      totalGainLossPercentage,
      totalHoldings,
      portfolioCount: portfolios.length,
    };
  };

  const stats = calculateOverallStats();

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
    }).format(amount);
  };

  // Loading state
  if (loading) {
    return (
      <>
        <Navbar />
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="text-center">
            <Loader2 className="w-12 h-12 animate-spin text-blue-600 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Loading your portfolio...</p>
          </div>
        </div>
      </>
    );
  }

  // Error state
  if (error) {
    return (
      <>
        <Navbar />
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="text-center max-w-md">
            <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Oops! Something went wrong</h2>
            <p className="text-gray-600 mb-6">{error}</p>
            <button
              onClick={fetchDashboardData}
              className="inline-flex items-center px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <RefreshCw className="w-5 h-5 mr-2" />
              Try Again
            </button>
          </div>
        </div>
      </>
    );
  }

  // Empty state
  if (portfolios.length === 0) {
    return (
      <>
        <Navbar />
        <div className="min-h-screen bg-gray-50">
          <div className="container mx-auto px-4 py-16">
            <div className="max-w-2xl mx-auto text-center">
              <Briefcase className="w-20 h-20 text-gray-400 mx-auto mb-6" />
              <h2 className="text-3xl font-bold text-gray-800 mb-4">
                Welcome to Portfolio Manager!
              </h2>
              <p className="text-gray-600 mb-8 text-lg">
                You don't have any portfolios yet. Create your first portfolio to start tracking your investments.
              </p>
              <button className="px-8 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-semibold">
                Create Your First Portfolio
              </button>
            </div>
          </div>
        </div>
      </>
    );
  }

  // Main dashboard
  return (
    <>
      <Navbar />
      <div className="min-h-screen bg-gray-50">
        <div className="container mx-auto px-4 py-8">
          {/* Header */}
          <div className="mb-8">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-3xl font-bold text-gray-800 mb-2">
                  Dashboard
                </h1>
                <p className="text-gray-600">
                  Welcome back! Here's your portfolio overview.
                </p>
              </div>
              <button
                onClick={fetchDashboardData}
                className="flex items-center space-x-2 px-4 py-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
              >
                <RefreshCw className="w-4 h-4" />
                <span>Refresh</span>
              </button>
            </div>
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <StatCard
              title="Total Portfolio Value"
              value={formatCurrency(stats.totalValue)}
              subtitle="Across all portfolios"
              icon={PoundSterling}
              iconBgColor="bg-green-100"
              iconColor="text-green-600"
            />
            <StatCard
              title="Total Gain/Loss"
              value={formatCurrency(Math.abs(stats.totalGainLoss))}
              subtitle={`${stats.totalGainLossPercentage.toFixed(2)}% return`}
              icon={TrendingUp}
              trend={{
                value: `${stats.totalGainLossPercentage.toFixed(2)}%`,
                isPositive: stats.totalGainLoss >= 0,
              }}
              iconBgColor={stats.totalGainLoss >= 0 ? 'bg-green-100' : 'bg-red-100'}
              iconColor={stats.totalGainLoss >= 0 ? 'text-green-600' : 'text-red-600'}
            />
            <StatCard
              title="Active Portfolios"
              value={stats.portfolioCount.toString()}
              subtitle="Investment portfolios"
              icon={Briefcase}
              iconBgColor="bg-blue-100"
              iconColor="text-blue-600"
            />
            <StatCard
              title="Total Holdings"
              value={stats.totalHoldings.toString()}
              subtitle="Across all accounts"
              icon={PieChart}
              iconBgColor="bg-purple-100"
              iconColor="text-purple-600"
            />
          </div>

          {/* Portfolios Section */}
          <div className="mb-8">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold text-gray-800">Your Portfolios</h2>
              <button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium">
                + New Portfolio
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {portfolios.map((portfolio) => (
                <PortfolioCard
                  key={portfolio.id}
                  portfolio={portfolio}
                  summary={summaries.get(portfolio.id)}
                  onClick={() => console.log('Navigate to portfolio', portfolio.id)}
                />
              ))}
            </div>
          </div>

          {/* Quick Actions */}
          <div className="bg-white rounded-xl shadow-md p-6">
            <h3 className="text-lg font-bold text-gray-800 mb-4">Quick Actions</h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button className="flex items-center space-x-3 p-4 border border-gray-200 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-colors">
                <div className="bg-blue-100 p-2 rounded-lg">
                  <TrendingUp className="w-5 h-5 text-blue-600" />
                </div>
                <div className="text-left">
                  <p className="font-medium text-gray-900">Buy Stocks</p>
                  <p className="text-sm text-gray-600">Add new holdings</p>
                </div>
              </button>
              <button className="flex items-center space-x-3 p-4 border border-gray-200 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-colors">
                <div className="bg-green-100 p-2 rounded-lg">
                  <DollarSign className="w-5 h-5 text-green-600" />
                </div>
                <div className="text-left">
                  <p className="font-medium text-gray-900">View Analytics</p>
                  <p className="text-sm text-gray-600">Performance insights</p>
                </div>
              </button>
              <button className="flex items-center space-x-3 p-4 border border-gray-200 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-colors">
                <div className="bg-purple-100 p-2 rounded-lg">
                  <Briefcase className="w-5 h-5 text-purple-600" />
                </div>
                <div className="text-left">
                  <p className="font-medium text-gray-900">Manage Accounts</p>
                  <p className="text-sm text-gray-600">Organize holdings</p>
                </div>
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};