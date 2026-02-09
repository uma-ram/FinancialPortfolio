import type { Portfolio } from "../types";
import { TrendingUp, Calendar } from "lucide-react";

interface PortfolioCardProps{
    portfolio: Portfolio;
    onClick?: () => void;
}

export const PortfolioCard = ({portfolio, onClick}: PortfolioCardProps) =>{
    const formatDate = (dateString: string)=>{
        return new Date(dateString).toLocaleDateString('en-US',{
            year: 'numeric',
            month : 'short',
            day: 'numeric'
        });;
    }
    return (
        <div onClick={onClick}
        className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow cursor-pointer border border-gray-200">
            <div className="flex items-start justify-between mb-4">
                <div>
                    <h3 className="text-xl font-semibold text-gray-800">
                        {portfolio.name}
                    </h3>
                    {portfolio.description && (
                        <p className="text-sm text-gray-600 mt-1">
                            {portfolio.description}
                        </p>
                    )}
                </div>
                <div className="bg-blue-100 p-2 rounded-full">
                    <TrendingUp className="w-6 h-6 text-blue-600" />
                </div>
            </div>

            <div className="space-y-2">
                <div className="flex items-center text-sm text-gray-600">
                    <Calendar className="w-4 h-4 mr-2" />
                    Created :{ formatDate(portfolio.createdAt)}
                </div>

                {portfolio.holdings && portfolio.holdings.length > 0 &&(
                    <div className="text-sm text-gray-600">
                        Holdings: {portfolio.holdings.length}
                    </div>
                )}

                {portfolio.accounts && portfolio.accounts.length >0 &&(
                    <div className="text-sm text-gray-600">
                        Accounts: {portfolio.accounts.length}
                    </div>
                )}
            </div>
        </div>
    );
};
