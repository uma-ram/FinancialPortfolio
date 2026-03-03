import { Link, useLocation } from 'react-router-dom';
import { TrendingUp, Home, Briefcase, ArrowLeftRight, BarChart3, User } from "lucide-react";

export const Navbar = () => {
    const location = useLocation();

  // Helper to check if link is active
  const isActive = (path: string) => {
    return location.pathname === path || location.pathname.startsWith(path);
  };

    return(
        <nav className="bg-gradient-to-r from-blue-600 to-blue-800 text-white shadow-lg">
            <div className="container mx-auto px-4">
                <div className="flex items-center justify-between h-16">
                    {/* Logo */}
                    <Link to="/" className="flex items-center space-x-3 hover:opacity-80 transition-opacity"></Link>
                    <div className="flex items-center space-x-3">
                        <div className="bg-white p-2 rounded-lg">
                            <TrendingUp className="w-6 h-6 text-blue-600" />
                        </div>
                        <span className="text-xl font-bold">Portfolio Manager</span>
                    </div>

                    {/* Navigation Links */}
                    <div className="hidden md:flex space-x-6">
                        <NavLink 
                            to="/" 
                            icon={<Home className="w-5 h-5" />} 
                            label="Dashboard" 
                            active={isActive('/')} 
                            />
                        <NavLink 
                            to="/portfolios" 
                            icon={<Briefcase className="w-5 h-5" />} 
                            label="Portfolios" 
                            active={isActive('/portfolios')} 
                        />
                        <NavLink 
                            to="/transactions" 
                            icon={<ArrowLeftRight className="w-5 h-5" />} 
                            label="Transactions" 
                            active={isActive('/transactions')} 
                        />
                        <NavLink 
                            to="/analytics" 
                            icon={<BarChart3 className="w-5 h-5" />} 
                            label="Analytics" 
                            active={isActive('/analytics')} 
                        />
                    </div>

                     {/* User Menu */}
                    <div className="flex items-center space-x-3">
                        <div className="hidden md:block text-right">
                            <p className="text-sm font-medium">Rani Investor</p>
                            <p className="text-xs text-blue-200">Rani@example.com</p>
                        </div>
                        <button className="bg-blue-700 hover:bg-blue-600 p-2 rounded-full transition-colors">
                            <User className="w-6 h-6" />
                        </button>
                    </div>

                </div>
            </div>
        </nav>
    );
};

// NavLink Component
interface NavLinkProps {
  to:string
  icon: React.ReactNode;
  label: string;
  active?: boolean;
}

const NavLink = ({ to, icon, label, active }: NavLinkProps) => {
  return (
    <Link
      to={to}
      className={`flex items-center space-x-2 px-3 py-2 rounded-lg transition-colors ${
        active
          ? 'bg-blue-700 text-white'
          : 'text-blue-100 hover:bg-blue-700 hover:text-white'
      }`}
    >
      {icon}
      <span className="font-medium">{label}</span>
    </Link>
  );
};