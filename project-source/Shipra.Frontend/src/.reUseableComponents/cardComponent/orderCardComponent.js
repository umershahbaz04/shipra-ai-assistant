import Visibility from '@mui/icons-material/Visibility';
import {
    Box,
    Button,
    Card,
    CardActions,
    CardContent,
    Tooltip
} from "@mui/material";
import CircularProgress from "@mui/material/CircularProgress";

export const OrderCardComponent = ({
  orderData,
  cardActionsChildren,
  onEyeClick,
  loading,
  status,
}) => {
  return (
    <Card
      sx={{
        backgroundColor: "white",
        borderRadius: "16px",
        padding: "2px",
        paddingTop: "10px !important",
        border: "1px solid rgba(0, 0, 0, 0.12)",
        boxShadow: "none",
        position: "relative",
        display: "flex",
        flexDirection: "column",
        justifyContent: "space-between",
      }}
    >
      {status && (
        <Box
          sx={{
            position: "absolute",
            top: 0,
            right: 0,
            backgroundColor: status.color || "gray",
            color: "white",
            padding: "4px 12px",
            borderRadius: "0 0 0 8px",
            fontSize: "0.75rem",
            fontWeight: "bold",
            minWidth: "70px",
            textAlign: "center",
          }}
        >
          {status.label}
        </Box>
      )}
      <Tooltip title={"Item Count"} placement="top">
        <Button
          sx={{
            padding: "2px !important",
            position: "absolute",
            top: 0,
            left: 0,
            color: "primary",
            "&:hover": { backgroundColor: "rgba(0, 0, 0, 0.1)" },
          }}
          onClick={onEyeClick}
          disabled={loading}
        >
          {loading ? <CircularProgress size={20} /> : <Visibility />}
        </Button>
      </Tooltip>
      <CardContent
        sx={{
          display: "flex",
          alignItems: "center",
          padding: "6px !important",
          paddingTop: "25px!important",
          justifyContent: "space-between",
          gap: "12px",
        }}
      >
        <Box
          sx={{ flex: 1, display: "flex", flexDirection: "column", gap: "4px" }}
        >
          {orderData}
        </Box>
      </CardContent>
      <CardActions sx={{ padding: "8px", justifyContent: "flex-end" }}>
        {cardActionsChildren}
      </CardActions>
    </Card>
  );
};
