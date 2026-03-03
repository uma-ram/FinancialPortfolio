import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Portfolio, PortfolioSummary } from '../types';
import { portfolioApi } from '../services/api';
import { PortfolioCard } from '../components/PortfolioCard';
import { Loader2, AlertCircle, Plus } from 'lucide-react';

export const Portfolios = () => {
  const navigate = useNavigate();
  const [portfolios, setPortfolios] = useState<Portfolio[]>([]);
  const [summaries, setSummaries] = useState<Map<number, PortfolioSummary>>(new Map());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchPortfolios();
  }, []);

  const fetchPortfolios = async () => {
    try {
      setLoading(true);
      setError(null);

      const portfolioData = await portfolioApi.getUserPortfolios(1);
      setPortfolios(portfolioData);

      const summaryMap = new Map<number, PortfolioSummary>();
      for (const portfolio of portfolioData) {
        try {
          const summary = await portfolioApi.getPortfolioSummary(portfolio.id);
          summaryMap.set(portfolio.id, summary);
        } catch (err) {
          console.error(`Failed to fetch summary for portfolio ${portfolio.id}`);
        }
      }
      setSummaries(summaryMap);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch portfolios');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="w-12 h-12 animate-spin text-blue-600" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
          <p className="text-red-600">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">All Portfolios</h1>
          <p className="text-gray-600">Manage your investment portfolios</p>
        </div>
        <button className="flex items-center space-x-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          <Plus className="w-5 h-5" />
          <span>New Portfolio</span>
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {portfolios.map((portfolio) => (
          <PortfolioCard
            key={portfolio.id}
            portfolio={portfolio}
            summary={summaries.get(portfolio.id)}
            onClick={() => navigate(`/portfolio/${portfolio.id}`)}
          />
        ))}
      </div>
    </div>
  );
};