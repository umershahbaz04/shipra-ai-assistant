import {
  Timeline,
  TimelineConnector,
  TimelineContent,
  timelineContentClasses,
  TimelineDot,
  TimelineItem,
  TimelineOppositeContent,
  TimelineSeparator,
} from "@mui/lab";
import { Grid, Typography } from "@mui/material";
import React from "react";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import { useSelector } from "react-redux";

const PayoutHistoryModal = (props) => {
  const { open, onClose, payoutStatusHistory } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION_HISTORY}
      >
        <Grid container spacing={2}>
          <Grid item md={8}>
            <Typography variant="h3" fontWeight="600" m={2}>
              History
            </Typography>{" "}
            <Timeline
              sx={{
                [`& .${timelineContentClasses.root}`]: {
                  flex: 5,
                },
              }}
            >
              {payoutStatusHistory
                ?.filter(
                  (value) => value.StatusName !== null || value.Comment !== null
                )
                .map((value, index) => (
                  <TimelineItem key={index}>
                    <TimelineOppositeContent
                      color="textSecondary"
                      fontSize="14px"
                    >
                      <Typography variant="h5" fontWeight={400}>
                        {UtilityClass.convertUtcToLocal(value.CreatedOn)}
                      </Typography>
                    </TimelineOppositeContent>
                    <TimelineSeparator>
                      <TimelineDot sx={styleSheet.timelineDot} />
                      {index !== payoutStatusHistory.length - 1 && (
                        <TimelineConnector />
                      )}
                    </TimelineSeparator>
                    <TimelineContent color="#000">
                      <Typography variant="h5">
                        {value.StatusName && (
                          <>
                            <span style={{ fontWeight: 400 }}>
                              Status changed to
                            </span>{" "}
                            {value.StatusName}{" "}
                            <span style={{ fontWeight: 400 }}>by</span>{" "}
                            {value.CreatedBy}
                            <br />
                          </>
                        )}
                        {value?.Comment && (
                          <>
                            <span style={{ fontWeight: 400 }}>Comment:</span>{" "}
                            {value.Comment}{" "}
                            <span style={{ fontWeight: 400 }}>by</span>{" "}
                            {value.CreatedBy}
                          </>
                        )}
                      </Typography>
                    </TimelineContent>
                  </TimelineItem>
                ))}
            </Timeline>
          </Grid>
        </Grid>
      </ModalComponent>
    </>
  );
};

export default PayoutHistoryModal;
