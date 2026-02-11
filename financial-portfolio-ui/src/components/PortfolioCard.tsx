import type { Portfolio } from '../types';
import { TrendingUp, TrendingDown, Calendar, DollarSign, PieChart } from 'lucide-react';

interface PortfolioCardProps {
  portfolio: Portfolio;
  summary?: {
    totalValue: number;
    totalGainLoss: number;
    totalGainLossPercentage: number;
  };
  onClick?: () => void;
}

export const PortfolioCard = ({ portfolio, summary, onClick }: PortfolioCardProps) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
    }).format(amount);
  };

  const isPositive = summary ? summary.totalGainLoss >= 0 : true;

  return (
    <div
      onClick={onClick}
      className="bg-white rounded-xl shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer border border-gray-200 overflow-hidden group"
    >
      {/* Header with gradient */}
      <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-4 text-white">
        <div className="flex items-start justify-between">
          <div>
            <h3 className="text-xl font-bold mb-1">
              {portfolio.name}
            </h3>
            {portfolio.description && (
              <p className="text-blue-100 text-sm">
                {portfolio.description}
              </p>
            )}
          </div>
          <div className="bg-white bg-opacity-20 p-2 rounded-lg">
            <PieChart className="w-6 h-6" />
          </div>
        </div>
      </div>

      {/* Body */}
      <div className="p-4">
        {/* Value Section */}
        {summary && (
          <div className="mb-4">
            <div className="flex items-baseline justify-between mb-2">
              <span className="text-sm text-gray-600">Total Value</span>
              <span className="text-2xl font-bold text-gray-900">
                {formatCurrency(summary.totalValue)}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">Gain/Loss</span>
              <div className="flex items-center space-x-2">
                {isPositive ? (
                  <TrendingUp className="w-4 h-4 text-green-600" />
                ) : (
                  <TrendingDown className="w-4 h-4 text-red-600" />
                )}
                <span className={`font-semibold ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
                  {formatCurrency(Math.abs(summary.totalGainLoss))} ({summary.totalGainLossPercentage.toFixed(2)}%)
                </span>
              </div>
            </div>
          </div>
        )}

        {/* Divider */}
        <div className="border-t border-gray-200 my-4"></div>

        {/* Stats */}
        <div className="grid grid-cols-3 gap-4 text-center">
          <div>
            <p className="text-xs text-gray-600 mb-1">Holdings</p>
            <p className="text-lg font-semibold text-gray-900">
              {portfolio.holdings?.length || 0}
            </p>
          </div>
          <div>
            <p className="text-xs text-gray-600 mb-1">Accounts</p>
            <p className="text-lg font-semibold text-gray-900">
              {portfolio.accounts?.length || 0}
            </p>
          </div>
          <div>
            <p className="text-xs text-gray-600 mb-1">Created</p>
            <p className="text-xs font-medium text-gray-700">
              {new Date(portfolio.createdAt).toLocaleDateString('en-GB', { month: 'short', year: 'numeric' })}
            </p>
          </div>
        </div>
      </div>

      {/* Footer - Hover effect */}
      <div className="bg-gray-50 px-4 py-3 border-t border-gray-200 group-hover:bg-blue-50 transition-colors">
        <p className="text-sm text-gray-600 group-hover:text-blue-600 font-medium text-center">
          View Details →
        </p>
      </div>
    </div>
  );
};