import VisibilityIcon from "@mui/icons-material/Visibility";
import { Box, Button, ButtonBase, Link, Typography } from "@mui/material";
import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  GreyBox,
  GridContainer,
  GridItem,
  downloadWayBillsByOrderNos,
  purple,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { handleDispatchUserProfile } from "../../topNavBar";
import { useSelector } from "react-redux";

const AddBillingModal = (props) => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  let { open, onClose, documentSetting, orderNosData } = props;
  const [selectedTemplateSize, setSelectedTemplateSize] = useState(null);
  const [documentSizeId, setDocumentSizeId] = useState(null);
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(false);

  
  const handleCarrierBill = async () => {
    if (!selectedTemplateSize) {
      errorNotification(
        LanguageReducer?.languageType?.ORDER_PLEASE_SELECT_TEMPLATES
      );
      return;
    }

    setisLoading(true);
    const selectedOrderNos = orderNosData;

    try {
      await downloadWayBillsByOrderNos(
        selectedOrderNos.join(","),
        documentSizeId
      );
      successNotification("Successfully Printed");
    } catch (error) {
      errorNotification("Failed to print. Please try again.");
    } finally {
      setisLoading(false);
      onClose();
    }
  };

  const handleClick = (event) => {
    navigate("/document-settings");
  };
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="lg"
        title={LanguageReducer?.languageType?.ORDER_PRINT_LABELS}
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.ORDER_PRINT}
            loading={isLoading}
            bg={purple}
            onClick={handleCarrierBill}
          />
        }
        component={"form"}
      >
        <GridContainer>
          {documentSetting?.result?.map((res, index) => (
            <GridItem xs={12} md={8} lg={8} key={index}>
              <Box display="flex" gap={2}>
                {res.documentTemplate?.map((data) => (
                  <Box
                    key={data.documentTemplateId}
                    width="100%"
                    position="relative"
                    mb={2}
                  >
                    <GreyBox
                      key={data.documentTemplateId}
                      bg={
                        selectedTemplateSize === data.documentTemplateId
                          ? "lightgrey"
                          : undefined
                      }
                      className="flex_center"
                      component={ButtonBase}
                      width="100%"
                      onClick={() => {
                        setSelectedTemplateSize(data.documentTemplateId);
                        setDocumentSizeId(data.documentTemplateId);
                      }}
                    >
                      <img
                        width="250px"
                        height="250px"
                        src={data.sampleImageUrl}
                        alt="Image Not Found"
                      />
                    </GreyBox>
                    <Typography className="flex_center" variant="h5" mt={1}>
                      {data.name}
                    </Typography>
                    <Button
                      onClick={() => window.open(data.samplePdfUrl, "_blank")}
                      className="eyeBtn"
                      style={{
                        position: "absolute",
                        top: "8px",
                        right: "8px",
                        minWidth: "auto",
                        padding: 0,
                      }}
                    >
                      <VisibilityIcon />
                    </Button>
                  </Box>
                ))}
              </Box>
            </GridItem>
          ))}
        </GridContainer>
        <Box display="flex" justifyContent="center">
          <Typography variant="h4">
            {LanguageReducer?.languageType?.ORDERS_NOTE}:
          </Typography>
          <Typography sx={{ marginTop: "4px", paddingLeft: "2px" }}>
            {
              LanguageReducer?.languageType
                ?.ORDER_IF_YOU_WANT_TO_USE_THE_DEFAULT_AWB_SETTING_THEN_PLEASE_CHANGE_THE_SETTING
            }
            <Link sx={{ cursor: "pointer" }} onClick={handleClick}>
              {" "}
              {
                LanguageReducer?.languageType
                  ?.ORDER_HERE_AND_UNCHECK_ASK_EVERY_TIME
              }
            </Link>
          </Typography>
        </Box>
      </ModalComponent>
    </>
  );
};

export default AddBillingModal;
