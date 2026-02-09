export interface User{
    id: number;
    userName: string;
    email:string;
    createdAt: string;
    portfolios?: Portfolio[];
}

export interface Portfolio{
    id: number;
  name: string;
  description?: string;
  userId: number;
  createdAt: string;
  accounts?: Account[];
  holdings?: Holding[];
}
export interface Account {
  id: number;
  name: string;
  accountType: string;
  portfolioId: number;
  createdAt: string;
  transactions?: Transaction[];
}

export interface Transaction {
  id: number;
  accountId: number;
  transactionType: string;
  symbol?: string;
  quantity: number;
  price: number;
  totalAmount: number;
  transactionDate: string;
  notes?: string;
}

export interface Holding {
  id: number;
  portfolioId: number;
  symbol: string;
  quantity: number;
  averageCost: number;
  currentPrice: number;
  lastUpdated: string;
  // Calculated properties
  totalCost: number;
  currentValue: number;
  gainLoss: number;
  gainLossPercentage: number;
}

// API Response Types
export interface PortfolioSummary {
  portfolioId: number;
  portfolioName: string;
  totalValue: number;
  totalCost: number;
  totalGainLoss: number;
  totalGainLossPercentage: number;
  totalHoldings: number;
  holdings: HoldingSummary[];
}

export interface HoldingSummary {
  symbol: string;
  quantity: number;
  averageCost: number;
  currentPrice: number;
  currentValue: number;
  gainLoss: number;
  gainLossPercentage: number;
}