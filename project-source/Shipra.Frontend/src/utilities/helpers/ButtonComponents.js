import CheckCircle from '@mui/icons-material/CheckCircle';
import Delete from '@mui/icons-material/Delete';
import Edit from '@mui/icons-material/Edit';
import FileUpload from '@mui/icons-material/FileUpload';
import FilterAltOutlined from '@mui/icons-material/FilterAltOutlined';
import Map from '@mui/icons-material/Map';
import MoneyOff from '@mui/icons-material/MoneyOff';
import NotInterested from '@mui/icons-material/NotInterested';
import PictureAsPdf from '@mui/icons-material/PictureAsPdf';
import PriceCheck from '@mui/icons-material/PriceCheck';
import Undo from '@mui/icons-material/Undo';
import AddCircleOutlineIcon from "@mui/icons-material/AddCircleOutline";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import FilterAltIcon from "@mui/icons-material/FilterAlt";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import VisibilityIcon from "@mui/icons-material/Visibility";
import { LoadingButton } from "@mui/lab";
import {
    Backdrop,
    Box,
    Button,
    ButtonBase,
    CircularProgress,
    ClickAwayListener,
    Fade,
    Grow,
    IconButton,
    ListItemIcon,
    MenuItem,
    MenuList,
    Paper,
    Popper,
    Typography
} from "@mui/material";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../assets/styles/style";
import Colors from "./Colors";
export function CrossIconButton({ onClick, color }) {
  return (
    <IconButton onClick={onClick}>
      <CloseIcon sx={{ color: color }} />
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
    height = "29px",
    loading = false,
    background = Colors.primary,
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
        }}
        disabled={loading}
        color="inherit"
        variant="outlined"
        id="demo-customized-button"
        aria-haspopup="true"
        disableElevation
        {...props}
      >
        {!label ? LanguageReducer?.languageType?.ACTION : label}
      </LoadingButton>
    </>
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
  export function PDFButton({ onClick, loading = false }) {
    return (
      <LoadingButton
        loading={loading}
        sx={{ ...buttonSX, background: Colors.pdf }}
        onClick={onClick}
        variant="contained"
      >
        <StyledTooltip title="Pdf">
          <PictureAsPdf sx={buttonIconSX} />
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
  export const FilterIconButton = ({ onClick }) => {
    return (
      <>
        <IconButton onClick={onClick}>
          <FilterAltIcon />
        </IconButton>
      </>
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