import React from "react";
import {
  Card,
  Dialog,
  DialogContent,
  DialogTitle,
  Button,
  Box,
  Stack,
  Avatar,
} from "@mui/material";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import DeleteIcon from "@mui/icons-material/Delete";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import { styleSheet } from "../../../assets/styles/style";
import Slide from "@mui/material/Slide";
import StatusBadge from "../../shared/statudBadge";
import { useSelector } from "react-redux";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
const rows = [
  {
    id: 1,
    orderNo: "#123",
    trackingNo: "#565",
    status: "Completed",
    lastName: "Roy",
    firstName: "Jason",
    phone: "91069281",
    remarks: "Good",
  },
  {
    id: 2,
    lastName: "Roy",
    firstName: "Jason",
    orderNo: "#123",
    trackingNo: "#565",
    status: "Pending",
    phone: "91069281",
    remarks: "Bad",
  },
  {
    id: 3,
    orderNo: "#123",
    trackingNo: "#565",
    status: "Completed",
    lastName: "Roy",
    firstName: "Jason",
    phone: "91069281",
    remarks: "Good",
  },
  {
    id: 4,
    lastName: "Roy",
    firstName: "Jason",
    orderNo: "#123",
    trackingNo: "#565",
    status: "Pending",
    phone: "91069281",
    remarks: "Bad",
  },
  {
    id: 3,
    orderNo: "#123",
    trackingNo: "#565",
    status: "Completed",
    lastName: "Roy",
    firstName: "Jason",
    phone: "91069281",
    remarks: "Good",
  },
];
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function EditNotesModal(props) {
  let { open, setOpen } = props;
  const [chipData, setChipData] = React.useState([
    { key: 0, label: "Angular" },
    { key: 1, label: "jQuery" },
    { key: 2, label: "Polymer" },
    { key: 3, label: "React" },
  ]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={"Edit Delivery Note"}
    >
      <TableContainer>
        <Table sx={{ minWidth: 650 }} aria-label="a dense table">
          <TableHead>
            <TableRow>
              <TableCell sx={styleSheet.tableHeading}>{"Order No"}</TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left">
                {"Tracking No"}
              </TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left">
                {"Status"}
              </TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left">
                {"Customer"}
              </TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left">
                {"Phone"}
              </TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left">
                {"Remarks"}
              </TableCell>
              <TableCell sx={styleSheet.tableHeading} align="left"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow
                key={row.name}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell component="th" scope="row">
                  {row.orderNo}
                </TableCell>
                <TableCell align="left">{row.trackingNo}</TableCell>
                <TableCell align="left">
                  {" "}
                  <Box>
                    <StatusBadge
                      title={row?.status}
                      maxWidth="88px"
                      borderColor="rgba(0, 186, 119, 0.2)"
                      color={
                        row?.status === "Completed" ? "#00BA77" : "#FF9A23"
                      }
                      bgColor={
                        row?.status === "Pending"
                          ? "rgba(255, 154, 35, 0.1)"
                          : "rgba(0, 186, 119, 0.1)"
                      }
                    />
                  </Box>
                </TableCell>
                <TableCell align="left">
                  {" "}
                  <Box>
                    <Stack
                      direction="row"
                      justifyContent="center"
                      alignItems="center"
                      spacing={1}
                    >
                      <Avatar
                        sx={{
                          width: 25,
                          height: 25,
                          fontSize: "13px",
                          color: "var(--primary-color)",
                          background: "rgba(86, 58, 213, 0.3)",
                        }}
                      >
                        {row.firstName?.slice(0, 1)}
                      </Avatar>
                      <Box>
                        {row.firstName || ""} {row.lastName || ""}
                      </Box>
                    </Stack>
                  </Box>
                </TableCell>
                <TableCell align="left">{row.phone}</TableCell>
                <TableCell align="left">
                  <StatusBadge
                    title={row.remarks}
                    maxWidth="65px"
                    color="#1E1E1E;"
                    bgColor="#EAEAEA"
                  />
                </TableCell>
                <TableCell>
                  <Button
                    sx={styleSheet.deleteProductButton}
                    variant="outlined"
                  >
                    <DeleteIcon />
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </ModalComponent>
  );
}
export default EditNotesModal;
