import { Box, Grid } from "@mui/material";
import React from "react";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import { greyBorder } from "../../../../utilities/helpers/Helpers";
import { useSelector } from "react-redux";

const OpenPayoutFileModal = (props) => {
  const { payoutFileData, open, onClose } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClick = (filePath) => {
    window.open(filePath, "_blank");
  };

  return (
    <Box>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={LanguageReducer?.languageType?.ORDERS_WALLET_PAYOUT_FILE}
      >
        <Grid container spacing={2}>
          {payoutFileData?.map((file) => {
            const isPdf = file.filePath.endsWith(".pdf");

            return (
              <Grid item md={4} key={file.payoutId}>
                <div
                  onClick={() => handleClick(file.filePath)}
                  style={{
                    width: "100%",
                    height: "100%",
                    cursor: "pointer",
                    border: greyBorder,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    padding: "4px",
                  }}
                >
                  {isPdf ? (
                    <iframe
                      src={file.filePath}
                      style={{
                        width: "100%",
                        height: "100%",
                        border: "none",
                        pointerEvents: "none",
                      }}
                      title={`PDF ${file.payoutId}`}
                    />
                  ) : (
                    <img
                      src={file.filePath}
                      alt="Payout File"
                      style={{
                        maxWidth: "100%",
                        maxHeight: "100%",
                        objectFit: "contain",
                      }}
                    />
                  )}
                </div>
              </Grid>
            );
          })}
        </Grid>
      </ModalComponent>
    </Box>
  );
};

export default OpenPayoutFileModal;
