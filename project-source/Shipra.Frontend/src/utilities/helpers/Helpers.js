import { Collapse, makeStyles } from "@material-ui/core";
import CheckCircle from "@mui/icons-material/CheckCircle";
import ContentCopyOutlined from "@mui/icons-material/ContentCopyOutlined";
import Delete from "@mui/icons-material/Delete";
import Edit from "@mui/icons-material/Edit";
import FileUpload from "@mui/icons-material/FileUpload";
import FilterAltOutlined from "@mui/icons-material/FilterAltOutlined";
import HelpOutline from "@mui/icons-material/HelpOutline";
import Map from "@mui/icons-material/Map";
import MoneyOff from "@mui/icons-material/MoneyOff";
import NotInterested from "@mui/icons-material/NotInterested";
import PictureAsPdf from "@mui/icons-material/PictureAsPdf";
import PriceCheck from "@mui/icons-material/PriceCheck";
import Refresh from "@mui/icons-material/Refresh";
import Undo from "@mui/icons-material/Undo";
import Sms from "@mui/icons-material/Sms";
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import AutorenewIcon from "@mui/icons-material/Autorenew";
import BorderColorIcon from "@mui/icons-material/BorderColor";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import FilterAltIcon from "@mui/icons-material/FilterAlt";
import FullscreenIcon from "@mui/icons-material/Fullscreen";
import FullscreenExitIcon from "@mui/icons-material/FullscreenExit";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import ReportGmailerrorredTwoToneIcon from "@mui/icons-material/ReportGmailerrorredTwoTone";
import ViewListIcon from "@mui/icons-material/ViewList";
import ViewModuleIcon from "@mui/icons-material/ViewModule";
import VisibilityIcon from "@mui/icons-material/Visibility";
import ModeEditIcon from "@mui/icons-material/ModeEdit";
import { LoadingButton } from "@mui/lab";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import { useJsApiLoader } from "@react-google-maps/api";
import PropTypes from "prop-types";
import LinearProgress from "@mui/material/LinearProgress";
import {
  Alert,
  Avatar,
  Backdrop,
  Box,
  Button,
  ButtonBase,
  Card,
  CardActionArea,
  CardActions,
  CardContent,
  CardMedia,
  Checkbox,
  CircularProgress,
  ClickAwayListener,
  Fade,
  FormControlLabel,
  Grid,
  Grow,
  IconButton,
  InputAdornment,
  InputLabel,
  ListItemIcon,
  Menu,
  MenuItem,
  MenuList,
  Paper,
  Popper,
  Skeleton,
  Step,
  StepLabel,
  Stepper,
  TableCell,
  TableRow,
  TextField,
  ToggleButton,
  ToggleButtonGroup,
  Tooltip,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { Stack } from "@mui/system";
import moment from "moment";
import {
  useCallback,
  useEffect,
  useLayoutEffect,
  useRef,
  useState,
} from "react";
import { Circles } from "react-loader-spinner";
import { useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import styled from "styled-components";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import CustomRHFPhoneInput from "../../.reUseableComponents/TextField/CustomRHFPhoneInput";
import TextFieldComponent from "../../.reUseableComponents/TextField/TextFieldComponent";
import {
  GetAccessTokenWithRefreshToken,
  GetAllCarrierTrackingStatusForSelection,
  GetAllClientTax,
  GetAllClientUserRole,
  GetAllCountry,
  GetAllRegionTimeZone,
  GetAllRegionbyCountryId,
  GetCityByRegionId,
  GetClientSubscription,
  GetGenericSetting,
  GetMapApiKey,
  GetNextEmployeeUserName,
  GetRegionTimeZone,
  GetWayBillsByOrderNos,
} from "../../api/AxiosInterceptors";
import avatarIcon from "../../assets/images/avatar.png";
import excelicon from "../../assets/images/excelicon.svg";
import { styleSheet } from "../../assets/styles/style";
import StatusBadge from "../../components/shared/statudBadge";
import {
  changeApiCallingFlag,
  changeApiFilterModel,
  changeBadgeData,
  changeClientSubscription,
  changeSuccessNFaildedData,
  updateBadgeData,
  setMapApiKey,
  clearMapApiKey,
} from "../../redux/changeFlags";
import UtilityClass from "../UtilityClass";
import {
  getThisKeyCookie,
  setThisKeyCookie,
  setGenericCheckboxCookies,
} from "../cookies";
import {
  EnumCarrierTrackingStatus,
  EnumCookieKeys,
  viewTypesEnum,
} from "../enum";
import { successNotification } from "../toast";
import Colors, { LinkColor } from "./Colors";
import { handleLogout } from "../../components/sideNavBar";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import ImageIcon from "@mui/icons-material/Image";
const useStyles = makeStyles((theme) => ({
  customButton: {
    backgroundColor: "blue", // Change this to your desired background color
    "&:hover": {
      backgroundColor: "red", // Change this to the hover background color
    },
  },
  customTooltip: {
    fontSize: "10px", // Adjust the font size as needed
  },
}));

const StyledLink = styled.a`
  color: ${Colors.linkColor};
  text-decoration: underline;
`;
// purple
export const purple = "var(--primary-color)";
// green
export const green = "#00BA77";
// yellow
export const yellow = "#FF9A23";
export const red = "#dc3545";
export const lightRed = "#dc3545";
export const lightgrey = "#ef9a9a";
// pink
export const pink = "#D53AB3";
export const selectedRow = "rgba(190, 33, 47, 0.2)";
export const greyBorder = "1px solid rgba(224, 224, 224, 1)";

export const pagePadding = "10px";
export const linkColor = "#0070E0 !important";
export const grey = "#f5f5f5";
export const selectedTabBg = "rgba(86, 58, 213, 0.08)";

export const navbarHeight = 64;
export const dataGridProFooterHeight = 52;
export const pageMainBoxPadding = 20;
export const GOOGLE_MAPS_LIBRARIES = ["places"];

export const useCustomJsApiLoader = () => {
  const mapApiKey = useGetMapApiKeyReducer();
  const apiKey = mapApiKey || process.env.REACT_APP_GOOGLE_API_KEY || "";

  const isAlreadyLoaded =
    typeof window !== "undefined" &&
    Boolean(window.google && window.google.maps);

  const loaderResult = useJsApiLoader({
    id: "script-loader",
    googleMapsApiKey: apiKey,
    libraries: GOOGLE_MAPS_LIBRARIES,
  });

  if (isAlreadyLoaded) {
    return { isLoaded: true, loadError: null };
  }

  return loaderResult;
};

// countries
//#endregion
// DataGridHeaderBox
export const DataGridHeaderBox = ({ title }) => {
  return (
    <Box sx={{ fontWeight: "600", textTransform: "capitalize" }}>{title}</Box>
  );
};
// DataGridRenderGreyBox
export const DataGridRenderGreyBox = ({ title, copyBtn }) => {
  return (
    <Box sx={{ color: "grey" }} display={"flex"} alignItems={"center"}>
      {title}{" "}
      {copyBtn && <ClipboardIcon sx={{ padding: "5px 7px" }} text={title} />}
    </Box>
  );
};

// CrossIconButton
export function CrossIconButton({ onClick, color, sx, fontSize = "medium" }) {
  return (
    <IconButton onClick={onClick} sx={{ ...sx }}>
      <CloseIcon sx={{ color: color }} fontSize={fontSize} />
    </IconButton>
  );
}
// ClearFilterButton
export function ClearFilterButton({ onClick }) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Button
      sx={{
        ...styleSheet.filterIcon,
        minWidth: "100px",
        marginLeft: "5px",
      }}
      color="inherit"
      variant="outlined"
      onClick={onClick}
    >
      {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
    </Button>
  );
}
// FilterButton
export function FilterButton({ onClick }) {
  // const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Button
      sx={{
        ...styleSheet.filterIcon,
        minWidth: "100px",
        marginLeft: "5px",
      }}
      variant="contained"
      onClick={onClick}
    >
      {/* {LanguageReducer?.languageType?.FILTER_TEXT} */}
      {"Filter"}
    </Button>
  );
}
export function BackdropCustom({ open }) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Backdrop
      sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
      open={open}
    >
      <CircularProgress color="inherit" />
    </Backdrop>
  );
}
export function ActionButton({ onClick }) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Button
      sx={{ ...styleSheet.filterIcon, minWidth: "90px" }}
      variant="contained"
      disableElevation
      onClick={onClick}
      endIcon={<KeyboardArrowDownIcon />}
    >
      {LanguageReducer?.languageType?.ACTION}
    </Button>
  );
}
export function ActionButtonCustom(props) {
  const {
    onClick = () => {},
    label,
    width,
    marginBottom,
    height = "29px",
    loading = false,
    background = Colors.primary,
    p,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <LoadingButton
        loading={loading}
        sx={{
          ...(props?.isDeleted
            ? styleSheet.filterIconDeleteColord
            : styleSheet.filterIconColord),
          minWidth: width || "90px",
          height: height,
          background: background,
          marginBottom: marginBottom,
          p: p,
        }}
        disabled={loading}
        color="inherit"
        variant="outlined"
        id="demo-customized-button"
        aria-haspopup="true"
        disableElevation
        {...props}
      >
        <div
          style={{
            visibility: loading ? "hidden" : "visible",
            height: "100%",
            width: "100%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          {!label ? LanguageReducer?.languageType?.ACTION : label}
        </div>
      </LoadingButton>
    </>
  );
}

export function CopyButton({
  onClick,
  label = "Copy",
  width = "90px",
  height = "29px",
  marginBottom,
  padding = "14px",
}) {
  const [copied, setCopied] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleClick = async (e) => {
    if (loading) return;

    setLoading(true);
    const result = await onClick(e);
    setLoading(false);

    if (result) {
      setCopied(true);
      setTimeout(() => setCopied(false), 5000);
    }
  };

  return (
    <LoadingButton
      loading={loading}
      onClick={handleClick}
      sx={{
        width,
        minWidth: width,
        height,
        background: copied
          ? "green"
          : "var(--primary-color)  !important",
        marginBottom,
        fontSize: "10px",
        color: "#fff",
        textTransform: "capitalize",
        padding,
        "&:hover": {
          background: copied ? "green !important" : undefined,
        },
      }}
      color="inherit"
      variant="contained"
      aria-haspopup="true"
    >
      <div
        style={{
          visibility: loading ? "hidden" : "visible",
          height: "100%",
          width: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          gap: 4,
        }}
      >
        {!copied && <ContentCopyIcon fontSize="small" />}
        <span style={{ whiteSpace: "nowrap" }}>
          {copied ? "Copied" : label}
        </span>
      </div>
    </LoadingButton>
  );
}

export function ActionButtonEdit(props) {
  const classes = useStyles();
  const { onClick = () => {}, label, width, loading = false } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <IconButton
        style={{
          borderRadius: "5px",
          width: "25px",
          background: "#1976d2",
          padding: "2px",
        }}
        color="error"
        aria-label="edit"
        size="small"
        onClick={onClick}
      >
        {loading ? (
          <CircularProgress size={20} />
        ) : (
          <Edit fontSize="small" sx={{ color: "#fff" }} />
        )}
      </IconButton>{" "}
    </>
  );
}
export function ActionButtonRefresh(props) {
  const classes = useStyles();
  const { onClick = () => {}, label, width, loading = false } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <IconButton
        style={{
          borderRadius: "5px",
          width: "25px",
          background: "#1976d2",
          padding: "2px",
        }}
        color="error"
        aria-label="edit"
        size="small"
        onClick={onClick}
      >
        {loading ? (
          <CircularProgress size={20} />
        ) : (
          <Refresh fontSize="small" sx={{ color: "#fff" }} />
        )}
      </IconButton>{" "}
    </>
  );
}
export function ActionButtonDelete(props) {
  const classes = useStyles();
  const { onClick = () => {}, label, width, loading = false } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <IconButton
        style={{
          borderRadius: "5px",
          width: "25px",
          background: "#d32f2f",
          padding: "2px",
        }}
        color="error"
        aria-label="delete"
        size="small"
        onClick={onClick}
      >
        <Delete fontSize="small" sx={{ color: "#fff" }} />
      </IconButton>{" "}
    </>
  );
}
export const StyledTooltip = ({ title = "show", children, empty, sx }) => {
  return (
    <Tooltip title={title}>
      {!empty && !children ? (
        <IconButton sx={{ ...sx }}>
          <HelpOutline sx={{ ...styleSheet.otherIconForTableRow }} />
        </IconButton>
      ) : (
        children
      )}
    </Tooltip>
  );
};

