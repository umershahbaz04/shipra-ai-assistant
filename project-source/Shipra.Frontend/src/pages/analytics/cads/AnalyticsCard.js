import { Box, Typography } from "@mui/material";
import React, { useLayoutEffect, useRef, useState } from "react";
function AnalyticsCard(props) {
  const { title, data, Icon, iconBgColor, height } = props;
  const imgRef = useRef();
  const [imgHeight, setImgHeight] = useState(0);

  useLayoutEffect(() => {
    setImgHeight(imgRef.current.offsetHeight - 10);
  }, [window.innerHeight, window.outerHeight]);

  return (
    <Box
      sx={{
        bgcolor: "#fff",
        borderRadius: { xs: "5px", md: "15px" },
        px: { xs: "5px", md: 1 },
        border: "1px solid #d3d3d394",
        height: height,
        display: "flex",
        alignItems: "center",
        gap: 1,
      }}
      ref={imgRef}
    >
      <Box
        sx={{
          width: imgHeight,
          height: imgHeight,
          bgcolor: iconBgColor,
          borderRadius: { xs: "5px", md: "10px" },
        }}
        flexShrink={0}
        className={"flex_center"}
      >
        <Box component={"img"} width={"100%"} height={"100%"} src={Icon} />
      </Box>
      <Box className={"flex_between"} flexWrap={"wrap"} width={"100%"}>
        <Typography variant="h5" fontSize={"1.5vh"} fontWeight={300}>
          {title}
        </Typography>

        <Typography variant="h6" fontSize={"1.5vh"}>
          {data.loading ? "loading..." : data.count}
        </Typography>
      </Box>
    </Box>
  );
}
export default React.memo(AnalyticsCard);
