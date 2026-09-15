import { useCallback, useState } from "react";
import UtilityClass from "../../utilities/UtilityClass";
import moment from "moment";
import "moment-timezone";
const useDateRangeHook = () => {
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);

  //get only date
  const [startDateFormated, setStartDateFormated] = useState(null);
  const [endDateFormated, setEndDateFormated] = useState(null);

  //get date with utc formaete
  const [startDateUtcFormated, setStartDateUtcFormated] = useState(null);
  const [endDateUtcFormated, setEndDateUtcFormated] = useState(null);

  // const getFormatedDate = (date) => {
  //   const formattedDate = date?.toLocaleDateString(
  //     UtilityClass.toLocaleDateStringFormate()
  //   );

  //   return formattedDate;
  // };
  const getUTCDifrrenceDateTime = (specificDate) => {
    return specificDate;
  };
  const handleStartDateChange = useCallback((date) => {
    // You can perform any custom logic here if needed
    setStartDate(date);
    if (date) {
      setStartDateUtcFormated(getUTCDifrrenceDateTime(date));
    }
    setStartDateFormated(UtilityClass.getFormatedDateWithoutTime(date));
  }, []);

  const handleEndDateChange = (date) => {
    // You can perform any custom logic here if needed
    setEndDate(date);
    setEndDateUtcFormated(getUTCDifrrenceDateTime(date));
    setEndDateFormated(UtilityClass.getFormatedDateWithoutTime(date));
  };

  const resetDates = () => {
    setStartDate(null);
    setEndDate(null);
    setStartDateFormated(null);
    setEndDateFormated(null);
    setEndDateUtcFormated(null);
    setStartDateUtcFormated(null);
  };

  return {
    startDate,
    endDate,
    setStartDate: handleStartDateChange,
    setEndDate: handleEndDateChange,
    resetDates,
    startDateFormated,
    endDateFormated,
    startDateUtcFormated,
    endDateUtcFormated,
  };
};
export default useDateRangeHook;