export function DisableButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.danger }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Disabled">
        <NotInterested sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}

// export function ActionButtonEdit(props) {
//   const { onClick = () => {}, label, width } = props;
//   const LanguageReducer = useSelector((state) => state.LanguageReducer);
//   return (
//     <IconButton aria-label="edit" size="small" onClick={onClick}>
//       <Edit fontSize="small" sx={{ color: "info" }} />
//     </IconButton>
//   );
// }
export function ActionFilterButton({ onClick }) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Button
      sx={{ ...styleSheet.filterIconColord, minWidth: "90px" }}
      color="inherit"
      onClick={onClick}
      variant="outlined"
      startIcon={<FilterAltOutlined fontSize="small" />}
    >
      {LanguageReducer?.languageType?.FILTER}
    </Button>
  );
}
export function ActionButtonWithPoper(props) {
  const {
    id,
    placeholder = "Action",
    placement = "bottom",
    minWidth = "90px",
    options = [],
    onClick = () => {},
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const anchorRef = useRef(null);
  const [selectedIndex, setSelectedIndex] = useState("");
  const [open, setOpen] = useState(false);

  const handleClose = (event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target)) {
      return;
    }
    setOpen(false);
  };
  const handleToggle = () => {
    setOpen((prevOpen) => !prevOpen);
  };

  return (
    <>
      <Button
        sx={{ ...styleSheet.filterIconColord, minWidth: minWidth }}
        ref={anchorRef}
        size="small"
        aria-controls={open ? id : undefined}
        aria-describedby={open ? id : undefined}
        aria-expanded={open ? "true" : undefined}
        aria-label="select merge strategy"
        aria-haspopup="menu"
        onClick={handleToggle}
        endIcon={<KeyboardArrowDownIcon />}
      >
        {placeholder}
      </Button>
      <Popper
        open={open}
        id={open ? id : undefined}
        anchorEl={anchorRef.current}
        transition
        disablePortal
        placement={placement}
        sx={{ zIndex: 10000 }}
        onClick={handleClose}
      >
        {({ TransitionProps }) => (
          <Fade {...TransitionProps} timeout={350}>
            <Grow {...TransitionProps}>
              <Paper>
                <ClickAwayListener onClickAway={handleClose}>
                  <MenuList
                    sx={{
                      minWidth: minWidth
                        ? `${Number(minWidth.split("px")[0]) + 40}px !important`
                        : "162px !important",
                    }}
                    id={id}
                    autoFocusItem
                  >
                    {options.map((option, index) => (
                      <MenuItem
                        key={index}
                        selected={index === selectedIndex}
                        onClick={(event) => onClick(event, index)}
                        value={option.value ? option.value : option}
                      >
                        {option.icon ? (
                          <ListItemIcon>{option.icon}</ListItemIcon>
                        ) : null}
                        {option.title ? option.title : option}
                      </MenuItem>
                    ))}
                  </MenuList>
                </ClickAwayListener>
              </Paper>
            </Grow>
          </Fade>
        )}
      </Popper>
    </>
  );
}
// amountFormat
export const amountFormat = (amount) => {
  if (amount) {
    amount = eval(amount);
    return amount?.toFixed(2).replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
  } else {
    return "0.00";
  }
};

export const EyeIconButton = ({ onClick }) => {
  return (
    <IconButton onClick={onClick}>
      <VisibilityIcon fontSize="small" />
    </IconButton>
  );
};
export const EyeIconLoadingButton = ({ onClick, loading = false }) => {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.purple }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="View">
        <VisibilityIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
};
export const ShowErrorIconLoadingButton = ({ onClick, loading = false }) => {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.danger }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip
        title="
      Show Errors"
      >
        <ReportGmailerrorredTwoToneIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
};
const VisuallyHiddenInput = styled("input")({
  clip: "rect(0 0 0 0)",
  clipPath: "inset(50%)",
  height: 1,
  overflow: "hidden",
  position: "absolute",
  bottom: 0,
  left: 0,
  whiteSpace: "nowrap",
  width: 1,
});

export const UploadButton = ({
  onChange,
  label = "Upload Image",
  multiple,
  background,
  accept,
  ...buttonProps
}) => {
  return (
    <Button
      sx={{
        textTransform: "capitalize !important",
        height: "29px",
        fontSize: "12px",
        backgroundColor: background || "#9e9e9e",
      }}
      component="label"
      role={undefined}
      variant="contained"
      tabIndex={-1}
      startIcon={<FileUploadIcon />}
      {...buttonProps}
    >
      {label}
      <VisuallyHiddenInput
        type="file"
        onChange={onChange}
        multiple={multiple}
        accept={accept}
      />
    </Button>
  );
};
export const FileUploadIconButton = ({ onClick, loading = false }) => {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.uploadFile }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Upload file">
        <FileUpload sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
};
//CodeBox
export const CodeBox = ({
  title,
  onClick,
  bold,
  eyeBtn = false,
  copyBtn = false,
  hasColor = true,
  color = LinkColor,
  fs = 12,
}) => {
  if (title) {
    return (
      <Box className={"flex_between"}>
        <ButtonBase
          disableRipple
          onClick={onClick && onClick}
          sx={{
            color: hasColor && color,
            fontWeight: bold || 500,
            fontSize: fs,
            cursor: onClick ? "pointer" : "initial",
            textDecoration:
              title === "loading..." ? "none" : onClick && "underline",
          }}
        >
          {title}
        </ButtonBase>
        {eyeBtn && <EyeIconButton onClick={onClick} />}
        {copyBtn && <ClipboardIcon sx={{ padding: "5px 10px" }} text={title} />}
      </Box>
    );
  }
  return null;
};
export const CodeBoxWithValue = ({
  title,
  value,
  onClick,
  bold,
  eyeBtn = false,
  copyBtn = false,
  hasColor = true,
  color = LinkColor,
}) => {
  if (title) {
    return (
      <Box className={"flex_between"}>
        <ButtonBase
          disableRipple
          onClick={onClick && onClick}
          sx={{
            color: hasColor && color,
            fontWeight: bold || 500,
            fontSize: 12,
            cursor: onClick ? "pointer" : "initial",
            textDecoration: onClick && "underline",
            // ":hover": {
            // },
          }}
        >
          {title}
        </ButtonBase>
        {eyeBtn && <EyeIconButton onClick={onClick} />}
        {copyBtn && <ClipboardIcon sx={{ padding: "5px 10px" }} text={value} />}
      </Box>
    );
  }
};
export const DescriptionBoxWithChild = ({
  onClick = () => {},
  children,
  isEditIcon,
}) => {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  // handleClick
  const handleClick = (event) => {
    onClick();
    setAnchorEl(event.currentTarget);
  };
  // handleClose
  const handleClose = () => {
    setAnchorEl(null);
  };

  if (children) {
    return (
      <>
        {isEditIcon ? (
          <IconButton onClick={handleClick}>
            <ModeEditIcon color="primary" fontSize="small" />
          </IconButton>
        ) : (
          <IconButton onClick={handleClick}>
            {open ? (
              <ExpandLess sx={{ fontSize: 14 }} />
            ) : (
              <ExpandMore sx={{ fontSize: 14 }} />
            )}
          </IconButton>
        )}
        <Menu
          id="long-menu"
          MenuListProps={{
            "aria-labelledby": "long-button",
          }}
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
          transformOrigin={{
            vertical: "top",
            horizontal: "left",
          }}
          anchorEl={anchorEl}
          open={open}
          onClose={handleClose}
          PaperProps={{
            sx: {
              px: 1.25,
              // width: 200,
              // maxHeight: 500,
            },
          }}
        >
          <Box>{children}</Box>
        </Menu>
      </>
    );
  }
};
export const DescriptionBox = ({ description, title = "Description" }) => {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  // handleClick
  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };
  // handleClose
  const handleClose = () => {
    setAnchorEl(null);
  };

  if (description) {
    return (
      <>
        <IconButton onClick={handleClick}>
          {open ? (
            <ExpandLess sx={{ fontSize: 14 }} />
          ) : (
            <ExpandMore sx={{ fontSize: 14 }} />
          )}
        </IconButton>
        <Menu
          id="long-menu"
          MenuListProps={{
            "aria-labelledby": "long-button",
          }}
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
          transformOrigin={{
            vertical: "top",
            horizontal: "left",
          }}
          anchorEl={anchorEl}
          open={open}
          onClose={handleClose}
          PaperProps={{
            sx: {
              px: 1.25,
              width: 200,
              maxHeight: 500,
            },
          }}
        >
          <Box>
            <Typography
              variant="h6"
              fontWeight={600}
              sx={{ textTransform: "capitalize" }}
            >
              {title}
            </Typography>
            <Typography
              variant="h6"
              sx={{
                overflowWrap: "break-word",
              }}
            >
              {description}
            </Typography>
          </Box>
        </Menu>
      </>
    );
  }
};
export const DescriptionBoxForUpdateStatus = ({
  description,
  title = "Description",
}) => {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  // handleClick
  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };
  // handleClose
  const handleClose = () => {
    setAnchorEl(null);
  };

  if (description) {
    return (
      <>
        <IconButton onClick={handleClick}>
          {open ? (
            <ExpandLess sx={{ fontSize: 14 }} />
          ) : (
            <ExpandMore sx={{ fontSize: 14 }} />
          )}
        </IconButton>
        <Menu
          id="long-menu"
          MenuListProps={{
            "aria-labelledby": "long-button",
          }}
          anchorOrigin={{
            vertical: "bottom",
            horizontal: "left",
          }}
          transformOrigin={{
            vertical: "top",
            horizontal: "left",
          }}
          anchorEl={anchorEl}
          open={open}
          onClose={handleClose}
          PaperProps={{
            sx: {
              px: 1.25,
              width: 200,
              maxHeight: 500,
            },
          }}
        >
          <Box>
            <Typography
              variant="h6"
              fontWeight={600}
              sx={{ textTransform: "capitalize" }}
            >
              {title}
            </Typography>
            <Typography
              variant="h6"
              sx={{
                overflowWrap: "break-word",
              }}
            >
              {description}
            </Typography>
          </Box>
        </Menu>
      </>
    );
  }
};

