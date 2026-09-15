import {
  endOfMonth,
  endOfWeek,
  startOfMonth,
  startOfWeek,
  subDays,
} from "date-fns";
import { useState } from "react";
import { DateRangePicker } from "react-date-range";
import "react-date-range/dist/styles.css"; // main style file
import "react-date-range/dist/theme/default.css"; // the default theme
import { purple } from "../../utilities/helpers/Helpers";
import moment from "moment";

export default function DateRangePickerComponent(props) {
  const { graphFilterModal, setGraphFilterModal } = props;
  const staticRanges = [
    {
      label: "Yesterday",
      isSelected: () => {},
      range: () => ({
        startDate: subDays(new Date(), 1),
        endDate: subDays(new Date(), 1),
      }),
    },
    {
      label: "Today",
      isSelected: () => {},
      range: () => ({ startDate: new Date(), endDate: new Date() }),
    },
    {
      label: "Last Week",
      isSelected: () => {},
      range: () => ({
        startDate: startOfWeek(subDays(new Date(), 7)),
        endDate: endOfWeek(subDays(new Date(), 7)),
      }),
    },
    {
      label: "This Week",
      isSelected: () => {},
      range: () => ({
        startDate: startOfWeek(new Date()),
        endDate:
          endOfWeek(new Date()) === new Date()
            ? endOfWeek(new Date())
            : new Date(),
      }),
    },
    {
      label: "Last Month",
      isSelected: () => {},
      range: () => ({
        startDate: startOfMonth(subDays(new Date(), 30)),
        endDate: endOfMonth(subDays(new Date(), 30)),
      }),
    },
    {
      label: "This Month",
      isSelected: () => {},
      range: () => ({
        startDate: startOfMonth(new Date()),
        endDate:
          endOfMonth(new Date()) === new Date()
            ? endOfMonth(new Date())
            : new Date(),
      }),
    },
  ];
  const [state, setState] = useState([
    {
      startDate: graphFilterModal.createdFrom,
      endDate: graphFilterModal.createdTo,
      key: "selection",
    },
  ]);

  return (
    <DateRangePicker
      onChange={(date) => {
        setState([date.selection]);
        setGraphFilterModal((prev) => ({
          ...prev,
          createdFrom: date.selection.startDate,
          createdTo: date.selection.endDate,
        }));
      }}
      ranges={state}
      showPreview={false}
      editableDateInputs={true}
      showDateDisplay={true}
      dateDisplayFormat={"dd/MM/yyyy"}
      staticRanges={staticRanges}
      rangeColors={[purple]}
      maxDate={new Date()}
      inputRanges={[]}
    />
  );
}
