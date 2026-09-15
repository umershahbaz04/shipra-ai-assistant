import { useDispatch, useSelector } from "react-redux";
import { handleDispatIsApiFilterModelFlag } from "./Helpers";
import { useState, useEffect } from "react";
import { Box, Pagination, Typography } from "@mui/material";
import { isShowCountryInTabbarFlag } from "../cookies";

export const PaginationComponent = ({
  color = "primary",
  dataCount = 0,
  paginationMethodUrl = "",
  paginationChangeMethod = async () => {},
  defaultRowsPerPage,
  setLoading = () => {},
  name,
  sx,
  currentPage,
}) => {
  const dispatch = useDispatch();
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry
  );
  const [page, setPage] = useState(1);

  useEffect(() => {
    if (!isShowCountryInTabbarFlag()) return;
    if (paginationMethodUrl) {
      handleDispatIsApiFilterModelFlag(dispatch, {
        apiName: paginationMethodUrl,
        page: 0,
        pageSize: defaultRowsPerPage,
      });
    }
    setPage(1);
  }, [reduxSelectedCountry]);

  useEffect(() => {
    if (currentPage !== undefined) {
      setPage(currentPage + 1);
    }
  }, [currentPage]);

  const handleChange = async (event, newPage) => {
    setPage(newPage);
    setLoading(true);
    handleDispatIsApiFilterModelFlag(dispatch, {
      apiName: paginationMethodUrl,
      page: newPage - 1,
      pageSize: defaultRowsPerPage,
    });

    await paginationChangeMethod(newPage - 1, defaultRowsPerPage);
    setLoading(false);
  };

  const count = Math.ceil(dataCount / defaultRowsPerPage);
  return (
    <Box
      className={"flex_between"}
      mt={2}
      bgcolor={"white"}
      border={"1px solid"}
      borderColor={"rgba(224, 224, 224, 1)"}
      py={1}
      px={2}
      width={"100%"}
      sx={sx}
    >
      <Typography variant="h">
        Total {name}: {dataCount}
      </Typography>
      <Pagination
        count={count}
        color={color}
        onChange={handleChange}
        page={page}
      />
    </Box>
  );
};
