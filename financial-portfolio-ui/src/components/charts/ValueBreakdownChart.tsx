import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

interface HoldingValue {
  symbol: string;
  currentValue: number;
  totalCost: number;
  gainLoss: number;
}

interface ValueBreakdownChartProps {
  data: HoldingValue[];
}

export const ValueBreakdownChart = ({ data }: ValueBreakdownChartProps) => {
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(value);
  };

  // Sort by current value (descending)
  const sortedData = [...data].sort((a, b) => b.currentValue - a.currentValue);

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h3 className="text-lg font-bold text-gray-800 mb-4">Holdings Value Breakdown</h3>
      
      {sortedData.length === 0 ? (
        <div className="flex items-center justify-center h-64">
          <p className="text-gray-500">No holdings to display</p>
        </div>
      ) : (
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={sortedData}>
            <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
            <XAxis 
              dataKey="symbol"
              stroke="#6b7280"
              style={{ fontSize: '12px', fontWeight: '600' }}
            />
            <YAxis 
              tickFormatter={formatCurrency}
              stroke="#6b7280"
              style={{ fontSize: '12px' }}
            />
            <Tooltip
              formatter={(value: number) => formatCurrency(value)}
              contentStyle={{
                backgroundColor: 'white',
                border: '1px solid #e5e7eb',
                borderRadius: '8px',
                padding: '12px',
              }}
            />
            <Legend />
            <Bar 
              dataKey="currentValue" 
              fill="#3b82f6" 
              name="Current Value"
              radius={[4, 4, 0, 0]}
            />
            <Bar 
              dataKey="totalCost" 
              fill="#94a3b8" 
              name="Total Cost"
              radius={[4, 4, 0, 0]}
            />
          </BarChart>
        </ResponsiveContainer>
      )}
    </div>
  );
};