//CodeBox
export const AnchorBox = ({ href }) => {
  if (href) {
    return (
      <a href={href} target="_blank">
        {href}
      </a>
    );
  }
};
// MailtoBox
export const MailtoBox = ({ email, classes }) => {
  if (email) {
    return <StyledLink href={`mailto:${email}`}>{email}</StyledLink>;
  }
};
// DialerBox
export const DialerBox = ({ phone }) => {
  if (phone) {
    return <StyledLink href={`tel:${phone}`}>{phone}</StyledLink>;
  }
};
// handleCopyToClipBoard
export const handleCopyToClipBoard = (textToCopy) => {
  UtilityClass.copyToClipboard(textToCopy);
};

// ClipboardIcon
export const ClipboardIcon = ({ text, size = 14, ...others }) => {
  const classes = useStyles();
  return (
    <Tooltip
      classes={{ tooltip: classes.customTooltip }}
      title="Copy to Clipboard"
    >
      <IconButton
        onClick={(e) => {
          e.stopPropagation();
          handleCopyToClipBoard(text);
        }}
        {...others}
      >
        <ContentCopyOutlined sx={{ fontSize: size }} />
      </IconButton>
    </Tooltip>
  );
};
export const CustomClipboardIcon = ({
  text,
  size = 14,
  onClick = () => {},
  ...others
}) => {
  const classes = useStyles();
  return (
    <Tooltip
      classes={{ tooltip: classes.customTooltip }}
      title="Copy to Clipboard"
    >
      <IconButton onClick={onClick} {...others}>
        <ContentCopyOutlined sx={{ fontSize: size }} />
      </IconButton>
    </Tooltip>
  );
};

export const StatusBadgeCustom = ({ title, isSuccess = true }) => {
  return (
    <StatusBadge
      title={title}
      color={isSuccess == false ? "#fff;" : "#fff;"}
      bgColor={isSuccess === false ? "#dc3545;" : "#28a745;"}
    />
  );
};
// centerColumn
export const centerColumn = {
  headerAlign: "center",
  align: "center",
};
// rightColumn
export const rightColumn = {
  headerAlign: "right",
  align: "right",
};

export const placeholders = Object.freeze({
  name: "e.g. John Doe",
  company_name: "e.g. Amazon",
  email: "e.g. abc@shipra.com",
  apartment: "e.g. The Palm, Dubai",
  password: "●●●●●●●●●",
  store_name: "e.g. ABC",
  license_num: "e.g. 123",
  zip_code: "e.g. 00000",
  url: "e.g. www.shipra.io",
  searchMap: "e.g. villa 009",
  longitude: "e.g. 25.0032° N",
  latitude: "e.g. 55.0204° E",
  weight: "e.g. 0.8",
  price: "e.g. 12345",
  sku: "e.g. ABC-12345-S-BL",
  category_name: "e.g. watch",
  quantity: "e.g. 10",
  station_name: "e.g. Palm Deira",
  order_no: "e.g. 10001",
  loading: "Loading...",
  refNo: "SH12321",
  clientPublicKey: "Public Key",
});

// export const CustomColorLabelledOutline = withStyles({
//   root: {
//     "& $notchedOutline": {
//       borderColor: purple,
//       borderWidth: "2px",
//     },
//     "& $inputLabel": {
//       color: purple,
//     },
//   },
//   notchedOutline: {},
//   inputLabel: {},
//   content: {},
// })(LabelledOutline);

export const CustomColorLabelledOutline = ({
  label,
  discription,
  height,
  isCollapse = false,
  defaultCollapseState = true,
  children,
  onClick = () => {},
}) => {
  const [transitionOpen, setTransitionOpen] = useState(defaultCollapseState);
  return (
    <Box
      component={"fieldset"}
      border={`2px solid ${purple}`}
      borderRadius={"8px"}
      height={height || "inherit"}
      marginTop={1}
      marginLeft={0}
      marginRight={0}
      width={"100%"}
    >
      {!isCollapse ? (
        <>
          <legend
            style={{
              color: purple,
              fontSize: 16,
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontWeight: 600,
            }}
          >
            {label}
          </legend>
          {children}
        </>
      ) : (
        <>
          <Box className="flex_between">
            <Box sx={{ display: "flex", gap: 0.5, alignItems: "center" }}>
              <legend
                style={{
                  color: purple,
                  fontSize: 16,
                  fontFamily:
                    "'Lato Regular', 'Inter Regular', 'Arial' !important",
                  fontWeight: 600,
                }}
              >
                {label}
              </legend>
              {discription && <p>{discription}</p>}
            </Box>

            <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
              <IconButton
                onClick={() => {
                  onClick();
                  setTransitionOpen((prev) => !prev);
                }}
              >
                {transitionOpen ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            </Box>
          </Box>
          <Collapse in={transitionOpen}>{children}</Collapse>
        </>
      )}
    </Box>
  );
};

// PaymentAmountTextField
export const PaymentAmountTextField = ({
  value,
  onChange,
  width = 200,
  required = false,
  disabled,
  id,
}) => {
  return (
    <Box>
      <TextField
        id={id}
        size="small"
        type="number"
        fullWidth
        sx={{
          "& input": {
            textAlign: "right",
          },
          paddingLeft: "0px",
          width: width,
          border: "none",
          background: "transparent",
          "& .MuiInputBase-root": {
            height: 30,
            pl: 0.5,
          },
        }}
        required={required}
        onChange={onChange}
        disabled={disabled}
        value={value}
        InputProps={{
          startAdornment: (
            <InputAdornment
              position="start"
              sx={{ padding: "0px", fontSize: 12 }}
            >
              {" "}
              AED
            </InputAdornment>
          ),
          style: {
            fontSize: "15px",
            fontWeight: 600,
          },
          step: "any",
        }}
      />
    </Box>
  );
};
export const PaymentAmountBox = ({
  title,
  value,
  onChange,
  required = false,
  disabled,
}) => {
  return (
    <>
      <Box className={"flex_between"}>
        <Typography variant="h5" fontWeight={500}>
          {title}
        </Typography>
        <PaymentAmountTextField
          required
          value={value}
          onChange={onChange}
          disabled={disabled}
        />
      </Box>
    </>
  );
};
// PaymentTaxBox
export const PaymentTaxBox = ({
  title,
  value,
  onChange,
  subtotal,
  disabled,
  getTaxValue,
}) => {
  const taxValue = ((subtotal / 100) * value).toFixed(2);
  useEffect(() => {
    getTaxValue(taxValue);
  }, [taxValue]);

  return (
    <>
      <Box className={"flex_between"}>
        <Box display={"flex"} gap={0.5} alignItems={"baseline"}>
          <Typography variant="h5" fontWeight={500}>
            {title}
          </Typography>
          <Typography variant="h6">
            {/* <TextField
              size="small"
              type="number"
              fullWidth
              disabled={disabled}
              sx={{
                "& input": {
                  textAlign: "center",
                  padding: "0px !important",
                  fontSize: "12px",
                  fontWeight: 600,
                },
                padding: "0px",
                width: "30px",
                border: "none",
                background: "transparent",
                height: "20px",
              }}
              onChange={onChange}
              value={value}
              InputProps={{
                inputProps: { min: 0, max: 100 },
                step: "any",
              }}
            /> */}
            {value}%
          </Typography>
        </Box>
        <Box className={"flex_center"} gap={"2px"}>
          <Typography variant="h5">
            AED {isNaN(taxValue) ? 0.0 : taxValue}
          </Typography>
        </Box>
      </Box>
    </>
  );
};

export const PaymentTaxAlert = ({ onClick }) => {
  const navigate = useNavigate();

  return (
    <>
      <Box>
        <Alert
          variant="outlined"
          severity="error"
          sx={{
            py: 0,
            color: "#d32f2f",
          }}
          action={
            <ButtonComponent
              onClick={() => {
                onClick();
                navigate("/tax");
              }}
              variant="outlined"
              title={"Add Tax"}
            />
          }
        >
          {"Please Add Tax from Settings"}
        </Alert>
      </Box>
    </>
  );
};
export const PaymentTotalBox = ({ value }) => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <Box className={"flex_between"}>
        <Typography variant="h4" fontWeight={700}>
          {LanguageReducer?.languageType?.ORDERS_TOTAL}
        </Typography>
        <Typography variant="h4" fontWeight={700}>
          AED {value}
        </Typography>
      </Box>
    </>
  );
};
// PaymentMethodBox
export const PaymentMethodCheckbox = ({
  checked,
  onChange,
  label,
  disabled,
}) => {
  return (
    <>
      <FormControlLabel
        disabled={disabled}
        sx={{ margin: "0px" }}
        control={
          <Checkbox
            sx={{
              
            }}
            checked={checked}
            onChange={onChange}
            edge="start"
            disableRipple
            defaultChecked
          />
        }
        label={label}
      />
    </>
  );
};
export const PaymentMethodBox = ({ options }) => {
  return (
    <>
      <Box textAlign={"center"}>
        <Typography variant="h5" fontWeight={700}>
          Payment Method
        </Typography>
      </Box>

      {options}
    </>
  );
};

// GridContainer
export const GridContainer = ({
  xs = 12,
  sm,
  md,
  lg,
  xl,
  spacing = 1,
  children,
  ...others
}) => {
  return (
    <Grid
      item
      xs={xs}
      sm={sm}
      md={md}
      lg={lg}
      xl={xl}
      container
      spacing={spacing}
      {...others}
    >
      {children}
    </Grid>
  );
};
// GridItem
export const GridItem = ({ xs = 6, sm, md, lg, xl, children, ...others }) => {
  return (
    <Grid item xs={xs} sm={sm} md={md} lg={lg} xl={xl} {...others}>
      {children}
    </Grid>
  );
};
export const GridItemResponsive = ({
  xs = 12,
  sm,
  md = 6,
  lg,
  xl,
  children,
  ...others
}) => {
  return (
    <Grid item xs={xs} sm={sm} md={md} lg={lg} xl={xl} {...others}>
      {children}
    </Grid>
  );
};

