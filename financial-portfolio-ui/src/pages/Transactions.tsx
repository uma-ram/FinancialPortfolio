import { useState, useEffect } from 'react';
import type{ Transaction, Account } from '../types';
import { portfolioApi } from '../services/api';
import { TransactionForm } from '../components/TransactionForm';
import { Toast } from '../components/Toast';

import { 
  Loader2, 
  AlertCircle, 
  Plus, 
  TrendingUp, 
  TrendingDown, 
  PoundSterling,
  Calendar 
} from 'lucide-react';

export const Transactions = () => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [selectedAccountId, setSelectedAccountId] = useState<number | null>(null);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  useEffect(() => {
    fetchData();
  }, []);
  
  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      // Fetch accounts for portfolio 1 (hardcoded for now)
      const accountsData = await portfolioApi.getPortfolioAccounts(1);
      setAccounts(accountsData);

      if (accountsData.length > 0) {
        setSelectedAccountId(accountsData[0].id);        
        // Fetch transactions for first account
        const transactionsData = await portfolioApi.getAccountTransactions(accountsData[0].id);
        setTransactions(transactionsData);
        console.log(typeof transactionsData)
      }
    } catch (err: any) {
      console.error('Error fetching data:', err);
      setError(err.message || 'Failed to fetch data');
    } finally {
      setLoading(false);
    }
  };

  const handleAccountChange = async (accountId: number) => {
    setSelectedAccountId(accountId);
    
    try {
      const transactionsData = await portfolioApi.getAccountTransactions(accountId);
      setTransactions(transactionsData);
    } catch (err: any) {
      console.error('Error fetching transactions:', err);
      setToast({ message: 'Failed to fetch transactions', type: 'error' });
    }
  };

  const handleTransactionSuccess = () => {
    setShowForm(false);
    setToast({ message: 'Transaction created successfully!', type: 'success' });
    fetchData(); // Refresh data
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getTransactionIcon = (type: string) => {
    switch (type) {
      case 'Buy':
        return <TrendingUp className="w-5 h-5 text-green-600" />;
      case 'Sell':
        return <TrendingDown className="w-5 h-5 text-red-600" />;
      case 'Deposit':
        return <PoundSterling className="w-5 h-5 text-blue-600" />;
      case 'Withdrawal':
        return <PoundSterling className="w-5 h-5 text-orange-600" />;
      default:
        return null;
    }
  };

  const getTransactionColor = (type: string) => {
    switch (type) {
      case 'Buy':
      case 'Deposit':
        return 'text-green-600 bg-green-50';
      case 'Sell':
      case 'Withdrawal':
        return 'text-red-600 bg-red-50';
      default:
        return 'text-gray-600 bg-gray-50';
    }
  };

  if (loading) {
    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <Loader2 className="w-12 h-12 animate-spin text-blue-600" />
        </div>
      
    );
  }

  if (error) {
    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="text-center">
            <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
            <p className="text-red-600">{error}</p>
          </div>
        </div>
      
    );
  }

  return (
      <div className="min-h-screen bg-gray-50">
        <div className="container mx-auto px-4 py-8">
          {/* Header */}
          <div className="flex items-center justify-between mb-8">
            <div>
              <h1 className="text-3xl font-bold text-gray-800 mb-2">Transactions</h1>
              <p className="text-gray-600">Manage your investment transactions</p>
            </div>
            <button
              onClick={() => setShowForm(!showForm)}
              className="flex items-center space-x-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <Plus className="w-5 h-5" />
              <span>New Transaction</span>
            </button>
          </div>

          {/* Account Selector */}
            {accounts.length > 0 && (
            <div className="mb-6">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Select Account
              </label>
              <select
                value={selectedAccountId || ''}
                onChange={(e) => handleAccountChange(Number(e.target.value))}
                className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                {accounts.map((account) => (
                  <option key={account.id} value={account.id}>
                    {account.name} ({account.accountType})
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Transaction Form */}
          {showForm && selectedAccountId && (
            <div className="mb-8">
              <TransactionForm
                accountId={selectedAccountId}
                onSuccess={handleTransactionSuccess}
                onCancel={() => setShowForm(false)}
              />
            </div>
          )}

          {/* Transactions List */}
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-xl font-bold text-gray-800">Transaction History</h2>
            </div>

            {
            transactions.length === 0 ? (
              <div className="p-12 text-center">
                <p className="text-gray-600 mb-4">No transactions yet</p>
                <button
                  onClick={() => setShowForm(true)}
                  className="text-blue-600 hover:text-blue-700 font-medium"
                >
                  Create your first transaction
                </button>
              </div>
            ) : (
              <div className="divide-y divide-gray-200">
                {transactions.map((transaction) => (
                  <div
                    key={transaction.id}
                    className="p-6 hover:bg-gray-50 transition-colors"
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex items-start space-x-4">
                        <div className={`p-2 rounded-lg ${getTransactionColor(transaction.transactionType)}`}>
                          {getTransactionIcon(transaction.transactionType)}
                        </div>
                        <div>
                          <div className="flex items-center space-x-2 mb-1">
                            <h3 className="font-semibold text-gray-900">
                              {transaction.transactionType}
                              {transaction.symbol && ` - ${transaction.symbol}`}
                            </h3>
                            <span className={`px-2 py-1 text-xs rounded-full ${getTransactionColor(transaction.transactionType)}`}>
                              {transaction.transactionType}
                            </span>
                          </div>
                          <div className="text-sm text-gray-600 space-y-1">
                            <div className="flex items-center space-x-4">
                              <span>Quantity: {transaction.quantity}</span>
                              <span>Price: {formatCurrency(transaction.price)}</span>
                            </div>
                            <div className="flex items-center space-x-2 text-gray-500">
                              <Calendar className="w-4 h-4" />
                              <span>{formatDate(transaction.transactionDate)}</span>
                            </div>
                            {transaction.notes && (
                              <p className="text-gray-500 italic">{transaction.notes}</p>
                            )}
                          </div>
                        </div>
                      </div>
                      <div className="text-right">
                        <p className="text-2xl font-bold text-gray-900">
                          {formatCurrency(transaction.totalAmount)}
                        </p>
                        {/* <p className="text-sm text-gray-500">{transaction.accountName}</p> */}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
        {/* Toast Notification */}
              {toast && (
                <Toast
                  message={toast.message}
                  type={toast.type}
                  onClose={() => setToast(null)}
                />
              )}

      </div>

      
    
  );
};