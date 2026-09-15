import React, { useEffect, useState } from "react";
import { styleSheet } from "../../assets/styles/style";
import { Box, Typography } from "@mui/material";
import PictureAsPdfOutlinedIcon from "@mui/icons-material/PictureAsPdfOutlined";
const FileViewver = (props) => {
  const { file } = props;
  const [fileView, setFileView] = useState("");
  function getFileExtension(filename) {
    const fileExtension = filename.slice(
      ((filename.lastIndexOf(".") - 1) >>> 0) + 2
    );
    const fileName = filename.slice(filename.lastIndexOf("/") + 1);
    setFileView({ extension: fileExtension.toLowerCase(), name: fileName });
  }

  useEffect(() => {
    console.log({ file });

    getFileExtension(file.filePath);
  }, []);
  const handleClick = (url) => {
    window.open(url, "_blank");
  };

  return (
    <Box
      sx={styleSheet.podFilesImgBox}
      onClick={() => handleClick(file.filePath)}
    >
      {fileView.extension == "pdf" ? (
        <>
          <PictureAsPdfOutlinedIcon style={styleSheet.podFilesIcon} />
          <Typography variant="h4">{fileView.name}</Typography>
        </>
      ) : (
        <img
          src={file.filePath}
          style={styleSheet.podFilesImg}
          onClick={() => handleClick(file.filePath)}
        />
      )}
    </Box>
  );
};

export default FileViewver;
