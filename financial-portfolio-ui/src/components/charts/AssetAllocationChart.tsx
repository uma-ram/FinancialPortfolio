import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from 'recharts';

interface AllocationData {
  name: string;
  value: number;
  percentage: number;
}

interface AssetAllocationChartProps {
  data: AllocationData[];
}

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];

export const AssetAllocationChart = ({ data }: AssetAllocationChartProps) => {
  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(value);
  };

  const renderCustomLabel = (entry: any) => {
    return `${entry.percentage.toFixed(1)}%`;
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h3 className="text-lg font-bold text-gray-800 mb-4">Asset Allocation</h3>
      
      {data.length === 0 ? (
        <div className="flex items-center justify-center h-64">
          <p className="text-gray-500">No holdings to display</p>
        </div>
      ) : (
        <>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={data}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={renderCustomLabel}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {data.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip
                formatter={(value: number) => formatCurrency(value)}
                contentStyle={{
                  backgroundColor: 'white',
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                  padding: '12px',
                }}
              />
            </PieChart>
          </ResponsiveContainer>

          {/* Legend */}
          <div className="mt-4 grid grid-cols-2 gap-3">
            {data.map((entry, index) => (
              <div key={entry.name} className="flex items-center space-x-2">
                <div
                  className="w-4 h-4 rounded-full"
                  style={{ backgroundColor: COLORS[index % COLORS.length] }}
                />
                <div className="flex-1">
                  <p className="text-sm font-medium text-gray-900">{entry.name}</p>
                  <p className="text-xs text-gray-600">
                    {formatCurrency(entry.value)} ({entry.percentage.toFixed(1)}%)
                  </p>
                </div>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
};