// useMenu
export const useMenu = () => {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  const handleOpen = (event) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };
  return { anchorEl, open, handleOpen, handleClose };
};
export const useMenuForLoop = () => {
  const [openElem, setOpenElem] = useState(null);
  const [anchorEl, setAnchorEl] = useState(null);
  const handleOpen = (event, elem) => {
    setAnchorEl(event.currentTarget);
    setOpenElem(elem);
  };
  const handleClose = () => {
    setAnchorEl(null);
    setOpenElem(null);
  };
  return { anchorEl, openElem, handleOpen, handleClose };
};

export const MenuComponent = (props) => {
  const { anchorEl, open, onClose, children, sx } = props;
  return (
    <Menu
      id="fade-menu"
      MenuListProps={{
        "aria-labelledby": "fade-button",
      }}
      anchorEl={anchorEl}
      open={open}
      onClose={onClose}
      TransitionComponent={Fade}
      disableElevation
      anchorOrigin={{
        vertical: "bottom",
        horizontal: "center",
      }}
      sx={sx}
      transformOrigin={{
        vertical: "top",
        horizontal: "center",
      }}
    >
      {children}
    </Menu>
  );
};

const buttonSX = {
  minWidth: 0,
  p: 0.5,
  height: "28px",
};
const buttonIconSX = {
  fontSize: "20px",
};
// ViewButton

export function ActionStack({ children }) {
  return (
    <Stack direction={"row"} spacing={0.5}>
      {children}
    </Stack>
  );
}
export function ViewButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.view }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="View">
        <VisibilityIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function ImageButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.image }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Image">
        <ImageIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function PDFButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.pdf }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Pdf">
        <PictureAsPdf sx={buttonIconSX} width={21} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function UnPaidButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.danger }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Un Paid">
        <MoneyOff sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function PaidButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.succes }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Paid">
        <PriceCheck sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function EditButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: "primary" }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Edit">
        <EditIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function EditIconButton({ onClick, loading = false, ...others }) {
  return (
    <IconButton onClick={onClick} {...others}>
      <StyledTooltip title="Edit">
        <EditIcon sx={buttonIconSX} />
      </StyledTooltip>
    </IconButton>
  );
}

// DeleteButton
export function DeleteButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.danger }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Delete">
        <DeleteIcon sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function MapButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.linkColor }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Update Coordinates">
        <Map sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function SmsButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.success }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Send SMS">
        <Sms sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
export function ValidateButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      ssx={{ ...buttonSX, background: Colors.succes }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Validate">
        <CheckCircle sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}
// DeleteIconButton
export function DeleteIconButton({ onClick, loading = false, ...others }) {
  return (
    <IconButton onClick={onClick} {...others}>
      <StyledTooltip title="Delete">
        <DeleteIcon sx={buttonIconSX} color="error" />
      </StyledTooltip>
    </IconButton>
  );
}
export function EnableButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.succes }}
      onClick={onClick}
      variant="contained"
    >
      <StyledTooltip title="Undo">
        <Undo sx={buttonIconSX} />
      </StyledTooltip>
    </LoadingButton>
  );
}

export const useIsUserProfileSideBarShow = () => {
  const isUserProfileSideBarShow = useSelector(
    (state) => state.UserProfileSideBarReducer.isUserProfileSideBarShow,
  );
  return isUserProfileSideBarShow;
};
export const useIsUserProfileChange = () => {
  const isUserProfileChange = useSelector(
    (state) => state.UserProfileImgChangeReducer.isUserProfileImgChange,
  );
  return isUserProfileChange;
};
// export const useIsNumericKeyPressInputChange = () => {
//   const isNumericKeyPressInputChange = useSelector(
//     (state) => state.changeFlagsReducer.isNumericKeyInputPress
//   );
//   return isNumericKeyPressInputChange;
// };

// export const handleDispatchNumericKeyPressInputFlag = (dispatch, value) => {
//   dispatch(changeNumericKeyInputPressFlag(!value));
// };

// export const useNumericKeyPressInputDependencyEffect = (dependencies) => {
//   const isNumericKeyPressInputChange = useIsNumericKeyPressInputChange();
//   const dispatch = useDispatch();
//   useEffect(() => {
//     handleDispatchNumericKeyPressInputFlag(
//       dispatch,
//       isNumericKeyPressInputChange
//     );
//   }, [...dependencies]);
// };
export const useLanguageReducer = () => {
  const LanguageReducer = useSelector(
    (state) => state.LanguageReducer.languageType,
  );
  return LanguageReducer;
};
export const useIsApiCallingFlagReducer = () => {
  const data = useSelector((state) => state.changeFlagsReducer.isApiCalling);
  return data;
};

export const handleDispatIsApiCallingFlag = (dispatch, value = {}) => {
  dispatch(changeApiCallingFlag(value));
};

export const useSuccessNFaildedDataReducer = () => {
  const data = useSelector(
    (state) => state.changeFlagsReducer.successNfailedData,
  );
  return data;
};
export const handleDispatSuccessNFaildedData = (dispatch, value = {}) => {
  dispatch(changeSuccessNFaildedData(value));
};

export const useFilterModelReducer = () => {
  const data = useSelector((state) => state.changeFlagsReducer.filterModel);
  return data;
};

export const handleDispatIsApiFilterModelFlag = (dispatch, value = {}) => {
  dispatch(changeApiFilterModel(value));
};

export const useClientSubscriptionReducer = () => {
  const data = useSelector(
    (state) => state.changeFlagsReducer.clientSubscription,
  );
  return data;
};
export const handleDispatchClientSubscription = (dispatch, value) => {
  dispatch(changeClientSubscription(value));
};

export const useGetBadgeDataReducer = () => {
  const badgeData = useSelector((state) => state.changeFlagsReducer.badgeData);
  return badgeData;
};

export const handleDispathShowBadge = (dispatch, value = {}) => {
  dispatch(changeBadgeData(value));
};

export const handleDispathUpdateBadge = (dispatch, value = {}) => {
  dispatch(updateBadgeData(value));
};

export const useUserRoleReducer = () => {
  const userRoleId = useSelector((state) => state.UserRoleReducer.userRoleId);
  return userRoleId;
};

export const useForm = (setSubmitData) => {
  const [errors, setErrors] = useState([]);
  // handleInvalid
  const handleInvalid = (e) => {
    setErrors([...errors, e.target.name]);
  };
  // handleFilterError
  const handleFilterError = (e, _name) => {
    const name = e ? e.target.name : _name;
    let newerrors = errors.filter((er) => er !== name);
    setErrors(newerrors);
  };
  const handleChange = (e) => {
    handleFilterError(e);
    setSubmitData((prev) => ({
      ...prev,
      [e.target.name]:
        e.target.value.length === 1
          ? e.target.value.trim().length === 1
            ? e.target.value
            : ""
          : e.target.value,
    }));
  };

  const handleChangeAutoComplete = (name, value) => {
    handleFilterError(null, name);
    setSubmitData((prev) => ({ ...prev, [name]: value }));
  };
  const handleChangePhone = (name, value) => {
    handleFilterError(null, name);
    setSubmitData((prev) => ({ ...prev, [name]: value }));
  };

  return {
    errors,
    handleInvalid,
    handleChange,
    handleChangeAutoComplete,
    handleChangePhone,
    setErrors,
  };
};
export const useFormForArray = (setSubmitData) => {
  const [errors, setErrors] = useState([]);
  // handleInvalid
  const handleInvalid = (e) => {
    setErrors([...errors, e.target.name]);
  };
  // handleFilterError
  const handleFilterError = (e, _name) => {
    const name = e ? e.target.name : _name;
    let newerrors = errors.filter((er) => er !== name);
    setErrors(newerrors);
  };
  const handleChange = (e) => {
    const { name, value } = e.target;
    const [selectedField, index] = name.split("_");
    handleFilterError(e);
    setSubmitData((prev) => {
      const _metaFields = [...prev];
      const _selectedObj = { ..._metaFields[index] };
      _selectedObj[selectedField] = value;
      _metaFields[index] = _selectedObj;
      return _metaFields;
    });
  };

  const handleChangeAutoComplete = (name, value) => {
    const [selectedField, index] = name.split("_");
    handleFilterError(null, name);
    setSubmitData((prev) => {
      const _metaFields = [...prev];
      const _selectedObj = { ..._metaFields[index] };
      _selectedObj[selectedField] = value;
      _metaFields[index] = _selectedObj;
      return _metaFields;
    });
  };
  const handleChangePhone = (name, value) => {
    handleFilterError(null, name);
    setSubmitData((prev) => ({ ...prev, [name]: value }));
  };

  return {
    errors,
    handleInvalid,
    handleChange,
    handleChangeAutoComplete,
    handleChangePhone,
    setErrors,
  };
};

export const PageMainBox = ({ children }) => {
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>{children}</div>
    </Box>
  );
};

export const UploadProfile = ({ file, onChange, size = 90, url }) => {
  return (
    <IconButton component="label" sx={{ position: "relative" }}>
      <Avatar
        sx={{
          width: size,
          height: size,
          border: file && "1px solid grey",
        }}
        src={file ? file && URL.createObjectURL(file) : url || avatarIcon}
      ></Avatar>
      <Box
        width={24}
        height={24}
        bgcolor={purple}
        borderRadius={"50%"}
        position={"absolute"}
        bottom={8}
        right={16}
        className={"flex_center"}
      >
        <BorderColorIcon
          sx={{
            width: 15,
            height: 15,
            color: "#fff",
          }}
        />
      </Box>
      <input onChange={onChange} hidden accept="image/*" type="file" />
    </IconButton>
  );
};

export const cookie_isUserProfileSideBarShow =
  getThisKeyCookie("isUserProfileSideBarShow") &&
  JSON.parse(getThisKeyCookie("isUserProfileSideBarShow"));

// fetchMethod
export const fetchMethod = async (
  method,
  setLoading = () => {},
  hasLoadingObj = true,
  loadingName = "loading",
) => {
  let response = {};
  hasLoadingObj
    ? setLoading((prev) => ({ ...prev, [loadingName]: true }))
    : setLoading(true);
  await method()
    .then((res) => {
      response = res.data;
    })
    .catch((err) => {
      response.isSuccess = false;
      console.log(err);
    })
    .finally(() => {
      hasLoadingObj
        ? setLoading((prev) => ({ ...prev, [loadingName]: false }))
        : setLoading(false);
    });
  return { response: response };
};

