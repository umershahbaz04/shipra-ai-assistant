import React, { useState } from "react";
import MoreVertIcon from "@mui/icons-material/MoreVert";

import {
  Card,
  Dialog,
  DialogContent,
  DialogTitle,
  ButtonBase,
  Button,
  Box,
  Stack,
  Avatar,
  DialogActions,
  IconButton,
  Grid,
} from "@mui/material";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import { styleSheet } from "../../../assets/styles/style";
import Slide from "@mui/material/Slide";
import StatusBadge from "../../shared/statudBadge";
import { useSelector } from "react-redux";
import { DataGrid } from "@mui/x-data-grid";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple, usePagination } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

export default function StoreChannelInfoModal(props) {
  const { open, onClose, data } = props;

  const [anchorEl, setAnchorEl] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    onClose();
  };
  const columns = [
    {
      field: "ChannelName",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Channel Name"}</Box>,
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ fontWeight: "600" }} component={ButtonBase} disableRipple>
            {params.row.ChannelName}
          </Box>
        );
      },
    },
    {
      field: "URL",
      headerName: <Box sx={{ fontWeight: "600" }}>Url</Box>,
      minWidth: 130,
      flex: 1,
    },

    {
      field: "Action",
      hide: true,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            <IconButton onClick={(e) => setAnchorEl(e.currentTarget)}>
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
      minWidth: 60,
      flex: 1,
    },
  ];
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } = usePagination(0,10);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={"Store Channels"}
    >
      <Grid mx={3}>
        <DataGrid
          autoHeight={true}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.ChannelId}
          rows={data}
          columns={columns}
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          // disableSelectionOnClick
          // checkboxSelection
        />
      </Grid>
    </ModalComponent>
  );
}
