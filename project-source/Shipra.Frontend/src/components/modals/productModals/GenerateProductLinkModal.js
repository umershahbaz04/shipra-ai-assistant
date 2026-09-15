import { Box, Link, Tooltip } from "@mui/material";
import { useState } from "react";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { styleSheet } from "../../../assets/styles/style";
import {
  ActionButtonCustom,
  centerColumn,
  ClipboardIcon,
  CodeBox,
  DataGridHeaderBox,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import {
  GenerateProductLinkToken,
  GetAllStoreWithTokenByProductId,
} from "../../../api/AxiosInterceptors";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const GenerateProductLinkModal = (props) => {
  const {
    open,
    onClose,
    GenerateProductLinkData,
    setOpenGenerateProductLinkModal,
  } = props;
  const [loading, setLoading] = useState({});

  const handleGenerateProductLink = async (dt) => {
    const StoreId = dt.StoreId;
    setLoading((prev) => ({ ...prev, [StoreId]: true }));
    try {
      const response = await GenerateProductLinkToken(
        GenerateProductLinkData.productId,
        dt.StoreId
      );
      if (response?.data?.isSuccess) {
        successNotification("Link Generate Successfully");
        const res = await GetAllStoreWithTokenByProductId(
          GenerateProductLinkData.productId
        );
        if (res.data.isSuccess) {
          const newData = res?.data?.result?.list;
          setOpenGenerateProductLinkModal((prev) => ({
            ...prev,
            data: newData,
          }));
        }
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error("Error generating product link", error);
    } finally {
      setLoading((prev) => ({ ...prev, [StoreId]: false }));
    }
  };
  const configColumns = [
    {
      field: "OrderNo",
      headerName: <DataGridHeaderBox title={"Title"} />,
      minWidth: 70,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.StoreName} />
          </>
        );
      },
    },
    {
      field: "OrderLink",
      headerName: <DataGridHeaderBox title={"Order Link"} />,
      minWidth: 300,
      flex: 1,
      renderCell: ({ row }) => {
        // const orderLink = row.OrderLink.split("/").pop();
        return (
          <>
            <Box display="flex" alignItems="center">
              <Box
                sx={{
                  whiteSpace: "nowrap",
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  maxWidth: 200,
                }}
              >
                <Tooltip title={row.OrderLink} arrow>
                  <Link
                    href={row.OrderLink}
                    target="_blank"
                    rel="noopener noreferrer"
                    sx={{
                      textDecoration: "none",
                      display: "block", // Ensure block-level element for ellipsis
                    }}
                  >
                    {row.OrderLink}
                  </Link>
                </Tooltip>
              </Box>
              <Box>
                {row.OrderLink && <ClipboardIcon text={row.OrderLink} />}
              </Box>
            </Box>
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "action",
      headerName: <DataGridHeaderBox title={"Action"} />,
      minWidth: 140,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <Box padding={0.5}>
              <ActionButtonCustom
                sx={{
                  ...styleSheet.integrationactivatedButton,
                  width: "100%",
                  height: "30px",
                  borderRadius: "4px",
                }}
                variant="contained"
                loading={loading?.[row.StoreId]}
                onClick={() => handleGenerateProductLink(row)}
                label={"Generate Link"}
              />
            </Box>
          </>
        );
      },
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Generate product Link"}
    >
      <GridContainer spacing={1}>
        <GridItem xs={11} sm={12} md={12} lg={12}>
          <Box>
            <DataGridComponent
              autoHeight
              bgColor={"#fff"}
              getRowHeight={() => "auto"}
              headerHeight={40}
              sx={{
                fontFamily:
                  "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
                minHeight: "150px",
              }}
              rows={GenerateProductLinkData?.data}
              getRowId={(row) => row.RowNum}
              columns={configColumns}
              checkboxSelection={false}
              disableSelectionOnClick
              pageSize={10}
              rowsPerPageOptions={[5, 10, 15, 25]}
            />
          </Box>
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
};

export default GenerateProductLinkModal;
