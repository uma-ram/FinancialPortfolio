import { useParams, useNavigate } from "react-router-dom";
import { PortfolioDetails } from "./PortfolioDetails";
import { ArrowLeft } from 'lucide-react'

export const PortfolioDetailsPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  if (!id) {
    return (
      <div className="container mx-auto px-4 py-8">
        <p className="text-red-600">Invalid portfolio ID</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Back Button */}
      <div className="bg-white border-b border-gray-200">
        <div className="container mx-auto px-4 py-4">
          <button
            onClick={() => navigate('/')}
            className="flex items-center space-x-2 text-gray-600 hover:text-gray-900 transition-colors"
          >
            <ArrowLeft className="w-5 h-5" />
            <span className="font-medium">Back to Dashboard</span>
          </button>
        </div>
      </div>

      {/* Portfolio Details */}
      <PortfolioDetails portfolioId={Number(id)} />
    </div>
  );
};
