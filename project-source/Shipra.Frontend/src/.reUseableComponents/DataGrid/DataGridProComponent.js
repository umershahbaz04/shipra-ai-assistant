import { TablePagination, Typography } from "@mui/material";
import Box from "@mui/material/Box";
import { DataGridPro } from "@mui/x-data-grid-pro";
import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  getElementByInnerHTML,
  grey,
  handleDispatIsApiFilterModelFlag,
} from "../../utilities/helpers/Helpers";
import { GridExpandMoreIcon } from "@mui/x-data-grid-pro";

export default function DataGridProComponent({
  columns,
  rows,
  getRowId,
  selectionModel,
  onSelectionModelChange,
  loading,
  checkboxSelection,
  columnVisibilityModel,
  onColumnVisibilityModelChange = () => {},
  rowsCount = 0,
  paginationMethodUrl = "",
  paginationChangeMethod = async () => {},
  defaultRowsPerPage,
  paddingBottom = 2,
  height = 540,
  getRowClassName,
  showRowsPerPageOptions = true,
  rowPadding = 0,
  enableMasterDetail = false,
  getDetailPanelContent,
  getDetailPanelHeight = () => 200,
  detailPanelExpandedRowIds,
  onDetailPanelExpandedRowIdsChange,
  isRowSelectable,
  keepNonExistentRowsSelected = false,
}) {
  const dispatch = useDispatch();
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const [flag, setFlag] = useState(false);
  const [showContent, setShowContent] = useState(false);
  const [_loading, setLoading] = useState(false);
  const [paginationModel, setPaginationModel] = useState({
    pageSize: defaultRowsPerPage,
    page: 0,
  });

  useEffect(() => {
    if (paginationMethodUrl) {
      handleDispatIsApiFilterModelFlag(dispatch, {
        apiName: paginationMethodUrl,
        page: 0,
        pageSize: paginationModel.pageSize || defaultRowsPerPage || 25,
      });
    }
    setPaginationModel((prev) => ({ ...prev, page: 0 }));
  }, [reduxSelectedCountry]);

  const handleChangePage = async (event, newPage) => {
    handleDispatIsApiFilterModelFlag(dispatch, {
      apiName: paginationMethodUrl,
      page: newPage,
      pageSize: paginationModel.pageSize,
    });
    setPaginationModel((prev) => ({ ...prev, page: newPage }));
    setLoading(true);
    await paginationChangeMethod();
    setLoading(false);
  };

  const handleChangeRowsPerPage = async (event) => {
    handleDispatIsApiFilterModelFlag(dispatch, {
      apiName: paginationMethodUrl,
      page: 0,
      pageSize: event.target.value,
    });
    setPaginationModel((prev) => ({
      ...prev,
      page: 0,
      pageSize: event.target.value,
    }));
    setLoading(true);
    await paginationChangeMethod();
    setLoading(false);
  };

  useEffect(() => {
    const selectedElement = getElementByInnerHTML("MUI X Missing license key");
    if (selectedElement) {
      setShowContent(true);
      selectedElement.style.display = "none";
    }
  }, [flag]);

  useEffect(() => {
    setLoading(loading);
    setFlag((prev) => !prev);
  }, [loading]);

  return (
    <Box
      sx={{
        height: height,
        width: "100%",
        position: "relative",
        pb: paddingBottom,
        "& .MuiDataGrid-cell": {
          display: "flex",
          alignItems: "center",
          padding: `${rowPadding}px`,
        },
        "& .MuiDataGrid-main div": {
          opacity: showContent ? 1 : 0,
        },
        "& .MuiDataGrid-virtualScroller": {
          background: "#F8F8F8",
        },
        "& .MuiDataGrid-root": {
          borderRadius: "0px!important",
        },
        "& .MuiTablePagination-root": {
          borderRadius: "0px 0px 8px 8px!important",
        },
      }}
    >
      <DataGridPro
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        getRowHeight={() => "auto"}
        columnHeaderHeight={40}
        columns={columns}
        rows={rows}
        loading={_loading}
        checkboxSelection={checkboxSelection}
        disableRowSelectionOnClick
        sortingMode="client"
        rowSelectionModel={selectionModel}
        isRowSelectable={isRowSelectable}
        onRowSelectionModelChange={(_rows_) => {
          if (_rows_.length === rows.length) {
            const _rows = _rows_.reverse();
            onSelectionModelChange(_rows);
          } else {
            onSelectionModelChange(_rows_);
          }
        }}
        getRowId={getRowId}
        getRowClassName={getRowClassName}
        columnVisibilityModel={columnVisibilityModel}
        onColumnVisibilityModelChange={onColumnVisibilityModelChange}
        keepNonExistentRowsSelected={keepNonExistentRowsSelected}
        hideFooter
        {...(enableMasterDetail && {
          getDetailPanelContent,
          getDetailPanelHeight,
          detailPanelExpandedRowIds: detailPanelExpandedRowIds ?? [],
          onDetailPanelExpandedRowIdsChange,
        })}
      />
      <Box position={"relative"}>
        <TablePagination
          sx={{
            border: "1px solid rgba(224, 224, 224, 1)",
          }}
          component="div"
          count={rowsCount}
          page={paginationModel.page}
          onPageChange={handleChangePage}
          rowsPerPage={paginationModel.pageSize}
          rowsPerPageOptions={
            showRowsPerPageOptions ? [10, 100, 500, 1000, 5000, 10000] : []
          }
          onRowsPerPageChange={handleChangeRowsPerPage}
        />
        {selectionModel?.length > 0 && (
          <Typography
            sx={{
              position: "absolute",
              top: "30%",
              left: "1.9%",
              fontSize: "14px",
            }}
          >
            {selectionModel.length} row{selectionModel?.length > 1 && "s"}{" "}
            selected
          </Typography>
        )}
      </Box>
    </Box>
  );
}
