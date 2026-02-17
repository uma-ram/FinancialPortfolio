import { useState, useEffect, useCallback } from 'react';

interface UseDataFetchingOptions<T> {
  fetchFn: () => Promise<T>;
  dependencies?: any[];
  autoFetch?: boolean;
}

interface UseDataFetchingResult<T> {
  data: T | null;
  loading: boolean;
  error: string | null;
  refetch: () => Promise<void>;
}

export function useDataFetching<T>({
  fetchFn,
  dependencies = [],
  autoFetch = true,
}: UseDataFetchingOptions<T>): UseDataFetchingResult<T> {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(autoFetch);
  const [error, setError] = useState<string | null>(null);

  // useCallback prevents function recreation on every render
  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await fetchFn();
      setData(result);
    } catch (err: any) {
      console.error('Error fetching data:', err);
      setError(err.message || 'Failed to fetch data');
    } finally {
      setLoading(false);
    }
  }, [fetchFn]);

  useEffect(() => {
    if (autoFetch) {
      fetchData();
    }
  }, [...dependencies, fetchData, autoFetch]);

  return { data, loading, error, refetch: fetchData };
}