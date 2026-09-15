import React, { useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "@mui/material/colors";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import {
  centerColumn,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import { usePagination } from "@mui/lab";
import { Box, Chip } from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import { DataGrid } from "@mui/x-data-grid";
import UtilityClass from "../../../utilities/UtilityClass";
import StatusBadge from "../../shared/statudBadge";

const SetupCustomDomainModal = (props) => {
  const { open, onClose, getAllCustomDomains, setupDomainData } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
    control,
  } = useForm();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);

  const handle = () => {
    // console.log(setupDomainData);
  };

  const columns = [
    {
      ...centerColumn,
      field: "recordName",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"record Name"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.recordName}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "recordValue",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"record Value"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.recordValue}</Box>;
      },
      flex: 1.5,
    },
    {
      ...centerColumn,
      field: "recordTypeValue",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"record Type Value"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.recordTypeValue}</Box>;
      },
      flex: 0.5,
    },
    {
      ...centerColumn,
      field: "isVerified",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>Verified</Box>,
      renderCell: ({ row }) => {
        const isTrue = row?.isVerified === true;
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={isTrue ? "True" : "Flase"}
                color={"#fff"}
                bgColor={isTrue ? "#28a745" : "#dc3545;"}
              />
            </>
          </Box>
        );
      },
      flex: 0.5,
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 150,
      headerName: <Box sx={{ fontWeight: "600" }}>{"created On"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.createdOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "verifiedOn",
      minWidth: 150,
      headerName: <Box sx={{ fontWeight: "600" }}>{"verified On"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.verifiedOn)}</Box>
        );
      },
      flex: 1,
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Setup Domain"}
      actionBtn={
        <ModalButtonComponent
          title={"Update"}
          bg={purple}
          loading={loading}
          onClick={handleSubmit(handle)}
        />
      }
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem sm={12} md={12} lg={12}></GridItem>
        <Box sx={styleSheet.allOrderTable}>
          <DataGrid
            loading={loading}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.customDomainRecordId}
            rows={setupDomainData}
            columns={columns}
            disableSelectionOnClick
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
        </Box>
      </GridContainer>
    </ModalComponent>
  );
};

export default SetupCustomDomainModal;
