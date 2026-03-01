import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, Cell } from 'recharts';

interface HoldingPerformance {
  symbol: string;
  gainLoss: number;
  gainLossPercentage: number;
  currentValue: number;
}

interface HoldingsPerformanceChartProps {
  data: HoldingPerformance[];
}

export const HoldingsPerformanceChart = ({ data }: HoldingsPerformanceChartProps) => {
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(Math.abs(value));
  };

  const formatPercentage = (value: number) => {
    return `${value >= 0 ? '+' : ''}${value.toFixed(2)}%`;
  };

  // Sort by gain/loss percentage (descending)
  const sortedData = [...data].sort((a, b) => b.gainLossPercentage - a.gainLossPercentage);

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h3 className="text-lg font-bold text-gray-800 mb-4">Holdings Performance</h3>
      
      {sortedData.length === 0 ? (
        <div className="flex items-center justify-center h-64">
          <p className="text-gray-500">No holdings to display</p>
        </div>
      ) : (
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={sortedData} layout="vertical">
            <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
            <XAxis 
              type="number" 
              tickFormatter={formatPercentage}
              stroke="#6b7280"
              style={{ fontSize: '12px' }}
            />
            <YAxis 
              type="category" 
              dataKey="symbol"
              stroke="#6b7280"
              style={{ fontSize: '12px', fontWeight: '600' }}
              width={60}
            />
            <Tooltip
              formatter={(value: number, name: string) => {
                if (name === 'Gain/Loss %') {
                  return formatPercentage(value);
                }
                return formatCurrency(value);
              }}
              contentStyle={{
                backgroundColor: 'white',
                border: '1px solid #e5e7eb',
                borderRadius: '8px',
                padding: '12px',
              }}
            />
            <Bar dataKey="gainLossPercentage" name="Gain/Loss %" radius={[0, 4, 4, 0]}>
              {sortedData.map((entry, index) => (
                <Cell 
                  key={`cell-${index}`} 
                  fill={entry.gainLossPercentage >= 0 ? '#10b981' : '#ef4444'} 
                />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      )}
    </div>
  );
};