export const handleGetAllCountries = async () => {
  let data = [];
  const { response } = await fetchMethod(() => GetAllCountry({}));
  if (response) {
    data = response.result;
  }
  return data;
};
export const handleGetAllRegionTimeZone = async () => {
  let data = [];
  const { response } = await fetchMethod(() => GetAllRegionTimeZone({}));
  if (response) {
    data = response.result;
  }
  return data;
};

export const handleGetAllRegionsByCountryId = async (selectedCountry) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAllRegionbyCountryId(selectedCountry),
  );
  if (response) {
    data = response.result;
  }
  return data;
};
export const handleGetAllCitiesByRegionId = async (selectedRegion) => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetCityByRegionId(selectedRegion),
  );
  if (response) {
    data = response.result;
  }
  return data;
};

// useGetAllCountries
export const useGetAllCountries = () => {
  const [countries, setCountries] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedCountryIds, setSelectedCountryIds] = useState([]);
  // handleGetAllCountries
  const handleGetAllCountries = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllCountry({}));
    if (response) {
      setCountries(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllCountries();
  }, []);
  return {
    loading,
    countries,
    selectedCountryIds,
    setSelectedCountryIds,
    handleGetAllCountries,
  };
};
export const useGetAllRegionTimeZone = () => {
  const [regionTimeZone, setRegionTimeZone] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllRegionTimeZone
  const handleGetAllRegionTimeZone = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetAllRegionTimeZone({}));
    if (response) {
      setRegionTimeZone(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllRegionTimeZone();
  }, []);
  return { loading, regionTimeZone };
};
export const useGetAllClientUserRole = (isRoleAdded) => {
  const [clientUserRole, setClientUserRole] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllClientUserRole
  const handleGetAllClientUserRole = async () => {
    setLoading(true);
    const { response } = await fetchMethod(GetAllClientUserRole);
    if (response) {
      setClientUserRole(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllClientUserRole();
  }, [isRoleAdded]);
  return { loading, clientUserRole };
};
// useGetAllRegions
export const useGetAllRegionsByCountryId = (selectedCountry) => {
  const [regions, setRegions] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllRegions
  const handleGetAllRegions = async () => {
    if (selectedCountry) {
      setLoading(true);
      const { response } = await fetchMethod(() =>
        GetAllRegionbyCountryId(selectedCountry),
      );
      if (response) {
        setRegions(response.result);
      }
      setLoading(false);
    }
  };
  useEffect(() => {
    handleGetAllRegions();
  }, [selectedCountry]);
  return { loading, regions };
};
// useGetAllCities
export const useGetAllCitiesByRegionId = (selectedRegion) => {
  const [cities, setCities] = useState([]);
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleGetAllCities = async () => {
    if (selectedRegion) {
      setLoading(true);
      const { response } = await fetchMethod(() =>
        GetCityByRegionId(selectedRegion),
      );
      if (response) {
        setCities(response.result);
      }
      setLoading(false);
    }
  };
  useEffect(() => {
    handleGetAllCities();
  }, [selectedRegion]);
  return { loading, cities };
};

export const CicrlesLoading = ({ height = "90vh", text = "" }) => {
  return (
    <Box
      className={"flex_center"}
      height={height}
      width={"100%"}
      flexDirection={"column"}
    >
      <Circles type="Circles" width={168} height={168} color={purple} />
      {text}
    </Box>
  );
};

// useShowPassword;
export const useShowPassword = () => {
  const [showPassword, setShowPassword] = useState(false);

  const handleShowPassword = () => {
    setShowPassword(!showPassword);
  };

  return { showPassword, handleShowPassword };
};
export const useUserNameWithPrefix = () => {
  const [loading, setLoading] = useState(false);
  const [userNamePrefix, setUserNamePrefix] = useState("");
  const handleNextEmployeeUserName = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetNextEmployeeUserName());
    if (response.isSuccess) {
      setUserNamePrefix(response.result.data);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleNextEmployeeUserName();
  }, []);
  const handleUserNamePrefix = (e) => {};
  return { loading, userNamePrefix, handleUserNamePrefix };
};
// export const useUserNameWithPrefix = () => {
//   const prefix = getThisKeyCookie("clIdentifier") || "";
//   const [userNamePrefix, setUserNamePrefix] = useState(prefix);
//   const handleUserNamePrefix = (e) => {
//     setUserNamePrefix(prefix + e.target.value);
//   };
//   return { userNamePrefix, handleUserNamePrefix };
// };

export const fetchMethodResponse = (
  response,
  succesMsg = "Updated successfully",
  successMethod = () => {},
) => {
  if (response) {
    if (response.isSuccess) {
      successNotification(succesMsg);
      successMethod();
    } else {
      UtilityClass.showErrorNotificationWithDictionary(response.errors);
    }
  }
};

export const LoadingTextField = (height) => {
  return (
    <TextFieldComponent
      value="loading..."
      fontSize="12px"
      borderRadius="4px"
      height={height}
    />
  );
};

export const useGetLocationOrPath = () => {
  const location = useLocation();
  const path = `/${location.pathname?.split("/")[1]}`;

  return { location, path };
};

export const s_span = (s) => {
  return (
    <>
      <Box component={"span"} textTransform={"lowercase"}>
        {s}
      </Box>
    </>
  );
};

export const truncate = (str, max = 20) => {
  if (str === null || str === undefined) return "";
  const stringValue = typeof str === "string" ? str : String(str);
  if (!stringValue) return "";
  return (
    <>
      <Tooltip title={stringValue}>
        <span>
          {stringValue.length > max ? stringValue.substring(0, max) + "..." : stringValue}
        </span>
      </Tooltip>
    </>
  );
};

export const decimalFormat = (num) => {
  return (Math.round(Number(num) * 100) / 100).toFixed(2);
};

export function ExcelButton({ onClick, loading = false }) {
  return (
    <LoadingButton
      loading={loading}
      sx={{ ...buttonSX, background: Colors.succes }}
      onClick={onClick}
      variant="contained"
    >
      <Box component={"img"} src={excelicon} width={21} />
    </LoadingButton>
  );
}

export const useGetNavigateState = (pageName, nullState = true) => {
  const [navigateStateData, setNavigateStateData] = useState(null);
  const { state } = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    if (state) {
      const selectedData = state[pageName] ? state[pageName] : null;
      if (selectedData) {
        setNavigateStateData(selectedData);
      }
    }
    const timer = nullState
      ? setTimeout(() => {
          navigate({
            state: null,
          });
        }, 0)
      : () => {};
    return () => clearTimeout(timer);
  }, [state]);

  return { navigateStateData };
};

export const useSetNavigateStateData = (navigateStateData, method) => {
  useEffect(() => {
    if (navigateStateData) {
      method();
    }
  }, [navigateStateData]);
};

export const useNavigateSetState = () => {
  const navigate = useNavigate();
  const setNavigateState = (url, state, newTab, key, carrierId) => {
    if (newTab) {
      // Generate a unique key for the state
      const newTabStateKey = `state_${key}`;

      // Store state in sessionStorage (self-cleaning when tab closes)
      sessionStorage.setItem(newTabStateKey, JSON.stringify(state));
      sessionStorage.setItem("carrierId", carrierId);

      // Append state key to URL
      const newUrl = `${url}?newTabStateKey=${encodeURIComponent(
        newTabStateKey,
      )}`;

      // Open in new tab
      window.open(newUrl, "_blank");
    } else {
      navigate(url, {
        state: state,
      });
    }
  };
  return { setNavigateState };
};

const handleKeyPress = (evt) => {
  // const isBackspace = evt.which === 8;
  // const isNumericKey = (evt.which >= 48 && evt.which <= 57) || evt.which === 46; // 46 corresponds to the dot character
  // // Allow backspace or numeric keys, including the dot
  // if (isBackspace || isNumericKey) {
  //   // Check if there is already a dot in the input
  //   const hasDot = evt.target.value.includes('.');
  //   // Prevent input of another dot if one is already present
  //   if (evt.which === 46 && hasDot) {
  //     evt.preventDefault();
  //   }
  //   return;
  // }
  // evt.preventDefault();
};

export const useSetNumericInputEffect = (dependencies = []) => {
  // useEffect(() => {
  //   const numberInputs = document.querySelectorAll("input[type='number']");
  //   console.log(dependencies, numberInputs);
  //   numberInputs.forEach((input) => {
  //     input.addEventListener("keypress", handleKeyPress);
  //   });
  //   // Clean up the event listeners when the component unmounts
  //   return () => {
  //     numberInputs.forEach((input) => {
  //       input.removeEventListener("keypress", handleKeyPress);
  //     });
  //   };
  // }, [...dependencies]);
};

export const FilterIconButton = ({ onClick }) => {
  return (
    <>
      <IconButton onClick={onClick}>
        <FilterAltIcon />
      </IconButton>
    </>
  );
};

export const CheckboxComponent = ({
  checked,
  onChange,
  label,
  defaultChecked = true,
  id,
}) => {
  return (
    <FormControlLabel
      sx={{
        margin: "0px !important",
      }}
      control={
        <Checkbox
          sx={{
            color: "var(--primary-color)",
            p: 0,
            "&.Mui-checked": {
              color: "var(--primary-color)",
            },
          }}
          id={id}
          checked={checked}
          onChange={onChange}
          size="small"
          edge="start"
          disableRipple
          defaultChecked={defaultChecked}
        />
      }
      label={label}
    />
  );
};

export const getColorByName = (statusName) => {
  const lowercaseStatusName = statusName.toLowerCase().replace(/\s/g, "");
  const statusColors = EnumCarrierTrackingStatus.properties;
  const matchingStatus = Object.values(statusColors).find(
    (status) =>
      status.name.toLowerCase().replace(/\s/g, "") === lowercaseStatusName,
  );

  if (matchingStatus) {
    return matchingStatus;
  } else {
    return "#000000";
  }
};

export const RefreshBtn = ({ onClick, loading, color, ...others }) => {
  return (
    <IconButton onClick={onClick} {...others}>
      {loading ? (
        <CircularProgress size={20} />
      ) : (
        <AutorenewIcon fontSize="small" sx={{ color: color }} />
      )}
    </IconButton>
  );
};

export const handleGetAllCarrierTrackingStatus = async () => {
  let data = [];
  const { response } = await fetchMethod(() =>
    GetAllCarrierTrackingStatusForSelection({}),
  );
  if (response) {
    data = response.result;
  }
  return data;
};
export const useGetAllCarrierTrackingStatus = (dependencies) => {
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] = useState([]);
  const [loading, setLoading] = useState(false);
  const handleGetAllCarrierTrackingStatus = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() =>
      GetAllCarrierTrackingStatusForSelection({}),
    );
    if (response) {
      setAllCarrierTrackingStatus(response.result);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllCarrierTrackingStatus();
  }, [...dependencies]);
  return {
    loading,
    allCarrierTrackingStatus,
  };
};

