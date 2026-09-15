import FilterAltOutlined from '@mui/icons-material/FilterAltOutlined';
import { Box, Button, CircularProgress, Grid, Stack } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLocation } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import { ActionButtonWithPoper } from "../../../utilities/helpers/Helpers";
import DraggableTabsList from "./DraggableTabsList";

// const StyledBadge = styled(Badge)(({ theme }) => ({
//   '& .MuiBadge-badge': {
//     right: -3,
//     top: 13,
//     border: `2px solid ${theme.palette.background.paper}`,
//     padding: '0 4px',
//   },
// }));

const GeneralTabBar = (props) => {
  let {
    onChangeMenu,
    handleAddTab,
    options,
    placement,
    id,
    minWidth,
    placeholder,
    setUpdateShipmentTabs,
    optionsTabSetting,
    value,
    setTrackingStatusIds,
    tabData,
    disableFilter,
    disableSearch,
    width,
    padding,
    isFilterOpen,
    setIsFilterOpen,
    copybtn,
    loading,
    tabGridWidth = { md: 9, lg: 9, sm: 12 },
    btnGridWidth = { md: 3, lg: 3, sm: 12 },
  } = props;
  const location = useLocation();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [tabValue, setTabValue] = useState("0");

  const handleTabChange = (event, statusIds) => {
    setTabValue(statusIds);
    setTrackingStatusIds(statusIds);
    props.getShipments(statusIds);
  };
  //for dynamic option
  const anchorRef = React.useRef(null);
  const [selectedIndex, setSelectedIndex] = React.useState("");
  const [open, setOpen] = React.useState(false);

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

  const [activeTab, setActiveTab] = useState(0);

  const onDragEnd = (result) => {
    if (result.destination) {
      const newTabs = Array.from(tabData);
      const draggedTab = newTabs.splice(result.source.index, 1)[0];
      newTabs.splice(result.destination.index, 0, draggedTab);

      var newUpdatedTabs = newTabs.map((status, index) => {
        return { ...status, displayOrder: index + 1 };
      });
      //update display order
      //update shipments tab
      setUpdateShipmentTabs(newUpdatedTabs);
    }
  };

  return (
    <Grid
      sx={{
        background: "#F8F8F8",
        borderRadius: "0px",
        border: "1px solid rgba(224, 224, 224, 1)",
        borderBottom: "none",
        display: "flex",
        justifyContent: "flex-start",
        alignItems: "center",
        padding: " 7px",
        "& .MuiTabs-root": {
          minHeight: "26px !important",
        },
      }}
      container
      xs={12}
    >
      <Grid
        item
        md={tabGridWidth.md}
        lg={tabGridWidth.lg}
        sm={tabGridWidth.sm}
        sx={{ marginBottom: { xs: 1, sm: 1, md: 0 } }}
      >
        {loading ? (
          <Box alignItems="center" height="100%">
            <CircularProgress size={20} />
          </Box>
        ) : (
          <DraggableTabsList
            onDragEnd={onDragEnd}
            tabs={tabData}
            handleChange={handleTabChange}
            value={tabValue}
            valueParam={"statusIds"}
          />
        )}
      </Grid>
      <Grid item md={btnGridWidth.md} lg={btnGridWidth.lg} sm={btnGridWidth.sm} width={"100%"}>
        <Stack
          direction="row"
          justifyContent="flex-end"
          alignItems="center"
          spacing={1}
        >
          {copybtn}
          {optionsTabSetting && (
            <>
              <ActionButtonWithPoper
                placement={placement}
                onClick={(event, index) => handleAddTab(event, index)}
                options={optionsTabSetting}
                placeholder={
                  LanguageReducer?.languageType?.SHIPMENTS_TAB_SETTING
                }
                minWidth={"100px"}
              />
            </>
          )}
          {options && (
            <>
              <ActionButtonWithPoper
                placement={placement}
                onClick={(event, index) => handleMenuItemClick(event, index)}
                options={options}
                placeholder={LanguageReducer?.languageType?.SHIPMENT_ACTION}
              />
            </>
          )}
          <Button
            sx={styleSheet.filterIconColord}
            color="inherit"
            variant="outlined"
            onClick={() => setIsFilterOpen(!isFilterOpen)}
            startIcon={<FilterAltOutlined fontSize="small" />}
          >
            {LanguageReducer?.languageType?.ORDERS_FILTER}
          </Button>
        </Stack>
      </Grid>
    </Grid>
  );
};
export default GeneralTabBar;
