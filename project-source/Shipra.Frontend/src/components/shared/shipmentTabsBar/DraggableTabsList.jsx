import TabContext from "@mui/lab/TabContext";
import TabList from "@mui/lab/TabList";
import TabPanel from "@mui/lab/TabPanel";
import { Badge } from "@mui/material";
import Box from "@mui/material/Box";
import Stack from "@mui/material/Stack";
import Tab from "@mui/material/Tab";
import * as React from "react";
import { DragDropContext, Droppable } from "react-beautiful-dnd";
import { styleSheet } from "../../../assets/styles/style";
import DraggableTab from "./DraggableTab";

export default function DraggableTabsList(props) {
  const _renderTabList = (droppableProvided) => (
    <TabList
      onChange={props?.handleChange}
      aria-label="Draggable Tabs"
      variant="scrollable"
      allowScrollButtonsMobile
      sx={{
        ...styleSheet.customTabsUI,
        justifyContent: "space-around",
        overflowX: "auto",
        "@media (max-width: 600px)": {
          display: "flex",
          width: "520px",
          flexWrap: "no-wrap",
        },
        "@media (max-width: 550px)": {
          display: "flex",
          width: "480px",
          flexWrap: "no-wrap",
        },
        "@media (max-width: 500px)": {
          display: "flex",
          width: "435px",
          flexWrap: "no-wrap",
        },
        "@media (max-width: 450px)": {
          display: "flex",
          width: "360px",
          flexWrap: "no-wrap",
        },
        "@media (max-width: 400px)": {
          display: "flex",
          width: "335px",
          flexWrap: "no-wrap",
        },
      }}
    >
      {props.tabs.map((tab, index) => {
        const child = (
          <Tab
            sx={{
              fontSize: "12px",
              minWidth: "60px",
              paddingLeft: "15px",
              paddingRight: "25px",
              minHeight: "33px !important",
              fontWeight: 600,
              width: "max-content",
              cursor: "move !important",
            }}
            label={
              <Box
                display={"-webkit-inline-box"}
                gap={3}
                justifyContent={"space-between"}
              >
                <Badge
                  max={100000}
                  badgeContent={String(tab.count || 0)}
                  sx={{
                    "& .MuiBadge-badge": {
                      backgroundColor: "var(--primary-color)",
                      width: "17px",
                      minWidth: "17px",
                      height: "17px",
                      color: "white",
                      padding: "0 4px",
                      right: -8,
                      top: 7,
                    },
                  }}
                ></Badge>
                <Box>{tab.label}</Box>
              </Box>
            }
            value={tab[props.valueParam] || ""}
            key={index}
          />
        );

        return (
          <DraggableTab
            label={
              <Box
                display={"flex"}
                gap={3}
                width="auto"
                minWidth="auto"
                justifyContent={"space-between"}
                paddingRight="2px!important"
              >
                <Badge
                  max={100000}
                  badgeContent={String(tab.count || 0)}
                  sx={{
                    "& .MuiBadge-badge": {
                      borderRadius: "2px!important",
                      backgroundColor: "var(--primary-color)",
                      width: "auto",
                      minWidth: "auto",
                      height: "auto",
                      color: "white",
                      padding: "4px 8px",
                      right: -8,
                      top: 7,
                    },
                  }}
                ></Badge>
                <Box flex="1" paddingLeft="8px">
                  {tab.label}
                </Box>
              </Box>
            }
            value={tab[props.valueParam] || ""}
            index={index}
            key={index}
            child={child}
          />
        );
      })}
      {droppableProvided ? droppableProvided.placeholder : null}
    </TabList>
  );

  const _renderTabListWrappedInDroppable = () => (
    <DragDropContext onDragEnd={props.onDragEnd}>
      <div>
        <Droppable droppableId="1" direction="horizontal">
          {(droppableProvided) => (
            <div
              ref={droppableProvided.innerRef}
              {...droppableProvided.droppableProps}
            >
              {_renderTabList(droppableProvided)}
            </div>
          )}
        </Droppable>
      </div>
    </DragDropContext>
  );

  return (
    <Box sx={{ width: "100%", typography: "body1" }}>
      <TabContext value={props.value}>
        <Box sx={{ borderColor: "divider" }}>
          <Stack direction="column">{_renderTabListWrappedInDroppable()}</Stack>
        </Box>
        {props.tabs.map((tab, index) => (
          <TabPanel value={tab.value} key={index}>
            {tab.content}
          </TabPanel>
        ))}
      </TabContext>
    </Box>
  );
}