export const GreyBox = ({ bg = grey, children, sx, ...others }) => {
  return (
    <Box
      sx={{
        bgcolor: bg,
        border: greyBorder,
        borderRadius: 1.25,
        padding: 1.25,
        ...sx,
      }}
      {...others}
    >
      {children}
    </Box>
  );
};

export const SelectTypeBtn = ({ onClick, bgcolor = "#fff", ...others }) => {
  return (
    <Box
      component={ButtonBase}
      className="flex_center"
      onClick={onClick}
      sx={{
        width: "100%",
        height: 50,
        border: `1px dotted ${purple}`,
        borderRadius: "10px",
        gap: 1,
        bgcolor: bgcolor,
      }}
      {...others}
    >
      <AddCircleOutlineIcon color="primary" />
      <Typography variant="h5" color={purple}>
        Select Type
      </Typography>
    </Box>
  );
};
export async function handleDownloadPdf(
  res,
  name = "pdfFile",
  onSuccess = () => {},
) {
  if (res.data?.type === "application/pdf") {
    UtilityClass.downloadPdf(res.data, name);
    onSuccess();
  } else {
    const response = await res.data.text();
    const parsedResponse = JSON.parse(response);
    UtilityClass.showErrorNotificationWithDictionary(parsedResponse?.errors);
  }
}
export async function handleDownloadMenifestPdf(
  res,
  name = "pdfFile",
  onSuccess = () => {},
) {
  const contentType = res.headers?.["content-type"] || res.data?.type;

  if (contentType === "application/pdf") {
    UtilityClass.downloadPdf(res.data, name);
    onSuccess();
  } else {
    try {
      const parsedResponse =
        typeof res.data === "string" ? JSON.parse(res.data) : res.data;

      UtilityClass.showErrorNotificationWithDictionary(parsedResponse?.errors);
    } catch (error) {
      console.error("Error parsing response", error);
      UtilityClass.showErrorNotification("Unexpected error occurred.");
    }
  }
}

export const SelectComponent32Height = {
  height: 32,
  borderRadius: "10px",
};

export const TextFieldErrorBorderColor = "#d32f2f";
export const TextFieldErrorBoxShadow = "0 0 0 1px  #d32f2f";

export const usePagination = (initialPage = 0, initialPageSize = 25) => {
  const [currentPage, setCurrentPage] = useState(initialPage);
  const [pageSize, setPageSize] = useState(initialPageSize);

  const handlePageChange = (newPage) => {
    setCurrentPage(newPage);
  };

  const handlePageSizeChange = (newPageSize) => {
    setPageSize(newPageSize);
  };

  return {
    currentPage,
    pageSize,
    handlePageChange,
    handlePageSizeChange,
  };
};

export const handleFocus = (event) => event.target.select();
export const MAX_TAGS = 150;
export const sampleRows = [
  { id: 0 },
  { id: 1 },
  { id: 2 },
  { id: 3 },
  { id: 4 },
  { id: 5 },
  { id: 6 },
  { id: 7 },
  { id: 8 },
  { id: 9 },
  { id: 10 },
  { id: 11 },
  { id: 12 },
  { id: 13 },
  { id: 14 },
];

export const handleGetAndSetTimeZone = async () => {
  const { response } = await fetchMethod(GetRegionTimeZone);
  if (response && response.isSuccess) {
    setThisKeyCookie(EnumCookieKeys.TIME_ZONE, response.result.timeZone);
  }
};

