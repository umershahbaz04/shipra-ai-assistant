import FileUploadOutlined from '@mui/icons-material/FileUploadOutlined';
import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import { Box, Card, Grid, InputLabel } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import PdfExporter from "../../../.reUseableComponents/TableDataPdfExporter/PdfExporter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateBulkStore,
  ExcelExportAddressEntitiesByType,
  GetSampleExcelFileForStoreUpload,
} from "../../../api/AxiosInterceptors";
import { getAllCountryFunc } from "../../../apiCallingFunction";
import { styleSheet } from "../../../assets/styles/style";
import ImportUploadStoreModal from "../../../components/modals/storeModals/ImportUploadStoreModal";
import { EnumOptions } from "../../../utilities/enum";
import { useGetAddressSchema } from "../../../utilities/helpers/addressSchema";
import CountrySchema from "../../../utilities/helpers/countryschema";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  fetchMethod,
} from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import UploadStoreList from "./list";

const UploadStore = (props) => {
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [openImportStoreModal, setOpenImportStoreModal] = useState(false);
  const [uploadStoreData, setUploadStoreData] = useState([]);
  const [btnLoading, setBnLoading] = useState(false);
  const [selectionModel, setSelectionModel] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const pdfRef = useRef(null);
  const [exportData, setExportData] = useState({
    columns: [],
    rows: [],
  });
  const { addressSchemaSelectData, handleSetSchema } = useGetAddressSchema();
  const {
    register,
    formState: { errors },
    setValue,
    getValues,
    control,
    unregister,
  } = useForm();

  useWatch({
    name: "country",
    control,
  });

  let getAllCountry = async () => {
    let data = await getAllCountryFunc();
    if (data?.length > 0) setAllCountries(data);
  };

  const downloadSample = async (id) => {
    let res = await GetSampleExcelFileForStoreUpload(id);
    const link = document.createElement("a");
    if (res?.data?.result?.length > 0) {
      link.href = res.data.result[0].filePath;
      link.download = "StoreSample.xlsx";
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } else {
      errorNotification("No Sample File Exist For This Country");
    }
  };

  const handleUploadStore = async () => {
    if (selectionModel.length === 0) {
      errorNotification("Please select at least one store before proceeding.");
      return;
    }

    setBnLoading(true);
    try {
      const selectedRows = uploadStoreData.filter((item) =>
        selectionModel.includes(item.rowNum),
      );
      const response = await CreateBulkStore({ list: selectedRows });
      if (response?.data?.result) {
        const { successList = [], errors = {} } = response.data.result;
        const failedRows = uploadStoreData
          .map((row) => {
            const isSuccess = successList.some(
              (s) => s.storeName === row.storeName,
            );
            if (isSuccess) return null;
            if (errors[row.storeName]) {
              return {
                ...row,
                hasError: true,
                errorMsg: errors[row.storeName].join(", "),
              };
            }
            return row;
          })
          .filter(Boolean);
        setUploadStoreData(failedRows);
        if (successList.length) {
          setExportData({
            columns: [
              { field: "storeName", headerName: "Store Name", excelWidth: 30 },
              { field: "storeId", headerName: "Store ID", excelWidth: 30 },
              { field: "userName", headerName: "Username", excelWidth: 30 },
              { field: "password", headerName: "Password", excelWidth: 30 },
            ],
            rows: successList,
          });
        }
        UtilityClass.showErrorNotificationWithDictionary(errors);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setBnLoading(false);
    }
  };

  useEffect(() => {
    if (!exportData.rows.length) return;
    pdfRef.current?.generateExcel();
    setExportData({ columns: [], rows: [] });
  }, [exportData]);

  useEffect(() => {
    getAllCountry();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Card
          sx={{
            ...styleSheet.uploadOrderCard,
            borderBottom: "0px !important",
            borderBottomLeftRadius: "0px !important",
            borderBottomRightRadius: "0px !important",
          }}
          variant="outlined"
        >
          <Grid container spacing={1.5} paddingTop={"0px"}>
            <Grid item md={3} sm={12} xs={12} paddingLeft={"12px"}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {LanguageReducer?.languageType?.ORDERS_COUNTRY}
              </InputLabel>
              <CountrySchema
                name="country"
                control={control}
                isRHF={true}
                required={true}
                isRefesh={true}
                handleRefreshClick={getAllCountry}
                {...register("country", {
                  required: {
                    value: true,
                  },
                })}
                value={getValues("country")}
                onChange={(event, newValue) => {
                  const resolvedId = newValue ? newValue : null;
                  handleSetSchema("country", resolvedId, setValue, unregister);
                }}
                errors={errors}
              />
            </Grid>
            <Grid item md={12} sm={6} sx={{ display: "flex" }} gap={1}>
              <Box display={"flex"} alignItems={"center"} width={"100%"}>
                <Box display={"flex"} gap={1} width={"100%"}>
                  <ActionButtonCustom
                    label={LanguageReducer?.languageType?.ORDER_IMPORT}
                    onClick={() => {
                      setOpenImportStoreModal(true);
                    }}
                    startIcon={<FileUploadOutlined />}
                    disabled={getValues("country")?.countryId ? false : true}
                  />
                  <ActionButtonCustom
                    label={"Sample Store"}
                    onClick={() => {
                      downloadSample(getValues("country")?.countryId);
                    }}
                    startIcon={<FileDownloadOutlinedIcon />}
                    disabled={getValues("country")?.countryId ? false : true}
                  />
                  {addressSchemaSelectData?.map((entity, entity_i) => {
                    const prevKey =
                      entity_i === 0
                        ? "country"
                        : addressSchemaSelectData[entity_i - 1].key;
                    return (
                      <ActionButtonCustom
                        background={Colors.succes}
                        label={`Download ${entity.label}`}
                        onClick={async () => {
                          const { response } = await fetchMethod(() =>
                            ExcelExportAddressEntitiesByType(
                              getValues("country")?.countryId,
                              entity.key,
                            ),
                          );
                          UtilityClass.downloadExcel(
                            response,
                            `${entity.label} File`,
                          );
                        }}
                        startIcon={<FileDownloadOutlinedIcon />}
                      />
                    );
                  })}
                </Box>
                <Box
                  width={"40%"}
                  display={"flex"}
                  gap={0.5}
                  justifyContent={"end"}
                >
                  <ActionButtonCustom
                    loading={btnLoading}
                    disabled={selectionModel.length === 0}
                    onClick={() => {
                      handleUploadStore();
                    }}
                    label={"Upload Store"}
                  />
                  <ActionButtonCustom
                    onClick={() => {
                      navigate("/store");
                    }}
                    label={"Store Deshboard"}
                  />
                </Box>
              </Box>
            </Grid>
          </Grid>
        </Card>
        <DataGridTabs
          customBorderRadius={"0px !important"}
          tabData={[
            {
              label: "All",
              route: "/upload-stores",
              children: (
                <UploadStoreList
                  uploadStoreData={uploadStoreData}
                  setUploadStoreData={setUploadStoreData}
                  selectionModel={selectionModel}
                  setSelectionModel={setSelectionModel}
                  allCountries={allCountries}
                />
              ),
            },
          ]}
        />
      </div>
      {openImportStoreModal && (
        <ImportUploadStoreModal
          open={openImportStoreModal}
          onClose={() => setOpenImportStoreModal(false)}
          setUploadStoreData={setUploadStoreData}
          countryId={getValues("country")?.countryId}
        />
      )}
      <PdfExporter
        ref={pdfRef}
        columns={exportData.columns}
        rows={exportData.rows}
        fileName="Created_Stores.xlsx"
      />
    </Box>
  );
};

export default UploadStore;
