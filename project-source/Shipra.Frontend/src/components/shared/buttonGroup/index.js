import React, { useEffect } from "react";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import ClickAwayListener from "@mui/material/ClickAwayListener";
import Grow from "@mui/material/Grow";
import Paper from "@mui/material/Paper";
import Popper from "@mui/material/Popper";
import MenuItem from "@mui/material/MenuItem";
import { MenuList, Fade, ListItemIcon } from "@mui/material";

function ButtonGroups(props) {
  let {
    options,
    id,
    bgColor,
    bgColorHover,
    height,
    placement,
    onChangeMenu,
    placeholder,
    size,
    minWidth,
    fontSize,
    color,
    bgColor1,
    borderColor,
    value,
  } = props;
  const [open, setOpen] = React.useState(false);
  const anchorRef = React.useRef(null);
  const [selectedIndex, setSelectedIndex] = React.useState("");

  useEffect(() => {
    if (value) { 
      let index = options.findIndex((item) => item.value === value);
      setSelectedIndex(index);
    }
  }, [value]);

  const handleMenuItemClick = (event, index) => { 
    setSelectedIndex(index);
    setOpen(false);
    if (options[index]?.value) {
      onChangeMenu(options[index]?.value);
    }
  };

  const handleToggle = () => {
    setOpen((prevOpen) => !prevOpen);
  };

  const handleClose = (event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target)) {
      return;
    }
    setOpen(false);
  };
  return (
    <React.Fragment>
      <ButtonGroup
        variant="contained"
        ref={anchorRef}
        aria-label="split button"
        sx={{ background: bgColor, height: height ? `${height} !important` : "35px" }}
      >
        <Button
          sx={{
            fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
            minWidth: `${minWidth || "120px"} !important`,
            color: `${color} !important`,
            border: borderColor ? `1px solid ${borderColor} !important` : `${bgColor}  !important`,
            background: bgColor,
            fontSize: fontSize,
            "&:hover": { background: bgColorHover },
          }}
          size={size}
          onClick={handleToggle}
        >
          {options[selectedIndex]?.title ? options[selectedIndex]?.title : options[selectedIndex] || placeholder}
        </Button>
        <Button
          size="small"
          aria-controls={open ? id : undefined}
          aria-expanded={open ? "true" : undefined}
          aria-label="select merge strategy"
          aria-haspopup="menu"
          onClick={handleToggle}
          sx={{
            color: `${color} !important`,
            border: borderColor ? `1px solid ${borderColor}` : "",
            background: bgColor1 || bgColor,
            "&:hover": { background: bgColorHover },
          }}
        >
          <ArrowDropDownIcon />
        </Button>
      </ButtonGroup>
      <Popper open={open} anchorEl={anchorRef.current} transition disablePortal placement={placement} sx={{ zIndex: 10000 }}>
        {({ TransitionProps }) => (
          <Fade {...TransitionProps} timeout={350}>
            <Grow {...TransitionProps}>
              <Paper>
                <ClickAwayListener onClickAway={handleClose}>
                  <MenuList
                    sx={{ minWidth: minWidth ? `${Number(minWidth.split("px")[0]) + 40}px !important` : "162px !important" }}
                    id={id}
                    autoFocusItem
                  >
                    {options.map((option, index) => (
                      <MenuItem
                        key={index}
                        selected={index === selectedIndex}
                        onClick={(event) => handleMenuItemClick(event, index)}
                        value={option.value ? option.value : option}
                      >
                        {option.icon ? <ListItemIcon>{option.icon}</ListItemIcon> : null}
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
    </React.Fragment>
  );
}

export default ButtonGroups;
