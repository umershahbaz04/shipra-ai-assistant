import { Button, ButtonBase, Grid, Typography } from "@material-ui/core";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import VisibilityIcon from "@mui/icons-material/Visibility";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  GetDocumentSetting,
  UpdateDocumentSetting,
} from "../../../api/AxiosInterceptors";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  CheckboxComponent,
  GreyBox,
  PageMainBox,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import { Link } from "react-router-dom";
import { useSelector } from "react-redux";

const MixSettings = () => {
  // States:
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [fetchedData, setFetchedData] = useState([]);
  const [payload, setPayload] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [documentSizeId, setDocumentSizeId] = useState(null);
  // Functions:
  const fetchingData = async () => {
    let res = await GetDocumentSetting();
    if (res.data.result != null) {
      let data = res.data.result;
      setFetchedData(res.data.result);
      data.map((dataset) => {
        let docTempId = dataset?.documentTemplate.filter((doc) =>
          doc.isSelected ? doc : ""
        );
        setPayload([
          ...payload,
          {
            documentTemplateId: docTempId[0]?.documentTemplateId,
            templateName: dataset.TemplateName,
            documentTemplateConfigId: dataset?.documentTemplateConfigId,
            isAskEveryTime: dataset.IsAskEveryTime,
          },
        ]);
      });
    }
  };

  const updateDocumentSetting = async () => {
    setIsLoading(true);
    let body = {
      list: [...payload],
    };

    let res = await UpdateDocumentSetting(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Something went wrong while updating settings");
          errorNotification(res?.data?.customErrorMessage);
          setIsLoading(false);
        } else {
          successNotification("Settings updated successfully");
          setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        setIsLoading(false);
        errorNotification("Unable to update settings");
      });
  };

  const selectedTempChange = (tempId, sectionIndex) => {
    let updatedPayload = payload.map((singlePayload, index) => {
      if (index !== sectionIndex) {
        return singlePayload;
      } else {
        singlePayload = {
          ...singlePayload,
          documentTemplateId: tempId,
        };
        return singlePayload;
      }
    });

    setPayload(updatedPayload);
  };

  const templateNameChange = (e, val, sectionIndex) => {
    let updatedPayload = payload.map((singlePayload, index) => {
      if (index !== sectionIndex) {
        return singlePayload;
      } else {
        singlePayload = {
          ...singlePayload,
          templateName: val,
        };
        return singlePayload;
      }
    });

    setPayload(updatedPayload);
  };

  const askEverytimeHandler = (askEverytimeVal, sectionIndex) => {
    let updatedPayload = payload.map((singlePayload, index) => {
      if (index === sectionIndex) {
        singlePayload = {
          ...singlePayload,
          isAskEveryTime: !askEverytimeVal,
        };

        return singlePayload;
      }

      return singlePayload;
    });

    setPayload(updatedPayload);
  };

  // Effects:
  useEffect(() => {
    fetchingData();
  }, []);

  return (
    <PageMainBox>
      <ProfileDetailsBox>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            {fetchedData?.map((section, sectionIndex) => {
              return (
                <Accordion
                  defaultExpanded
                  key={sectionIndex}
                  sx={{
                    boxShadow: "none !important",
                    borderTop: "1px solid #d3d3d375",
                    margin: "0px !important",
                    backgroundColor: "unset",
                  }}
                >
                  <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls={`section${sectionIndex}-content`}
                    id={`section${sectionIndex}-header`}
                    sx={{
                      padding: "0 16px",
                      fontSize: "16px",
                      fontWeight: 500,
                    }}
                  >
                    {section.value}
                  </AccordionSummary>
                  <AccordionDetails>
                    <Grid container spacing={3}>
                      {section?.documentTemplate?.map((data, index) => (
                        <Grid item xs={12} md={4} lg={4} key={index}>
                          <Box display="flex" gap={2}>
                            <Box
                              key={data.documentTemplateId}
                              width="100%"
                              position="relative"
                              mb={2}
                            >
                              <GreyBox
                                key={data.documentTemplateId}
                                bg={
                                  payload[sectionIndex].documentTemplateId ===
                                  data.documentTemplateId
                                    ? "lightgrey"
                                    : "unset"
                                }
                                className="flex_center"
                                component={Link}
                                width="100%"
                                onClick={() => {
                                  selectedTempChange(
                                    data.documentTemplateId,
                                    sectionIndex
                                  );
                                }}
                              >
                                <img
                                  width="250px"
                                  height="250px"
                                  src={data.sampleImageUrl}
                                  alt="Image Not Found"
                                />
                              </GreyBox>
                              <h3
                                className="flex_center"
                                style={{ marginTop: "20px" }}
                              >
                                {data.name}
                              </h3>
                              <Link
                                to={data.samplePdfUrl}
                                target="_blank"
                                className="eyeBtn"
                                style={{
                                  position: "absolute",
                                  top: "8px",
                                  right: "8px",
                                  minWidth: "auto",
                                  padding: 0,
                                  color: "#000",
                                }}
                              >
                                <VisibilityIcon />
                              </Link>
                            </Box>
                          </Box>
                        </Grid>
                      ))}
                    </Grid>
                    <Box marginLeft="11px" marginRight="9px" marginTop="20px">
                      <TextFieldComponent
                        name="templateName"
                        value={payload[sectionIndex]?.templateName}
                        placeholder="Enter Template Name"
                        required={true}
                        height={40}
                        borderRadius="5px"
                        onChange={(e) => {
                          templateNameChange(e, e.target.value, sectionIndex);
                        }}
                      />
                    </Box>
                    <Box marginLeft="21px" marginTop="5px">
                      <CheckboxComponent
                        key={sectionIndex}
                        label={`\u00A0 Ask Every Time`}
                        value={payload[sectionIndex].isAskEveryTime}
                        checked={payload[sectionIndex].isAskEveryTime}
                        onChange={() =>
                          askEverytimeHandler(
                            payload[sectionIndex].isAskEveryTime,
                            sectionIndex
                          )
                        }
                      />
                      <Box
                        sx={{ fontSize: 13 }}
                        marginLeft="15px"
                        marginTop="4px"
                      >
                        <strong>Note:</strong> If this option will be checked
                        then you will always have to choose design from pop up
                        while print label.
                      </Box>
                    </Box>
                  </AccordionDetails>
                </Accordion>
              );
            })}
          </Grid>
          <Grid item xs={12}>
            <Box display="flex" justifyContent="end" marginTop="20px">
              <ActionButtonCustom
                label={
                  LanguageReducer?.languageType?.SETTINGS_DOCUMENT_SETTINGS_SAVE
                }
                background={Colors.primary}
                loading={isLoading}
                onClick={updateDocumentSetting}
              />
            </Box>
          </Grid>
        </Grid>
      </ProfileDetailsBox>
    </PageMainBox>
  );
};

export default MixSettings;
