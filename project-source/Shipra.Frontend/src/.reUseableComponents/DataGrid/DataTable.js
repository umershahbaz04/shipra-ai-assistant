import { Box } from "@mui/material";
import Paper from "@mui/material/Paper";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import React, { useEffect, useRef, useState } from "react";
import ReactDOM from "react-dom/client";
import { Circles } from "react-loader-spinner";
import { grey, greyBorder, purple } from "../../utilities/helpers/Helpers";

const getSliceStart = (page) => {
  return (page - 1) * 100;
};

const getArrayOfCheckedRowsIds = (data = {}) => {
  return Object.values(data).filter((val) => val === true);
};

const CheckboxComp = ({ onClick }) => {
  return <input type="checkbox" onClick={onClick} />;
};

const scrolledCheckboxValue = () => {
  const tbody = document.getElementById("table-body");
  const trsWithUncheckedCheckboxes = tbody.querySelectorAll(
    "tr input[type='checkbox']:not(:checked)"
  );
  trsWithUncheckedCheckboxes.forEach((tr) => {
    tr.checked = true;
  });
};

const Loading = () => {
  return (
    <Box
      sx={{
        width: "100%",
        height: "100%",
      }}
      className={"flex_center"}
    >
      <Circles type="Circles" width={100} height={100} color={purple} />
    </Box>
  );
};

export default function DataTable(props) {
  const {
    columns,
    rows,
    checkboxSelection = false,
    rowId,
    loading = true,
  } = props;
  const [currPage, setCurrPage] = useState(1);
  const [selectedRows, setSelectedRows] = useState({});
  const tableRef = useRef(null);
  const checkedAll = useRef(null);

  // handleChangeSelectAll
  const handleChangeSelectAll = (e) => {
    const value = e.target.checked;
    checkedAll.current = value;
    const tbody = document.getElementById("table-body");
    const trElements = tbody.querySelectorAll("tr");
    if (value) {
      trElements.forEach((tr) => {
        const checkbox = tr.querySelector('input[type="checkbox"]');
        checkbox.checked = value;
        tr.classList.add("selected_row");
      });
      const _selectedAllRows = {};
      rows.forEach((dt) => {
        _selectedAllRows[dt[rowId]] = true;
      });
      setSelectedRows({ ..._selectedAllRows });
    } else {
      trElements.forEach((tr) => {
        const checkbox = tr.querySelector('input[type="checkbox"]');
        checkbox.checked = value;
        tr.classList.remove("selected_row");
      });
      setSelectedRows({});
    }
  };
  // handleChangeSelectSingle
  const handleChangeSelectSingle = (e, rowId) => {
    const value = e.target.checked;
    const tr = document.getElementById(rowId);
    if (value) {
      tr.classList.add("selected_row");
    } else {
      tr.classList.remove("selected_row");
    }
    setSelectedRows((prev) => ({
      ...prev,
      [rowId]: value,
    }));
  };
  // handleScroll
  function handleScroll() {
    const _checkedAll = checkedAll.current;
    if (_checkedAll) {
      scrolledCheckboxValue();
    }
    const bottom =
      tableRef.current.scrollHeight - tableRef.current.scrollTop - 100 <
      tableRef.current.clientHeight;
    if (bottom) {
      const _currPage = currPage + 1;
      setCurrPage(_currPage);
      renderTableRows(getSliceStart(_currPage));
    }
  }
  // renderTableHeaders
  function renderTableHeaders(columns) {
    const thead = document.getElementById("table-head");
    thead.innerHTML = "";
    const row = document.createElement("tr");
    row.className = "MuiTableRow-root";
    row.style.background = grey;
    columns.forEach((col, col_index) => {
      row.className = "MuiTableRow-root";
      if (!col.hide) {
        const cell = document.createElement("td");
        cell.className = "MuiTableCell-root";
        cell.style.textAlign = col.headerAlign;
        if (col_index === 0 && checkboxSelection) {
          const checkboxContainer = document.createElement("div");
          const checkbox = ReactDOM.createRoot(checkboxContainer);
          checkbox.render(<CheckboxComp onClick={handleChangeSelectAll} />);
          cell.appendChild(checkboxContainer);
        } else {
          const headerContainer = document.createElement("div");
          const header = ReactDOM.createRoot(headerContainer);
          header.render(col.headerName);
          cell.appendChild(headerContainer);
        }
        row.appendChild(cell);
      }
    });
    thead.appendChild(row);
  }
  // renderTableRows
  function renderTableRows(startFrom, empty_tbody) {
    const tbody = document.getElementById("table-body");
    if (empty_tbody) {
      tbody.innerHTML = "";
    }
    rows.slice(startFrom, startFrom + 100).forEach((record) => {
      const row = document.createElement("tr");
      row.id = record[rowId];
      row.classList.add("MuiTableRow-root");
      if (selectedRows[record[rowId]]) {
        row.classList.add("selected_row");
      }
      columns.forEach((col, col_ind) => {
        if (!col.hide) {
          const cell = document.createElement("td");
          cell.className = "MuiTableCell-root";
          cell.style.textAlign = col.align;
          if (col_ind === 0 && checkboxSelection) {
            const checkboxContainer = document.createElement("div");
            const checkbox = ReactDOM.createRoot(checkboxContainer);
            checkbox.render(
              <CheckboxComp
                onClick={(e) => handleChangeSelectSingle(e, record[rowId])}
              />
            );
            cell.appendChild(checkboxContainer);
          } else {
            // Render cell content using ReactDOM
            const cellContent = document.createElement("div");
            const root = ReactDOM.createRoot(cellContent);
            root.render(col.renderCell({ row: record }));
            cell.appendChild(cellContent);
          }

          row.appendChild(cell);
        }
      });
      tbody.appendChild(row);
    });
  }

  useEffect(() => {
    renderTableHeaders(columns);
    renderTableRows(getSliceStart(currPage), true);
  }, [rows]);

  return (
    <>
      <Box
        component={Paper}
        sx={{
          width: "100%",
          height: "100%",
          border: greyBorder,
          fontSize: "12px !important",
          background: grey,
          position: "relative",
        }}
        elevation={0}
      >
        <TableContainer
          sx={{
            maxHeight: 600,
            "& .MuiTableCell-root": {
              fontSize: "12px",
              padding: "2px 16px",
            },
          }}
          onScroll={handleScroll}
          ref={tableRef}
        >
          <Table size="small" stickyHeader aria-label="a dense table">
            <TableHead
              id="table-head"
              sx={{ position: "sticky", top: 0, zIndex: 100 }}
            ></TableHead>
            <TableBody id="table-body"></TableBody>
          </Table>
        </TableContainer>
        {loading ? <Loading /> : <></>}
      </Box>
    </>
  );
}
