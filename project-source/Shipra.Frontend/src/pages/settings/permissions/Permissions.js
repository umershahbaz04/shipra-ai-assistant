import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  InputLabel,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetMenuItemPermissions,
  SaveMenuItemPermissions,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";

import {
  CheckboxComponent,
  CicrlesLoading,
  GridContainer,
  GridItem,
  PageMainBox,
  useGetAllClientUserRole,
} from "../../../utilities/helpers/Helpers";
import AddUserRoleModal from "../../../components/modals/settingsModals/AddUserRoleModal";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import { useSelector } from "react-redux";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

export default function Permission() {
  const [openAddUserRoleModal, setOpenAddUserRoleModal] = useState(false);
  const [isRoleAdded, setIsRoleAdded] = useState(false);
  const [isloading, setIsLoading] = useState(false);

  const handleOpenAddUserRoleModal = () => {
    setOpenAddUserRoleModal(true);
  };

  const handleCloseAddUserRoleModal = () => {
    setOpenAddUserRoleModal(false);
  };

  const handleRefreshClientUserRole = () => {
    setIsRoleAdded((prev) => !prev);
  };

  const [selectedClientRole, setSelectedClientRole] = useState({
    clientUserRoleId: 0,
    roleName: "Select Please",
  });
  const { clientUserRole } = useGetAllClientUserRole(isRoleAdded);

  const [expandedTab, setExpandedTab] = useState({});
  const [menuPermissionsGroup, setMenuPermissionsGroup] = useState({
    loading: false,
    data: [],
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [selectedMenuPermissions, setSelectedMenuPermissions] = useState([]);

  const handleGetMenuItemPermissions = async (roleId) => {
    setMenuPermissionsGroup((prev) => ({ ...prev, loading: true }));
    try {
      const response = await GetMenuItemPermissions(roleId);
      if (response?.data?.isSuccess && Array.isArray(response?.data?.result)) {
        const resultData = response.data.result;
        setMenuPermissionsGroup({ loading: false, data: resultData });
        setSelectedMenuPermissions(resultData);
      } else {
        setMenuPermissionsGroup({ loading: false, data: [] });
        setSelectedMenuPermissions([]);
      }
    } catch (error) {
      console.error("Error fetching menu item permissions:", error);
      setMenuPermissionsGroup({ loading: false, data: [] });
      setSelectedMenuPermissions([]);
    }
  };

  useEffect(() => {
    if (
      selectedClientRole?.clientUserRoleId &&
      selectedClientRole.clientUserRoleId !== 0
    ) {
      handleGetMenuItemPermissions(selectedClientRole.clientUserRoleId);
    } else {
      setMenuPermissionsGroup({ loading: false, data: [] });
      setSelectedMenuPermissions([]);
    }
  }, [selectedClientRole]);

  const handleUpdatePermissions = async () => {
    if (
      !selectedClientRole?.clientUserRoleId ||
      selectedClientRole.clientUserRoleId === 0
    ) {
      errorNotification("Please select a User Role first");
      return;
    }

    setIsLoading(true);

    const menuIds = [];
    const menuItemIds = [];

    selectedMenuPermissions.forEach((menu) => {
      const hasChildren = menu.menuItems && menu.menuItems.length > 0;
      if (hasChildren) {
        menu.menuItems.forEach((item) => {
          if (item.hasPermission) {
            menuItemIds.push(item.menuItemId);
          }
        });
      } else {
        if (menu.hasPermission) {
          menuIds.push(menu.menuId);
        }
      }
    });
    try {
      const res = await SaveMenuItemPermissions(
        selectedClientRole.clientUserRoleId,
        menuItemIds,
        menuIds,
      );
      if (res?.data?.isSuccess) {
        successNotification("Permissions updated successfully");
        handleGetMenuItemPermissions(selectedClientRole?.clientUserRoleId);
      } else {
        if (res?.data?.errors) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          errorNotification("Failed to update permissions");
        }
      }
    } catch (e) {
      console.log("Error saving permissions:", e);
      errorNotification("Something went wrong while saving permissions");
    } finally {
      setIsLoading(false);
    }
  };

  const isAllChecked =
    selectedMenuPermissions.length > 0 &&
    selectedMenuPermissions.every((menu) => {
      const parentChecked = menu.hasPermission;
      const childrenChecked =
        menu.menuItems.length === 0 ||
        menu.menuItems.every((dt) => dt.hasPermission === true);
      return parentChecked && childrenChecked;
    });

  const handleSelectAllChange = (e) => {
    const isChecked = e.target.checked;
    setSelectedMenuPermissions((prev) =>
      prev.map((menu) => ({
        ...menu,
        hasPermission: isChecked,
        menuItems: menu.menuItems.map((sub) => ({
          ...sub,
          hasPermission: isChecked,
        })),
      })),
    );
  };

  console.log(selectedMenuPermissions);

  const hasExpandedTab = Object.values(expandedTab).some((tab) => tab === true);

  const handleExpandCollapseAll = () => {
    const _expandedTab = {};
    if (hasExpandedTab) {
      selectedMenuPermissions.forEach((dt, index) => {
        if (dt.menuItems && dt.menuItems.length > 0) {
          _expandedTab[index] = false;
        }
      });
    } else {
      selectedMenuPermissions.forEach((dt, index) => {
        if (dt.menuItems && dt.menuItems.length > 0) {
          _expandedTab[index] = true;
        }
      });
    }
    setExpandedTab(_expandedTab);
  };

  return (
    <PageMainBox>
      {menuPermissionsGroup.loading ? (
        <CicrlesLoading />
      ) : (
        <ProfileDetailsBox
          rightBtn={
            <div style={{ display: "flex", gap: "10px" }}>
              <ButtonComponent
                style={{ marginRight: "10px" }}
                title={
                  LanguageReducer?.languageType
                    ?.SETTING_PERMISSION_ADD_USER_ROLE
                }
                onClick={handleOpenAddUserRoleModal}
              />
              <div
                style={{
                  display:
                    selectedClientRole?.roleName?.toLowerCase() === "admin"
                      ? "none"
                      : "block",
                }}
              >
                <ButtonComponent
                  title={
                    LanguageReducer?.languageType
                      ?.SETTING_PERMISSION_UPDATE_PERMISSIONS ||
                    "Update Permissions"
                  }
                  loading={isloading}
                  onClick={handleUpdatePermissions}
                />
              </div>
            </div>
          }
        >
          <GridContainer>
            {/* First GridItem - User Role Select */}
            <GridItem item xs={12}>
              <Box
                sx={{
                  backgroundColor: "#ffffff",
                  border: "1px solid #e0e0e0",
                  borderRadius: "6px",
                  padding: "16px 20px",
                }}
              >
                <InputLabel sx={styleSheet.inputLabel}>
                  {"User Role"}
                </InputLabel>
                <SelectComponent
                  name="clientUserRole"
                  options={clientUserRole}
                  value={selectedClientRole}
                  isRefesh={true}
                  handleRefreshClick={handleRefreshClientUserRole}
                  optionLabel={EnumOptions.ROLE.LABEL}
                  optionValue={EnumOptions.ROLE.VALUE}
                  onChange={(name, val) => setSelectedClientRole(val)}
                />
              </Box>
            </GridItem>

            {!selectedClientRole?.clientUserRoleId ||
              selectedClientRole.clientUserRoleId === 0 ? (
              <GridItem xs={12}>
                <Box
                  sx={{
                    backgroundColor: "#ffffff",
                    border: "1px solid #e0e0e0",
                    borderRadius: "6px",
                    padding: "40px 20px",
                    textAlign: "center",
                    boxShadow: "0 2px 4px rgba(0,0,0,0.02)",
                    marginTop: "8px",
                  }}
                >
                  <Typography
                    variant="h6"
                    sx={{
                      color: "var(--primary-color)",
                      fontWeight: 600,
                      fontSize: "18px",
                      fontFamily: "'Lato', 'Inter', sans-serif",
                      mb: 1,
                    }}
                  >
                    Please Select a User Role
                  </Typography>
                  <Typography
                    variant="body2"
                    sx={{
                      color: "#64748b",
                      fontSize: "14px",
                      fontFamily: "'Lato', 'Inter', sans-serif",
                    }}
                  >
                    Please choose a role to view and set permissions.
                  </Typography>
                </Box>
              </GridItem>
            ) : (
              <>
                {/* Select All and Expand All Box */}
                <GridItem xs={12}>
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                      backgroundColor: "#ffffff",
                      border: "1px solid #e0e0e0",
                      borderRadius: "6px",
                      padding: "12px 20px",
                    }}
                  >
                    <Box
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        gap: "12px",
                      }}
                    >
                      <CheckboxComponent
                        label=""
                        checked={isAllChecked}
                        onChange={handleSelectAllChange}
                        sx={{
                          "& .MuiSvgIcon-root": {
                            fontSize: "26px",
                          },
                          "& .MuiCheckbox-root": {
                            color: "#1976d2",
                            padding: "8px",
                            backgroundColor: isAllChecked
                              ? "#e3f2fd"
                              : "transparent",
                            borderRadius: "8px",
                            transition: "all 0.2s ease",
                            "&:hover": {
                              backgroundColor: "#e3f2fd",
                            },
                            "&.Mui-checked": {
                              color: "#1976d2",
                            },
                          },
                        }}
                      />
                      <Typography
                        sx={{
                          fontWeight: 600,
                          fontSize: "15px",
                          color: "#1e293b",
                        }}
                      >
                        {
                          LanguageReducer?.languageType
                            ?.SETTING_PERMISSION_SELECT_ALL
                        }
                      </Typography>
                    </Box>
                    <ButtonComponent
                      title={
                        hasExpandedTab
                          ? "Collapse all"
                          : LanguageReducer?.languageType
                            ?.SETTING_PERMISSION_EXPAND_ALL
                      }
                      onClick={handleExpandCollapseAll}
                    />
                  </Box>
                </GridItem>

                {/* Accordions Section */}
                <GridItem xs={12}>
                  {selectedMenuPermissions.map((item, index) => {
                    const hasChildren =
                      item.menuItems && item.menuItems.length > 0;
                    const totalCount = hasChildren ? item.menuItems.length : 1;
                    const enabledCount = hasChildren
                      ? item.menuItems.filter((dt) => dt.hasPermission === true)
                        .length
                      : item.hasPermission
                        ? 1
                        : 0;

                    const isParentChecked = hasChildren
                      ? item.hasPermission &&
                      item.menuItems.every((dt) => dt.hasPermission === true)
                      : item.hasPermission === true;

                    if (!hasChildren) {
                      // Render a simple flat card without accordion expand icon / open-close functionality
                      return (
                        <Box
                          key={item.menuId || index}
                          sx={{
                            border: "1px solid #e0e0e0",
                            borderRadius: "6px",
                            marginBottom: "12px",
                            backgroundColor: "#ffffff",
                            padding: "22px 24px",
                            minHeight: "68px",
                            boxSizing: "border-box",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            transition: "all 0.3s ease",
                            "&:hover": {
                              borderColor: "#1976d2",
                              boxShadow: "0 4px 12px rgba(25, 118, 210, 0.15)",
                            },
                          }}
                        >
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              gap: "14px",
                            }}
                          >
                            <CheckboxComponent
                              label=""
                              checked={isParentChecked}
                              onChange={(e) => {
                                const isChecked = e.target.checked;
                                setSelectedMenuPermissions((prev) => {
                                  const _selected = [...prev];
                                  const _target = { ..._selected[index] };
                                  _target.hasPermission = isChecked;
                                  _selected[index] = _target;
                                  return _selected;
                                });
                              }}
                              sx={{
                                "& .MuiSvgIcon-root": {
                                  fontSize: "30px",
                                  borderRadius: "6px",
                                },
                                "& .MuiCheckbox-root": {
                                  color: "#1976d2",
                                  padding: "8px",
                                  backgroundColor: isParentChecked
                                    ? "#e3f2fd"
                                    : "transparent",
                                  borderRadius: "6px",
                                  transition: "all 0.2s ease",
                                  "&:hover": {
                                    backgroundColor: "#e3f2fd",
                                  },
                                  "&.Mui-checked": {
                                    color: "#1976d2",
                                  },
                                },
                              }}
                            />
                            <Box>
                              <Typography
                                sx={{
                                  fontWeight: 600,
                                  fontSize: "16px",
                                  color: "#1e293b",
                                  lineHeight: "1.2",
                                }}
                              >
                                {item.menuName}
                              </Typography>
                            </Box>
                          </Box>
                        </Box>
                      );
                    }

                    return (
                      <Accordion
                        key={item.menuId || index}
                        expanded={expandedTab[index] || false}
                        onChange={() =>
                          setExpandedTab((prev) => ({
                            ...prev,
                            [index]: !prev[index],
                          }))
                        }
                        sx={{
                          border: "1px solid #e0e0e0",
                          borderRadius: "6px !important",
                          marginBottom: "12px",
                          boxShadow: "none",
                          "&:before": {
                            display: "none",
                          },
                          "&.Mui-expanded": {
                            margin: "0 0 12px 0",
                            borderColor: "#1976d2",
                            boxShadow: "0 4px 12px rgba(25, 118, 210, 0.12)",
                          },
                        }}
                      >
                        <AccordionSummary
                          expandIcon={
                            <ExpandMoreIcon sx={{ color: "#64748b" }} />
                          }
                          sx={{
                            backgroundColor: "#f8fafc",
                            borderRadius: "6px",
                            padding: "0 20px",
                            minHeight: "64px",
                            "&.Mui-expanded": {
                              minHeight: "64px",
                              borderBottomLeftRadius: 0,
                              borderBottomRightRadius: 0,
                              borderBottom: "1px solid #e2e8f0",
                              backgroundColor: "#f1f5f9",
                            },
                          }}
                        >
                          <Box
                            sx={{
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "space-between",
                              width: "100%",
                              pr: 2,
                            }}
                          >
                            <Box
                              sx={{
                                display: "flex",
                                alignItems: "center",
                                gap: "12px",
                              }}
                              onClick={(e) => e.stopPropagation()}
                            >
                              <CheckboxComponent
                                label=""
                                checked={isParentChecked}
                                onChange={(e) => {
                                  const isChecked = e.target.checked;
                                  setSelectedMenuPermissions((prev) => {
                                    const _selected = [...prev];
                                    const parentMenu = { ..._selected[index] };

                                    parentMenu.hasPermission = isChecked;

                                    const updatedChildren =
                                      parentMenu.menuItems.map((dt) => ({
                                        ...dt,
                                        hasPermission: isChecked,
                                      }));
                                    
                                    parentMenu.menuItems = updatedChildren;
                                    _selected[index] = parentMenu;
                                    return _selected;
                                  });
                                }}
                                sx={{
                                  "& .MuiSvgIcon-root": {
                                    fontSize: "26px",
                                  },
                                  "& .MuiCheckbox-root": {
                                    color: "#1976d2",
                                    padding: "8px",
                                    backgroundColor: isParentChecked
                                      ? "#e3f2fd"
                                      : "transparent",
                                    borderRadius: "8px",
                                    transition: "all 0.2s ease",
                                    "&:hover": {
                                      backgroundColor: "#e3f2fd",
                                    },
                                    "&.Mui-checked": {
                                      color: "#1976d2",
                                    },
                                  },
                                }}
                              />
                              <Box>
                                <Typography
                                  sx={{
                                    fontWeight: 600,
                                    fontSize: "16px",
                                    color: "#1e293b",
                                    lineHeight: "1.2",
                                  }}
                                >
                                  {item.menuName}
                                </Typography>
                              </Box>
                            </Box>
                            <Box
                              sx={{
                                backgroundColor:
                                  enabledCount > 0 ? "#e0e7ff" : "#f1f5f9",
                                color: enabledCount > 0 ? "#4338ca" : "#64748b",
                                borderRadius: "20px",
                                padding: "4px 12px",
                                fontSize: "13px",
                                fontWeight: 600,
                              }}
                            >
                              {enabledCount} of {totalCount} Enabled
                            </Box>
                          </Box>
                        </AccordionSummary>
                        <AccordionDetails
                          sx={{
                            backgroundColor: "#ffffff",
                            padding: "20px 24px",
                          }}
                        >
                          <Box
                            sx={{
                              display: "grid",
                              gridTemplateColumns:
                                "repeat(auto-fill, minmax(220px, 1fr))",
                              gap: "16px",
                            }}
                          >
                            {item.menuItems.map((data, item_ind) => (
                              <Box
                                key={data.menuItemId || item_ind}
                                sx={{
                                  display: "flex",
                                  alignItems: "center",
                                  padding: "8px 12px",
                                  borderRadius: "6px",
                                  backgroundColor: data.hasPermission
                                    ? "#f0fdf4"
                                    : "#f8fafc",
                                  border: `1px solid ${data.hasPermission ? "#bbf7d0" : "#e2e8f0"
                                    }`,
                                  transition: "all 0.2s ease",
                                  "&:hover": {
                                    backgroundColor: data.hasPermission
                                      ? "#dcfce7"
                                      : "#f1f5f9",
                                  },
                                }}
                              >
                                <CheckboxComponent
                                  label=""
                                  checked={data.hasPermission || false}
                                  onChange={(e) => {
                                    const isChecked = e.target.checked;
                                    setSelectedMenuPermissions((prev) => {
                                      const _selected = [...prev];
                                      const parentMenu = {
                                        ..._selected[index],
                                      };
                                      const updatedChildren = [
                                        ...parentMenu.menuItems,
                                      ];

                                      const child = {
                                        ...updatedChildren[item_ind],
                                      };
                                      child.hasPermission = isChecked;
                                      updatedChildren[item_ind] = child;

                                      parentMenu.menuItems = updatedChildren;
                                      parentMenu.hasPermission =
                                        updatedChildren.some(
                                          (sub) => sub.hasPermission,
                                        );

                                      _selected[index] = parentMenu;
                                      return _selected;
                                    });
                                  }}
                                  sx={{
                                    "& .MuiSvgIcon-root": {
                                      fontSize: "20px",
                                    },
                                    "& .MuiCheckbox-root": {
                                      color: data.hasPermission
                                        ? "#4caf50"
                                        : "#9e9e9e",
                                      padding: "6px",
                                      "&.Mui-checked": {
                                        color: "#4caf50",
                                      },
                                    },
                                  }}
                                />
                                <Typography
                                  sx={{
                                    fontSize: "14px",
                                    fontWeight: data.hasPermission ? 600 : 400,
                                    color: data.hasPermission
                                      ? "#2e7d32"
                                      : "#424242",
                                    marginLeft: "8px",
                                    userSelect: "none",
                                  }}
                                >
                                  {data.menuItemName}
                                </Typography>
                              </Box>
                            ))}
                          </Box>
                        </AccordionDetails>
                      </Accordion>
                    );
                  })}
                </GridItem>
              </>
            )}
          </GridContainer>
        </ProfileDetailsBox>
      )}
      {openAddUserRoleModal && (
        <AddUserRoleModal
          open={openAddUserRoleModal}
          setOpen={setOpenAddUserRoleModal}
          onClose={handleCloseAddUserRoleModal}
          setIsRoleAdded={setIsRoleAdded}
        />
      )}
    </PageMainBox>
  );
}
