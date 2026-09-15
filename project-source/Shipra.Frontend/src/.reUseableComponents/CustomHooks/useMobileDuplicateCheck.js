import { useState, useEffect } from "react";
import { CheckMobileNoDuplicate } from "../../api/AxiosInterceptors";

/**
 * Custom hook to validate and check duplicate mobile number orders dynamically
 * @param {string} mobileNo - The mobile number to check
 * @param {number} minLength - The minimum length to trigger API lookup (default 8)
 * @param {number} delay - Debounce delay in milliseconds (default 600)
 */
export default function useOrderMobileDuplicateCheck(mobileNo, minLength = 8, delay = 600) {
  const [warningState, setWarningState] = useState({
    isDuplicate: false,
    orderNo: "",
    daysAgo: 0,
  });

  useEffect(() => {
    if (!mobileNo || mobileNo.length < minLength) {
      setWarningState({ isDuplicate: false, orderNo: "", daysAgo: 0 });
      return;
    }

    const timer = setTimeout(async () => {
      try {
        const res = await CheckMobileNoDuplicate(mobileNo);
        if (res?.data?.isSuccess && res.data.result?.isDuplicate) {
          setWarningState({
            isDuplicate: true,
            orderNo: res.data.result.orderNo,
            daysAgo: res.data.result.daysAgo,
          });
        } else {
          setWarningState({ isDuplicate: false, orderNo: "", daysAgo: 0 });
        }
      } catch (err) {
        console.error("Error checking mobile duplicate:", err);
      }
    }, delay);

    return () => clearTimeout(timer);
  }, [mobileNo, minLength, delay]);

  return warningState;
}
