import { Box } from "@mui/material";
import { RefreshBtn } from "../../../utilities/helpers/Helpers";

function StatusBadge(props) {
  let {
    borderColor,
    color,
    bgColor,
    title,
    maxWidth,
    showBtn,
    btnLoading,
    btnOnClick,
  } = props;
  return (
    <Box
      sx={{
        border: `1px solid ${borderColor}`,
        color: color,
        background: bgColor,
        borderRadius: "4px",
        padding: "2px 7px",
        maxWidth: maxWidth ? maxWidth : "auto",
        textAlign: "center",
        position: "relative",
        pl: showBtn ? 3.5 : "7px",
      }}
    >
      {showBtn && (
        <RefreshBtn
          sx={{
            position: "absolute",
            left: -4,
            top: -7,
          }}
          color={"#0070E0"}
          loading={btnLoading}
          onClick={btnOnClick}
        />
      )}

      {title}
    </Box>
  );
}
export default StatusBadge;
