import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

interface PerformanceDataPoint {
  date: string;
  value: number;
  cost: number;
}

interface PortfolioPerformanceChartProps {
  data: PerformanceDataPoint[];
}

export const PortfolioPerformanceChart = ({ data }: PortfolioPerformanceChartProps) => {
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(value);
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h3 className="text-lg font-bold text-gray-800 mb-4">Portfolio Performance</h3>
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
          <XAxis 
            dataKey="date" 
            tickFormatter={formatDate}
            stroke="#6b7280"
            style={{ fontSize: '12px' }}
          />
          <YAxis 
            tickFormatter={formatCurrency}
            stroke="#6b7280"
            style={{ fontSize: '12px' }}
          />
          <Tooltip
            formatter={(value: number) => formatCurrency(value)}
            labelFormatter={formatDate}
            contentStyle={{
              backgroundColor: 'white',
              border: '1px solid #e5e7eb',
              borderRadius: '8px',
              padding: '12px',
            }}
          />
          <Legend 
            wrapperStyle={{ paddingTop: '20px' }}
            iconType="line"
          />
          <Line
            type="monotone"
            dataKey="value"
            stroke="#3b82f6"
            strokeWidth={3}
            name="Current Value"
            dot={{ fill: '#3b82f6', r: 4 }}
            activeDot={{ r: 6 }}
          />
          <Line
            type="monotone"
            dataKey="cost"
            stroke="#94a3b8"
            strokeWidth={2}
            strokeDasharray="5 5"
            name="Total Cost"
            dot={{ fill: '#94a3b8', r: 3 }}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};2