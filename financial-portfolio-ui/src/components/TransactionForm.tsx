import { useState } from 'react';
import type { CreateTransactionRequest } from '../types';
import { Loader2, PoundSterling, TrendingUp, TrendingDown } from 'lucide-react';

interface TransactionFormProps {
  accountId: number;
  onSuccess?: () => void;
  onCancel?: () => void;
}

export const TransactionForm = ({ accountId, onSuccess, onCancel }: TransactionFormProps) => {
  const [formData, setFormData] = useState<CreateTransactionRequest>({
    accountId: accountId,
    transactionType: 'Buy',
    symbol: '',
    quantity: 0,
    price: 0,
    notes:'',

  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  // Handle input changes
  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    
    // Convert to number for quantity and price
    const newValue = name === 'quantity' || name === 'price' 
      ? parseFloat(value) || 0 
      : value;

    setFormData((prev) => ({
      ...prev,
      [name]: newValue,
    }));

    // Clear error for this field when user types
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };
  // Validate form
  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    // Validate transaction type
    if (!formData.transactionType) {
      newErrors.transactionType = 'Transaction type is required';
    }

    // Validate symbol for Buy/Sell
    if ((formData.transactionType === 'Buy' || formData.transactionType === 'Sell')) {
      if (!formData.symbol || formData.symbol.trim() === '') {
        newErrors.symbol = 'Stock symbol is required for Buy/Sell transactions';
      } else if (!/^[A-Z]{1,5}$/.test(formData.symbol.toUpperCase())) {
        newErrors.symbol = 'Enter a valid stock symbol (1-5 uppercase letters)';
      }
    }

    // Validate quantity
    if (formData.quantity <= 0) {
      newErrors.quantity = 'Quantity must be greater than 0';
    }

    // Validate price
    if (formData.price <= 0) {
      newErrors.price = 'Price must be greater than 0';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle submit
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setLoading(true);
    setSubmitError(null);

    try {
      // Import portfolioApi at the top of the file
      const { portfolioApi } = await import('../services/api');
      
      // Create transaction
      await portfolioApi.createTransaction({
        ...formData,
        symbol: formData.symbol?.toUpperCase(), // Ensure uppercase
      });

      // Success!
      if (onSuccess) {
        onSuccess();
      }

      // Reset form
      setFormData({
        accountId: accountId,
        transactionType: 'Buy',
        symbol: '',
        quantity: 0,
        price: 0,
        notes: '',
      });
    } catch (err: any) {
      console.error('Error creating transaction:', err);
      setSubmitError(
        err.response?.data?.message || 
        err.message || 
        'Failed to create transaction'
      );
    } finally {
      setLoading(false);
    }
  };

  const totalAmount = formData.quantity * formData.price;
  const isBuyOrSell = formData.transactionType === 'Buy' || formData.transactionType === 'Sell';

  return (
    <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">New Transaction</h2>

      {/* Error Alert */}
      {submitError && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-800 text-sm">{submitError}</p>
        </div>
      )}

      {/* Transaction Type */}
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Transaction Type *
        </label>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
          {['Buy', 'Sell', 'Deposit', 'Withdrawal'].map((type) => (
            <button
              key={type}
              type="button"
              onClick={() => setFormData({ ...formData, transactionType: type })}
              className={`px-4 py-3 rounded-lg border-2 transition-all font-medium ${
                formData.transactionType === type
                  ? type === 'Buy' || type === 'Deposit'
                    ? 'bg-green-50 border-green-500 text-green-700'
                    : 'bg-red-50 border-red-500 text-red-700'
                  : 'bg-white border-gray-200 text-gray-700 hover:border-gray-300'
              }`}
            >
              {type === 'Buy' && <TrendingUp className="w-4 h-4 inline mr-1" />}
              {type === 'Sell' && <TrendingDown className="w-4 h-4 inline mr-1" />}
              {type === 'Deposit' && <PoundSterling className="w-4 h-4 inline mr-1" />}
              {type === 'Withdrawal' && <PoundSterling className="w-4 h-4 inline mr-1" />}
              {type}
            </button>
          ))}
        </div>
        {errors.transactionType && (
          <p className="mt-1 text-sm text-red-600">{errors.transactionType}</p>
        )}
      </div>

      {/* Symbol (only for Buy/Sell) */}
      {isBuyOrSell && (
        <div className="mb-4">
          <label htmlFor="symbol" className="block text-sm font-medium text-gray-700 mb-2">
            Stock Symbol *
          </label>
          <input
            type="text"
            id="symbol"
            name="symbol"
            value={formData.symbol}
            onChange={handleChange}
            placeholder="e.g., AAPL, MSFT, GOOGL"
            className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent uppercase ${
              errors.symbol ? 'border-red-500' : 'border-gray-300'
            }`}
            maxLength={5}
          />
          {errors.symbol && (
            <p className="mt-1 text-sm text-red-600">{errors.symbol}</p>
          )}
        </div>
      )}

      {/* Quantity and Price */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
        {/* Quantity */}
        <div>
          <label htmlFor="quantity" className="block text-sm font-medium text-gray-700 mb-2">
            Quantity {isBuyOrSell ? '(Shares)' : '(Amount)'} *
          </label>
          <input
            type="number"
            id="quantity"
            name="quantity"
            value={formData.quantity || ''}
            onChange={handleChange}
            step="0.01"
            min="0"
            placeholder="0.00"
            className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
              errors.quantity ? 'border-red-500' : 'border-gray-300'
            }`}
          />
          {errors.quantity && (
            <p className="mt-1 text-sm text-red-600">{errors.quantity}</p>
          )}
        </div>

        {/* Price */}
        <div>
          <label htmlFor="price" className="block text-sm font-medium text-gray-700 mb-2">
            Price {isBuyOrSell ? '(Per Share)' : '(Total)'} *
          </label>
          <div className="relative">
            <span className="absolute left-3 top-2.5 text-gray-500">$</span>
            <input
              type="number"
              id="price"
              name="price"
              value={formData.price || ''}
              onChange={handleChange}
              step="0.01"
              min="0"
              placeholder="0.00"
              className={`w-full pl-8 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${
                errors.price ? 'border-red-500' : 'border-gray-300'
              }`}
            />
          </div>
          {errors.price && (
            <p className="mt-1 text-sm text-red-600">{errors.price}</p>
          )}
        </div>
      </div>

      {/* Total Amount Display */}
      {totalAmount > 0 && (
        <div className="mb-4 p-4 bg-blue-50 border border-blue-200 rounded-lg">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium text-blue-900">Total Amount:</span>
            <span className="text-2xl font-bold text-blue-900">
              ${totalAmount.toFixed(2)}
            </span>
          </div>
        </div>
      )}

      {/* Notes */}
      <div className="mb-6">
        <label htmlFor="notes" className="block text-sm font-medium text-gray-700 mb-2">
          Notes (Optional)
        </label>
        <textarea
          id="notes"
          name="notes"
          value={formData.notes}
          onChange={handleChange}
          rows={3}
          placeholder="Add any notes about this transaction..."
          className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </div>

      {/* Action Buttons */}
      <div className="flex items-center justify-end space-x-3">
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            disabled={loading}
            className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
        )}
        <button
          type="submit"
          disabled={loading}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 flex items-center"
        >
          {loading ? (
            <>
              <Loader2 className="w-4 h-4 mr-2 animate-spin" />
              Creating...
            </>
          ) : (
            'Create Transaction'
          )}
        </button>
      </div>
    </form>
  );
};