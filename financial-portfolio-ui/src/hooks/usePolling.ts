import  { useEffect, useRef} from 'react';

interface UsePollingOptions {
  callback: () => void | Promise<void>;
  interval: number; // milliseconds
  enabled?: boolean;
}
export function usePolling({ callback, interval, enabled = true }: UsePollingOptions) {
  const savedCallback = useRef(callback);

  // Remember the latest callback
  useEffect(() => {
    savedCallback.current = callback;
  }, [callback]);

  // Set up the interval
  useEffect(() => {
    if (!enabled) return;

    const tick = () => {
      savedCallback.current();
    };

    const id = setInterval(tick, interval);
    return () => clearInterval(id);
  }, [interval, enabled]);
}