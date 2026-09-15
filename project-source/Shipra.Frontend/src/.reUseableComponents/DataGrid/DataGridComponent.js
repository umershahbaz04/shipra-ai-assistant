import React, { useEffect, useState } from "react";
import { Box } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { usePagination } from "../../utilities/helpers/Helpers";
import BorderBottom from '@mui/icons-material/BorderBottom';
export default function DataGridComponent(props) {
  const {
    columns,
    rows,
    getRowId,
    getOrdersRef,
    resetRowRef,
    getRowHeight,
    checkboxSelection = false,
    bgColor = "#f5f5f5",
    borderRadius = "0px 0px 8px 8px",
    footerHeight = 52,
    rowHeight,
    rowPerPage,
    ...other
  } = props;
  const styles = {
    analyticsOrderTable: {
      fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
      height: !other.autoHeight && (other.height || "70vh"),
      width: "100%",
      "& .MuiCheckbox-root": {
        color: "var(--primary-color) !important",
      },
      "& .MuiDataGrid-root": {
        borderRadius: borderRadius,
      },
      "& .MuiDataGrid-main": {
        background: bgColor,
      },
      "& .MuiDataGrid-footerContainer": {
        background: "#fff",
        minHeight: footerHeight,
        height: footerHeight,
        BorderBottom: "1px none!important",
        borderRadius: "0px 0px 8px 8px",
      },
      "& .Mui-selected": {
        background: "#E6E1FD !important",
      },
      "& .MuiDataGrid-columnHeaders": {
        background: "#fff !important",
      },
    },
  };
  const getRowClassName = (params) => {
    // Apply "striped" class to alternate rows
    return params.id % 2 === 0 ? "striped" : "";
  };
  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, rowPerPage ? rowPerPage : 10);
  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };
  ////////////

  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef?.current]);

  return (
    <Box sx={styles.analyticsOrderTable}>
      <DataGrid
        rowHeight={rowHeight}
        getRowHeight={getRowHeight}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowId={getRowId}
        rows={rows ? rows?.list || rows : []}
        columns={columns}
        checkboxSelection={checkboxSelection}
        disableSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        // getRowClassName={getRowClassName}
        selectionModel={selectionModel}
        onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
        {...other}
      />
    </Box>
  );
}