export const useGeolocation = () => {
  const [autocomplete, setAutocomplete] = useState(null);
  const [error, setError] = useState(null);

  const handleValues = async (selectedLatLng) => {
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?lat=${selectedLatLng.lat}&lon=${selectedLatLng.lng}&format=json`,
      );
      const data = await response.json();
      if (data.address) {
        setAutocomplete(data);
      } else {
        setError("Address not found");
      }
    } catch (error) {
      setError("Error fetching geolocation data");
      console.error("Error fetching geolocation data:", error);
    }
  };

  return { autocomplete, error, handleValues };
};

export function useMapAutocompleteSetter(
  autocomplete,
  allCountries,
  allRegions,
  allCities,
  setValue,
) {
  // useEffect(() => {
  //   if (autocomplete && allCountries) {
  //     const selectedMapCountry = allCountries.find(
  //       (country) => country.name === autocomplete.address.country
  //     );
  //     setValue("country", selectedMapCountry);
  //   }
  // }, [autocomplete, allCountries, setValue]);
  // useEffect(() => {
  //   if (autocomplete && allRegions) {
  //     const selectedMapRegion = allRegions.find(
  //       (region) => region.name === autocomplete.address.state
  //     );
  //     setValue("region", selectedMapRegion);
  //   }
  // }, [autocomplete, allRegions, setValue]);
  // useEffect(() => {
  //   if (autocomplete && allCities) {
  //     const selectedMapCity = allCities.find(
  //       (city) =>
  //         city.name === autocomplete.address.neighbourhood ||
  //         city.name === autocomplete.address.suburb
  //     );
  //     setValue("city", selectedMapCity);
  //   }
  // }, [autocomplete, allCities, setValue]);
}

export const getSelectedValueFromStringOrObject = (
  options = [],
  optionLabel = "",
  value = "",
) => {
  let _selectedOption = { [optionLabel]: "" };
  const isOptionExist = options.some((opt) => opt[optionLabel] === value);
  if (isOptionExist) {
    _selectedOption = options.find((opt) => opt[optionLabel] === value);
  } else {
    const zero_value_obj = {
      [optionLabel]: value,
    };
    _selectedOption = zero_value_obj;
  }

  return _selectedOption;
};

export const getSelectedOrInputValueForApi = (
  data,
  optionValue,
  optionName,
) => {
  // debugger;
  let value = null;
  if (data) {
    value =
      typeof data === "object"
        ? data[optionValue] !== 0
          ? data[optionName]
          : null
        : typeof data === "string"
          ? data
          : null;
  }
  return value;
};

export const getElementByInnerHTML = (innerHTML = "") => {
  const divTags = document.querySelectorAll(".MuiDataGrid-main div");
  let foundElement = null;
  // Loop through each div element
  for (let i = 0; i < divTags.length; i++) {
    const currentDiv = divTags[i];
    if (
      currentDiv.innerHTML.trim().toLowerCase() ===
      innerHTML.trim().toLowerCase()
    ) {
      foundElement = currentDiv;
      break;
    }
  }

  return foundElement;
};

export const checkRefArrayValue = (data = []) => {
  return data?.length > 0;
};
export const getDefaultValueIndex = (options = [], optionValue) => {
  let _defaultIndex = 0;
  if (options.length > 0) {
    _defaultIndex = options[0][optionValue] > 0 ? 0 : 1;
  }
  return _defaultIndex;
};
export const getOptionValueObjectByValue = (
  options = [],
  optionValue,
  value,
) => {
  const _optionIndex = options.find((opt) => opt[optionValue] === value);
  return _optionIndex;
};

export const useGetAllClientTax = (dependencies) => {
  const [allClientTax, setAllClientTax] = useState([]);
  const [loading, setLoading] = useState(true);
  const handleGetAllCientTax = async () => {
    const { response } = await fetchMethod(GetAllClientTax, setLoading, false);
    if (response) {
      setAllClientTax(response.result.list || []);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleGetAllCientTax();
  }, [...dependencies]);
  return {
    loading,
    allClientTax,
  };
};

export const useGetAllGenericSetting = (dependencies, enabled = true) => {
  const [allGenericSetting, setAllGenericSetting] = useState([]);
  const [loading, setLoading] = useState(true);

  const handleGenericSetting = async () => {
    const { response } = await fetchMethod(
      GetGenericSetting,
      setLoading,
      false,
    );

    if (response) {
      const data = response.result ? JSON.parse(response.result) : null;
      setAllGenericSetting(data || []);
      setThisKeyCookie("genericSetting", JSON.stringify(data));
      setGenericCheckboxCookies(data);
    }

    setLoading(false);
  };

  useEffect(() => {
    if (enabled) {
      handleGenericSetting();
    } else {
      setLoading(false);
    }
  }, [...dependencies, enabled]);

  return {
    loading,
    allGenericSetting,
  };
};

const calculateTaxesValue = (data = []) => {
  let taxes = 0;
  data.forEach((dt) => {
    taxes += Number(dt.taxValue);
  });
  return Number(taxes);
};

export const calculateSubtotalValue = (
  discount = 0,
  shipping = 0,
  taxes = [],
  total = 0,
) => {
  const subtotal =
    Number(total) +
    Number(discount) -
    Number(shipping) -
    calculateTaxesValue(taxes);
  return subtotal;
};

export const camelizeArray = (arr) => {
  if (!Array.isArray(arr) || arr.length === 0) return [];

  const toCamel = (str) =>
    str
      .replace(/([-_][a-z])/gi, (s) => s.toUpperCase().replace(/[-_]/g, ""))
      .replace(/^[A-Z]/, (s) => s.toLowerCase());

  return arr.map((obj) =>
    obj && typeof obj === "object"
      ? Object.fromEntries(Object.entries(obj).map(([k, v]) => [toCamel(k), v]))
      : obj,
  );
};

export const HandleResetSeelctedRows = () => {
  const resetRowRef = useRef(false);
  resetRowRef.current = true;
};
export const downloadWayBillsByOrderNos = async (
  orderNo,
  DocumentTemplateId,
) => {
  const params = {
    orderNos: orderNo,
    DocumentTemplateId: DocumentTemplateId,
  };
  await GetWayBillsByOrderNos(params)
    .then((res) => handleDownloadPdf(res, "AWB", HandleResetSeelctedRows))
    .catch((e) => {
      console.log("e", e);
    })
    .finally((e) => {});
};

export const getLowerCase = (val = "") => {
  return val.toLowerCase();
};
export const getTrimValue = (val = "") => {
  return String(val).trim().toLowerCase();
};

export const getTrim = (val = "") => {
  return String(val).trim();
};

export const TextLoading = ({ data, height, xs, sm, md, lg, xl }) => {
  return [...data].map((dt) => (
    <Grid item xs={xs} sm={sm} md={md} lg={lg} xl={xl}>
      <InputLabel required={dt.required} sx={styleSheet.inputLabel}>
        {dt.label}
      </InputLabel>
      <Skeleton
        variant="rectangular"
        height={height}
        sx={{ borderRadius: "4px" }}
      />
    </Grid>
  ));
};
export const useGetWindowHeight = (clientSubscriptionData) => {
  const isPlanNavbarShow =
    clientSubscriptionData?.isTrialMode ||
    clientSubscriptionData?.data?.isSubscriptionCancel;
  const [height, setHeight] = useState(window.innerHeight);
  useLayoutEffect(() => {
    const handleResize = () => {
      setHeight(window.innerHeight);
    };
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);
  return { height: isPlanNavbarShow ? height - 37 : height };
};
export const useGetHeight = (dependencies = []) => {
  const ref = useRef();
  const [height, setHeight] = useState(0);

  useLayoutEffect(() => {
    setHeight(ref.current?.offsetHeight);
  }, [window.innerHeight, ...dependencies]);
  return { ref, height };
};

export const HeightBox = ({ length = 0, sx }) => {
  return (
    <Box
      sx={{
        ...sx,
        height: {
          xs: length === 0 ? 0 : undefined,
          sm: length === 0 ? 0 : undefined,
          md: length === 0 ? 80.25 : undefined,
        },
      }}
    ></Box>
  );
};

export function useHistory() {
  const location = useLocation();
  const [currentPath, setCurrentPath] = useState(location.pathname);
  const [previousPath, setPreviousPath] = useState(null);
  const currentControllers = useRef([]);
  const previousControllers = useRef([]);

  useEffect(() => {
    if (location.pathname !== currentPath) {
      setPreviousPath(currentPath);
      previousControllers.current = currentControllers.current;
      currentControllers.current = [];
      setCurrentPath(location.pathname);
    }
  }, [location, currentPath]);

  return { currentPath, previousPath, currentControllers, previousControllers };
}

export function setupTabLogout() {
  const TAB_ID = Date.now() + "_" + Math.random().toString(36).slice(2);
  const HEARTBEAT_KEY = "tabs_heartbeat";
  const STALE_MS = 3000; // anything older than 3s is considered closed/crashed

  // Clear all cookies
  const clearAll = () => {
    const keysToPreserve = Object.keys(localStorage).filter(
      (key) => key.includes("columnConfig") || key.includes("columns_"),
    );
    const preservedData = {};
    keysToPreserve.forEach((key) => {
      preservedData[key] = localStorage.getItem(key);
    });
    localStorage.clear();
    keysToPreserve.forEach((key) => {
      localStorage.setItem(key, preservedData[key]);
    });
    document.cookie.split(";").forEach((cookie) => {
      const cookieName = cookie.split("=")[0].trim();
      document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    });
  };

  const cleanupStale = () => {
    let tabs = JSON.parse(localStorage.getItem(HEARTBEAT_KEY) || "{}");
    const now = Date.now();
    let changed = false;

    Object.keys(tabs).forEach((id) => {
      if (now - tabs[id] > STALE_MS) {
        delete tabs[id];
        changed = true;
      }
    });

    if (changed) {
      localStorage.setItem(HEARTBEAT_KEY, JSON.stringify(tabs));
    }
    return tabs;
  };

  const heartbeat = () => {
    let tabs = JSON.parse(localStorage.getItem(HEARTBEAT_KEY) || "{}");
    tabs[TAB_ID] = Date.now();
    localStorage.setItem(HEARTBEAT_KEY, JSON.stringify(tabs));
  };

  // === INITIAL SETUP ON TAB LOAD ===
  const cleanedTabs = cleanupStale();

  // If no recent tabs existed → this is after full close → clear everything
  if (Object.keys(cleanedTabs).length === 0) {
    clearAll();
  }

  // Start heartbeating (this tab is now alive)
  heartbeat();
  const heartbeatInterval = setInterval(heartbeat, 1000);

  // Optional but recommended: clean up stale entries periodically
  const cleanupInterval = setInterval(cleanupStale, 5000);

  // Return cleanup function for React
  return () => {
    clearInterval(heartbeatInterval);
    clearInterval(cleanupInterval);
  };
}

export const usePhoneInput = (
  name,
  control,
  getValues = () => {},
  required,
  isContact,
) => {
  const LanguageReducer = useLanguageReducer();
  const PhoneInput = useCallback(() => {
    return (
      <CustomRHFPhoneInput
        error={LanguageReducer?.FIELD_REQUIRED_TEXT}
        name={name}
        selectedCountry={getValues("country")?.mapCountryCode}
        control={control}
        style={{ bg: "#fff", fs: "12px !important" }}
        required={required}
        isContact={isContact}
      />
    );
  }, [getValues("country")]);

  return PhoneInput;
};

export const getBooleanFromObject = (data) => {
  return Object.values(data).length > 0 ? true : false;
};

export const checkObjHasSpecificProperty = (obj = {}, property) => {
  return obj.hasOwnProperty(property);
};

export function getSpacesLabelFromCamelCase(input) {
  // Split the input string by spaces or underscores, and capitalize the first letter of each word
  return input
    .replace(/([a-z])([A-Z])/g, "$1 $2") // Insert space before each capital letter (e.g., "totalOrder" -> "total Order")
    .split(/[\s_]+/) // Split by spaces or underscores
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()) // Capitalize the first letter
    .join(" "); // Join the words with spaces
}

export const graphColorsArr = [
  "#FF5733",
  "#FFBD33",
  "#DBFF33",
  "#75FF33",
  "#33FF57",
  "#33FFBD",
  "#33DBFF",
  "#3375FF",
  "#5733FF",
  "#BD33FF",
  "#FF33DB",
  "#FF3375",
  "#FF3366",
  "#FF3380",
  "#FF3399",
  "#FF33B3",
  "#FF33CC",
  "#FF33E6",
  "#E633FF",
  "#CC33FF",
  "#B333FF",
  "#9933FF",
  "#8033FF",
  "#6633FF",
  "#4D33FF",
  "#333FFF",
  "#334DFF",
  "#3366FF",
  "#3380FF",
  "#3399FF",
  "#33B3FF",
  "#33CCFF",
  "#33E6FF",
  "#33FFE6",
  "#33FFCC",
  "#33FFB3",
  "#33FF99",
  "#33FF80",
  "#33FF66",
  "#33FF4D",
  "#33FF33",
  "#4DFF33",
  "#66FF33",
  "#80FF33",
  "#99FF33",
  "#B3FF33",
  "#CCFF33",
  "#E6FF33",
  "#FFFF33",
  "#FF3333",
  "#FF4D33",
  "#FF6633",
  "#FF8033",
  "#FF9933",
  "#FFB333",
  "#FFCC33",
  "#FFE633",
  "#FFFF66",
  "#FFFF80",
  "#FFFF99",
  "#FFFFB3",
  "#FFFFCC",
  "#FFFFE6",
  "#FFF2E6",
  "#FFE6E6",
  "#FFD9E6",
  "#FFCCE6",
  "#FFBFE6",
  "#FFB3E6",
  "#FFA6E6",
  "#FF99E6",
  "#FF8CE6",
  "#FF80E6",
  "#FF73E6",
  "#FF66E6",
  "#FF59E6",
  "#FF4DE6",
  "#FF40E6",
  "#FF33E6",
  "#E633FF",
  "#CC33FF",
  "#B333FF",
  "#9933FF",
  "#8033FF",
  "#6633FF",
  "#4D33FF",
  "#333FFF",
  "#334DFF",
  "#3366FF",
  "#3380FF",
  "#3399FF",
  "#33B3FF",
  "#33CCFF",
  "#33E6FF",
  "#33FFE6",
  "#33FFCC",
  "#33FFB3",
  "#33FF99",
  "#33FF80",
  "#33FF66",
];
export const orderLabelColor = [
  "#1B2631",
  "#212F3D",
  "#283747",
  "#2E4053",
  "#34495E",
  "#3E4C59",
  "#3B3B98",
  "#2C3E50",
  "#1C2833",
  "#1F618D",
  "#154360",
  "#0E6251",
  "#1A5276",
  "#17202A",
  "#2E86C1",
  "#2471A3",
  "#1F618D",
  "#1B4F72",
  "#512E5F",
  "#4A235A",
  "#6C3483",
  "#5B2C6F",
  "#4A235A",
  "#76448A",
  "#7D3C98",
  "#6E2C00",
  "#873600",
  "#784212",
  "#633974",
  "#5D6D7E",
  "#4D5656",
  "#424949",
  "#1B2631",
  "#2C3E50",
  "#273746",
  "#1A5276",
  "#0B5345",
  "#196F3D",
  "#145A32",
  "#186A3B",
  "#117A65",
  "#0E6655",
  "#239B56",
  "#27AE60",
  "#1E8449",
  "#196F3D",
  "#145A32",
  "#0B5345",
  "#154360",
  "#1C2833",
];

const ToggleIconButton = ({ show, showBtnIcon, hideButtonIcon, onClick }) => {
  return (
    <IconButton onClick={onClick}>
      {show ? showBtnIcon : hideButtonIcon}
    </IconButton>
  );
};

export const ScreenToggleButton = ({ show, onClick }) => {
  return (
    <ToggleIconButton
      show={show}
      showBtnIcon={<FullscreenExitIcon />}
      hideButtonIcon={<FullscreenIcon />}
      onClick={onClick}
    />
  );
};
export const useShowHide = () => {
  const [show, setShow] = useState(false);

  const handleShowHide = () => {
    setShow((prev) => !prev);
  };

  return { show, handleShowHide };
};
export const DataGridAvatar = ({ src, name, onClick }) => {
  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
      <Avatar src={src} sx={{ width: 30, height: 30, fontSize: "13px" }} />
      <CodeBox title={name} onClick={onClick} />
    </Box>
  );
};
export const ToggleButtonComponent = ({ value, onChange, reverse }) => {
  if (reverse) {
    return (
      <ToggleButtonGroup
        value={value}
        exclusive
        onChange={onChange}
        size="small"
      >
        <ToggleButton
          value={viewTypesEnum.TABLE}
          aria-label="module"
          sx={{
            padding: "2px",
            bgcolor:
              value === viewTypesEnum.TABLE
                ? "rgb(86,58,213,.2)!important"
                : undefined,
            color: value === viewTypesEnum.TABLE ? purple : "inherit",
            "& .MuiSvgIcon-root": {
              color: value === viewTypesEnum.TABLE ? purple : "inherit",
            },
          }}
        >
          <ViewListIcon />
        </ToggleButton>
        <ToggleButton
          value={viewTypesEnum.GRID}
          aria-label="list"
          sx={{
            padding: "3px",
            bgcolor:
              value === viewTypesEnum.GRID
                ? "rgb(86,58,213,.2)!important"
                : undefined,
            color: value === viewTypesEnum.GRID ? purple : "inherit",
            "& .MuiSvgIcon-root": {
              color: value === viewTypesEnum.GRID ? purple : "inherit",
            },
          }}
        >
          <ViewModuleIcon />
        </ToggleButton>
      </ToggleButtonGroup>
    );
  } else {
    return (
      <ToggleButtonGroup
        value={value}
        exclusive
        onChange={onChange}
        size="small"
      >
        <ToggleButton
          value={viewTypesEnum.GRID}
          aria-label="list"
          sx={{
            padding: "3px",
            bgcolor:
              value === viewTypesEnum.GRID
                ? "rgb(86,58,213,.2)!important"
                : undefined,
            color: value === viewTypesEnum.GRID ? purple : "inherit",
            "& .MuiSvgIcon-root": {
              color: value === viewTypesEnum.GRID ? purple : "inherit",
            },
          }}
        >
          <ViewModuleIcon />
        </ToggleButton>
        <ToggleButton
          value={viewTypesEnum.TABLE}
          aria-label="module"
          sx={{
            padding: "2px",
            bgcolor:
              value === viewTypesEnum.TABLE
                ? "rgb(86,58,213,.2)!important"
                : undefined,
            color: value === viewTypesEnum.TABLE ? purple : "inherit",
            "& .MuiSvgIcon-root": {
              color: value === viewTypesEnum.TABLE ? purple : "inherit",
            },
          }}
        >
          <ViewListIcon />
        </ToggleButton>
      </ToggleButtonGroup>
    );
  }
};

export const ReusableCardComponent = ({
  image,
  title,
  description,
  children,
  cardActionsChildren,
  onCardActionClick,
  status,
  ActionAreaBackgroundColor,
}) => {
  return (
    <Card
      sx={{
        backgroundColor: ActionAreaBackgroundColor
          ? "rgb(86,58,213,.2)!important"
          : "white",
        borderRadius: "16px",
        padding: "16px 16px 8px 16px",
        border: "1px solid rgba(0, 0, 0, 0.12)",
        boxShadow: "none",
        position: "relative",
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
            padding: "8px 16px",
            borderRadius: "0 0 0 8px",
            fontSize: "0.75rem",
            fontWeight: "bold",
            transform: "translate(10%, -10%)",
          }}
        >
          {status.label}
        </Box>
      )}
      <CardActionArea
        onClick={onCardActionClick}
        sx={{
          ".MuiCardActionArea-focusHighlight": {
            backgroundColor: "transparent",
          },
          ":hover .MuiCardActionArea-focusHighlight": {
            backgroundColor: "transparent",
          },
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          height: "100%",
          width: "100%",
        }}
      >
        {image && (
          <Box
            sx={{
              width: 150,
              height: 100,
              border: greyBorder,
              borderRadius: "10px",
              overflow: "hidden",
            }}
            className={"flex_center"}
          >
            <Box
              component={"img"}
              src={image}
              sx={{ maxWidth: "100%", maxHeight: "100%" }}
            />
          </Box>
        )}
        <CardContent sx={{ textAlign: "center" }}>
          <Typography gutterBottom variant="h5" component="div">
            {title}
          </Typography>
          {children}
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </CardContent>
      </CardActionArea>
      <CardActions>{cardActionsChildren}</CardActions>
    </Card>
  );
};

export function useGetBreakPoint(breakpoint = "md") {
  const theme = useTheme();
  const matches = useMediaQuery(theme.breakpoints.down(breakpoint));
  return matches;
}

// textTrucate
export function textTrucate(truncateText, width = "250px") {
  if (truncateText) {
    return (
      <Tooltip title={truncateText}>
        <span className="d-inline-block text-truncate" style={{ width: width }}>
          {truncateText}
        </span>
      </Tooltip>
    );
  }
}
export const colorSpan = (title, color) => {
  return <span style={{ color: color }}>{title}</span>;
};
export const useSearchMethod = (searchText, method = () => {}) => {
  useEffect(() => {
    const timer = setTimeout(() => {
      method();
    }, 1000);

    return () => {
      clearTimeout(timer);
    };
  }, [searchText]);
};

export const TableRowsLoader = ({ rowsNum = 5, columnsNum = 5 }) => {
  return [...Array(rowsNum)].map((row, index) => (
    <TableRow key={index}>
      {[...Array(columnsNum)].map((col, col_index) => (
        <TableCell component="th" scope="row" key={col_index}>
          <Skeleton animation="wave" variant="text" />
        </TableCell>
      ))}
    </TableRow>
  ));
};

export const TextLoader = ({ rowsNum = 5, columnsNum = 5, height = 50 }) => {
  return [...Array(rowsNum)].map((row, index) => (
    <Box key={index} className={"flex_between"} gap={1}>
      {[...Array(columnsNum)].map((col, col_index) => (
        <Box width={`${100 / columnsNum}%`} height={height}>
          <Skeleton
            animation="wave"
            variant="text"
            sx={{ width: `100%`, height: "100%" }}
          />
        </Box>
      ))}
    </Box>
  ));
};

export function calculateDaysBetween(futureDate) {
  const givenDate = moment(futureDate).format("YYYY-MM-DD");
  const today = moment().format("YYYY-MM-DD"); // Defaults to current date and time
  const time_difference =
    new Date(givenDate).getTime() - new Date(today).getTime();
  const days_difference = time_difference / (1000 * 60 * 60 * 24);
  return days_difference;
}

export const handleGetClientSubscription = async (dispatch) => {
  const { response } = await fetchMethod(GetClientSubscription);
  if (response?.isSuccess) {
    if (getBooleanFromObject(response?.result)) {
      handleDispatchClientSubscription(dispatch, response?.result || {});
    }
  }
};

// Map API Key helpers
export const useGetMapApiKeyReducer = () => {
  const reduxKey = useSelector((state) => state.changeFlagsReducer?.mapApiKey);
  const cookieKey = getThisKeyCookie("map_api_key");
  const localKey =
    typeof window !== "undefined" ? localStorage.getItem("map_api_key") : null;
  if (reduxKey && typeof reduxKey === "string" && reduxKey.trim() !== "")
    return reduxKey;
  if (cookieKey && typeof cookieKey === "string" && cookieKey.trim() !== "")
    return cookieKey;
  if (localKey && typeof localKey === "string" && localKey.trim() !== "")
    return localKey;
  return process.env.REACT_APP_GOOGLE_API_KEY;
};

export const handleDispatchMapApiKey = (dispatch, value) => {
  if (value && typeof value === "string" && value.trim() !== "") {
    try {
      setThisKeyCookie("map_api_key", value);
      localStorage.setItem("map_api_key", value);
    } catch (e) {}
  }
  dispatch(setMapApiKey(value));
};

export const handleClearMapApiKey = (dispatch) => {
  try {
    localStorage.removeItem("map_api_key");
  } catch (e) {}
  dispatch(clearMapApiKey());
};

export const handleGetAndSetMapApiKey = async (dispatch) => {
  try {
    const res = await GetMapApiKey();
    if (res?.data?.isSuccess && res?.data?.result) {
      handleDispatchMapApiKey(dispatch, res.data.result);
    }
  } catch (e) {
    console.error("Error fetching map API key:", e);
  }
};

export const handleCalculateVolume = (length, width, height) => {
  if (length && width && height) {
    return (length * width * height) / 5000;
  }
  return null;
};

export const setTokenCookies = (access_token, refresh_token, id_token) => {
  setThisKeyCookie("access_token", access_token);
  setThisKeyCookie("refresh_token", refresh_token);
  setThisKeyCookie("id_token", id_token);
};
export const handleGetAccessTokenByRefreshToken = async (
  navigate,
  LanguageReducer,
) => {
  const body = {
    UserName: getThisKeyCookie("user_name"),
    RefreshToken: getThisKeyCookie("refresh_token"),
  };
  const { response } = await fetchMethod(() =>
    GetAccessTokenWithRefreshToken(body),
  );
  if (response?.result) {
    const { access_token, refresh_token, id_token } = response?.result;
    setTokenCookies(access_token, refresh_token, id_token);
  } else {
    handleLogout(navigate, LanguageReducer);
  }
};

export const WhatsAppMsg = ({ phoneNumber, message, whatsAppMsgRef }) => {
  return (
    <>
      <a
        ref={whatsAppMsgRef}
        target="_blank"
        href={`https://api.whatsapp.com/send?phone=${phoneNumber}&text=${encodeURIComponent(
          message,
        )}`}
        style={{ display: "none" }} // Hide the link
      ></a>
    </>
  );
};
export const useWhatsAppMsg = () => {
  const whatsAppMsgRef = useRef(null);
  const handleSendWhatsAppMsg = () => {
    whatsAppMsgRef.current.click();
  };

  return { whatsAppMsgRef, handleSendWhatsAppMsg };
};

export const useStepper = (steps = []) => {
  const [activeStep, setActiveStep] = useState(0);
  const showBackBtn = activeStep !== 0;
  const showNextBtn = activeStep !== steps.length - 1; //-1 because active step starts from 0;

  const handleBack = () => {
    setActiveStep((prevActiveStep) => prevActiveStep - 1);
  };

  const handleNext = () => {
    setActiveStep((prevActiveStep) => prevActiveStep + 1);
  };
  return {
    activeStep,
    setActiveStep,
    showBackBtn,
    showNextBtn,
    handleBack,
    handleNext,
  };
};

export default function StepperComp({ steps = [], activeStep = 0 } = {}) {
  return (
    <Box sx={{ width: "100%" }}>
      <Stepper activeStep={activeStep}>
        {steps.map((label, index) => {
          const stepProps = {};
          const labelProps = {};

          return (
            <Step key={label} {...stepProps}>
              <StepLabel {...labelProps}>
                <Typography variant="h4">{label}</Typography>
              </StepLabel>
            </Step>
          );
        })}
      </Stepper>
    </Box>
  );
}

export const handleFilterAddressSchemaSelectDataIncExcValues = (data = {}) => {
  const result = {};
  for (const [key, value] of Object.entries(data)) {
    if (value && value.id !== "") {
      result[key] = {
        Id: String(value.id),
        Include: value.include !== undefined ? value.include : true,
      };
    }
  }
  return result;
};

export const useImageLoader = (imageUrl) => {
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    const img = new Image();
    img.src = imageUrl;
    img.onload = () => setLoaded(true);
  }, [imageUrl]);

  return loaded;
};

export const ImageWithSkeleton = ({ imageUrl, alt }) => {
  const loaded = useImageLoader(imageUrl);

  return loaded ? (
    <CardMedia
      component="img"
      image={imageUrl}
      alt={alt}
      sx={{
        width: "100%",
        height: "100%",
        objectFit: "cover",
      }}
    />
  ) : (
    <Skeleton
      variant="rectangular"
      width="100%"
      height="100%"
      animation="wave"
    />
  );
};

export const ProgressBarWithLabel = ({ value, loading }) => {
  if (!loading) return null;

  return (
    <Box sx={{ display: "flex", alignItems: "center", width: "100%" }}>
      <Box sx={{ flexGrow: 1, mr: 1 }}>
        <LinearProgress variant="determinate" value={value} />
      </Box>
      <Box sx={{ minWidth: 35 }}>
        <Typography variant="body2" sx={{ color: "text.secondary" }}>
          {`${Math.round(value)}%`}
        </Typography>
      </Box>
    </Box>
  );
};

ProgressBarWithLabel.propTypes = {
  value: PropTypes.number.isRequired,
  loading: PropTypes.bool,
};

ProgressBarWithLabel.defaultProps = {
  loading: false,
};
