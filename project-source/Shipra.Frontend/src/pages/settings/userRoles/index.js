import { Box, CircularProgress } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { GetAllClientUserRole } from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";

import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent.js";
import AddUserRoleModal from "../../../components/modals/settingsModals/AddUserRoleModal.js";
import {
  CodeBox,
  greyBorder,
  navbarHeight,
  PageMainBox,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers.js";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile.js";

function UserRolesPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [allUserRoles, setAllUserRoles] = useState({
    loading: false,
    list: [],
  });

  const [selectedRowData, setSelectedRowData] = useState();
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [isRoleAdded, setIsRoleAdded] = useState(false);
  const [roleInfo, setRoleInfo] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  let getAllClientUSerRole = async () => {
    setAllUserRoles((prev) => ({
      ...prev,
      loading: true,
    }));
    let res = await GetAllClientUserRole({});
    if (res.data.result !== null) {
      const data = res.data.result.filter((d, index) => index !== 0);
      setAllUserRoles((prev) => ({
        loading: false,
        list: data,
      }));
    }
  };

  useEffect(() => {
    getAllClientUSerRole();
  }, [isRoleAdded]);
  const handleOpenAddUserRoleModal = () => {
    setOpen(true);
  };
  const handleCloseAddUserRoleModal = () => {
    setOpen(false);
  };

  // const handleEditActionClick = (cTarget, data) => {
  //   // setAnchorEl(cTarget);
  //   setSelectedRowData(data);
  //   handleEditRole();
  // };
  // const handleEditRole = () => {
  //   // setOpenEditModel(true);
  //   setRoleInfo((prev)=>({...prev, open:true}))
  // };
  // const handleDeleteConfirmation = (data) => {
  //   setSelectedRowData(data);
  //   setOpenDeleteRecord(true);
  // };
  // const handleDelete = () => {
  //   if (selectedRowData) {
  //     EnableDisableTax({
  //       clientTaxId: selectedRowData.ClientTaxId,
  //       isActive: selectedRowData.Active === true ? 0 : 1,
  //     })
  //       .then((res) => {
  //         if (!res?.data?.isSuccess) {
  //           errorNotification("Unable to change client tax status");
  //         } else {
  //           successNotification("Client tax status changed successfully");
  //           getAllClientUSerRole();
  //         }
  //       })
  //       .catch((e) => {
  //         console.log("e", e);
  //         errorNotification("Something went wrong");
  //       })
  //       .finally(() => {
  //         setSelectedRowData(null);
  //         setIsDeletedConfirm(false);
  //         setOpenDeleteRecord(false);
  //       });
  //   }
  // };

  // useEffect(() => {
  //   console.log(isDeletedConfirm);
  //   if (isDeletedConfirm) {
  //     handleDelete();
  //   }
  // }, [isDeletedConfirm]);

  // const handleUpdateUserRole = (selectedRowData) =>{
  //   if (selectedRowData) {
  //     setRoleInfo((prev) => ({ ...prev, loading: { [selectedRowData.ClientTaxId]: true } }));
  //     GetClientTaxById({
  //       clientTaxId: selectedRowData.ClientTaxId,
  //     })
  //       .then((res) => {
  //         console.log("res:::", res);
  //         if (!res?.data?.isSuccess) {
  //           UtilityClass.showErrorNotificationWithDictionary(res?.errors);
  //         } else {
  //           setRoleInfo((prev) => ({
  //             ...prev,
  //             open: true,
  //             data: res?.data?.result,
  //           }));
  //         }
  //       })
  //       .catch((e) => {
  //         console.log("e", e);
  //         errorNotification("Something went wrong");
  //       })
  //       .finally(() => {
  //         setRoleInfo((prev) => ({ ...prev, loading: { [selectedRowData.ClientTaxId]: false } }));
  //       });
  //   }
  // }
  const columns = [
    {
      field: "roleName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_Name}
        </Box>
      ),
      minWidth: 400,
      renderCell: (params) => {
        return (
          <>
            {roleInfo.loading[params.row.clientUserRoleId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.roleName}
                  hasColor={false}
                  // onClick={() => handleUpdateUserRole(params.row)}
                />
              </>
            )}
          </>
        );
      },
    },

    {
      field: "roleDescription",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTING_DESCRIPTION}
        </Box>
      ),
      flex: 1,
    },

    // {
    //   field: "Active",
    //   headerName: (
    //     <Box sx={{ fontWeight: "600" }}>
    //       {" "}
    //       {LanguageReducer?.languageType?.STATUS_TEXT}
    //     </Box>
    //   ),
    //   flex: 1,

    //   renderCell: (params) => {
    //     let isActive = params.row.Active;
    //     let title = isActive ? "Active" : "InActive";
    //     return (
    //       <Box>
    //         <Stack
    //           direction="row"
    //           justifyContent="center"
    //           alignItems="center"
    //           spacing={1}
    //         >
    //           <Box>
    //             <StatusBadge
    //               title={title}
    //               color={isActive == false ? "#fff;" : "#fff;"}
    //               bgColor={isActive === false ? Colors.danger : Colors.succes}
    //             />
    //           </Box>
    //         </Stack>
    //       </Box>
    //     );
    //   },
    // },
    // {
    //   field: "Action",
    //   minWidth: 60,
    //   flex: 1,
    //   headerName: (
    //     <Box sx={{ fontWeight: "600" }}>
    //       {" "}
    //       {LanguageReducer?.languageType?.ACTION}
    //     </Box>
    //   ),
    //   renderCell: ({ row, params }) => {
    //     return (
    //       !row?.IsClient && (
    //         <Box>
    //           {row?.Active ? (
    //             <DisableButton onClick={() => handleDeleteConfirmation(row)} />
    //           ) : (
    //             <EnableButton
    //               onClick={(e) => handleDeleteConfirmation(row)}
    //             />
    //           )}
    //         </Box>
    //       )
    //     );
    //   },
    // },
  ];
  const calculatedHeight = windowHeight - navbarHeight - 67;
  return (
    <PageMainBox>
      <ProfileDetailsBox
        sx={{
          boxShadow: "none!important",
          padding: "8px 0px!important",
          border: greyBorder,
          borderBottom: "0px!important",
          borderRadius: "8px 8px 0px 0px",
        }}
        // title={"Users Roles"}
        // description={"Manage user roles for Shipra"}
        rightBtn={
          <div
            style={{
              display: "flex",
              gap: "10px",
            }}
          >
            <ButtonComponent
              sx={{ marginRight: "10px" }}
              title={
                LanguageReducer?.languageType?.SETTING_CHOOSE_ADD_USER_ROLE
              }
              // loading={isloading}
              onClick={handleOpenAddUserRoleModal}
            />
          </div>
        }
      ></ProfileDetailsBox>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          loading={allUserRoles.loading}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
            "& .MuiDataGrid-footerContainer": {
              background: "#fff",
              BorderBottom: "1px none!important",
              borderRadius: "0px 0px 8px 8px",
            },
          }}
          getRowId={(row) => row.clientUserRoleId}
          rows={allUserRoles.list}
          columns={columns}
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />
      </Box>

      {open && (
        <AddUserRoleModal
          open={open}
          setOpen={setOpen}
          onClose={handleCloseAddUserRoleModal}
          setIsRoleAdded={setIsRoleAdded}
        />
      )}
      {/* {roleInfo.open && (
        <EditClientTaxModal
          open={roleInfo.open}
          setRoleInfo={setRoleInfo}
          {...props}
          selectedRowData={selectedRowData}
          allTaxForSelection={allTaxForSelection}
          getAllClientUSerRole={getAllClientUSerRole}
          roleInfo={roleInfo}
        />
      )} */}
      {/* <DeleteConfirmationModal
        open={openDeleteRecord}
        setOpen={setOpenDeleteRecord}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
        heading={"Are you sure to disable this item?"}
        message={
          "After this action the item will be disabled immediately but you can undo this action anytime."
        }
        buttonText={"Disable"}
      /> */}
    </PageMainBox>
  );
}
export default UserRolesPage;
