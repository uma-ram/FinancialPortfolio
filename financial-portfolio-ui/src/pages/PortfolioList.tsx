import { useState, useEffect } from "react";
import type { Portfolio } from "../types";
import { portfolioApi } from "../services/api";
import { PortfolioCard } from "../components/PortfolioCard"; 
import { Loader2, AlertCircle } from "lucide-react";

export const PortfolioList = () =>{
    const[portfolios, setPortfolios] = useState<Portfolio[]>([]);
    const [loading, setLoading] = useState(true);
    const[error, setError] = useState<string| null>(null);
    
    //Fetch portfolios on component mount

    useEffect(()=>{
        fetchPortfolios();
    }, []);

    const fetchPortfolios = async()=>{
        try{
            setLoading(true);
            setError(null);

            // For now, hardcode userId = 1 (we'll make this dynamic later)
            const data = await portfolioApi.getUserPortfolios(1);
            setPortfolios(data);

        } catch(err: any){
            console.error('Error fetching portfolios:', err);
            setError(err.message || 'Failed to fetch portfolios');
        }finally {
            setLoading(false);
        }
    };

    const handlePortfolioClick = (portfolio: Portfolio) =>{
        console.log('Clicked portfolio:', portfolio);
    // We'll add navigation later
    } ;

    //Loading state
    if (loading){
        return (
        <div className="flex items-center justify-center min-h-screen">
            <div className="text-center">
                <Loader2 className="w-12 h-12 animate-spin text-blue-600 mx-auto mb-4" />
                <p className="text-gray-600">Loading portfolios...</p>
            </div>
        </div>
        );
    }

    // Error state
  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
          <p className="text-red-600 font-semibold mb-2">Error Loading Portfolios</p>
          <p className="text-gray-600 mb-4">{error}</p>
          <button
            onClick={fetchPortfolios}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Try Again
          </button>
        </div>
      </div>
    );
  }

  // Empty state
  if (portfolios.length === 0) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <p className="text-gray-600 mb-4">No portfolios found</p>
          <button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
            Create Portfolio
          </button>
        </div>
      </div>
    );
  }

   // Portfolio list
  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-2">
          My Portfolios
        </h1>
        <p className="text-gray-600">
          Manage your investment portfolios
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {portfolios.map((portfolio) => (
          <PortfolioCard
            key={portfolio.id}
            portfolio={portfolio}
            onClick={() => handlePortfolioClick(portfolio)}
          />
        ))}
      </div>
    </div>
  );
}