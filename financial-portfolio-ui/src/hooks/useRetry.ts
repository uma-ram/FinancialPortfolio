import { useState, useCallback } from 'react';

interface UseRetryOptions<T> {
  fetchFn: () => Promise<T>;
  maxRetries?: number;
  retryDelay?: number;
}

export function useRetry<T>({
  fetchFn,
  maxRetries = 3,
  retryDelay = 1000,
}: UseRetryOptions<T>) {
  const [retryCount, setRetryCount] = useState(0);

  const fetchWithRetry = useCallback(async (): Promise<T> => {
    let lastError: Error | null = null;

    for (let attempt = 0; attempt <= maxRetries; attempt++) {
      try {
        const result = await fetchFn();
        setRetryCount(0); // Reset on success
        return result;
      } catch (error: any) {
        lastError = error;
        setRetryCount(attempt + 1);

        if (attempt < maxRetries) {
          console.log(`Retry attempt ${attempt + 1}/${maxRetries}...`);
          await new Promise((resolve) => setTimeout(resolve, retryDelay * (attempt + 1)));
        }
      }
    }

    throw lastError || new Error('Max retries exceeded');
  }, [fetchFn, maxRetries, retryDelay]);

  return { fetchWithRetry, retryCount };
}