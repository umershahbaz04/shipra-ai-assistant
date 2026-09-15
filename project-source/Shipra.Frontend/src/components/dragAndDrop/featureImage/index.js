import { Typography } from "@mui/material";
import "./style.css";
import React, { useEffect, useRef, useState } from "react";
import ImageIcon from "../../../assets/images/uploadImage.png";
import { useSelector } from "react-redux";
import { UploadProductFile } from "../../../api/AxiosInterceptors";
import { successNotification } from "../../../utilities/toast";

function DragDropFile(props) {
  // drag state
  const [dragActive, setDragActive] = useState(false);
  const [file, setFile] = useState();
  useEffect(() => {
    if (props?.imageURL) {
      setFile(props.imageURL);
    }
  }, [props]);
  // ref
  const inputRef = useRef(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  // handle drag events
  const handleDrag = function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  // triggers when file is dropped
  const handleDrop = function (e) {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      let file = e.dataTransfer.files[0];
      let fileName = file.name.replace(/\s+/g, "_");
      const renamedFile = new File([file], fileName, { type: file.type });
      setFile(renamedFile);

      const formData = new FormData();
      formData.append("File", e.target.files[0]);
      UploadProductFile(formData)
        .then((res) => {
          console.log("res", res);
          props.setProductFile(res.data.result.url);
          successNotification(res.data.result.message);
        })
        .catch((e) => console.log("e", e));
    }
  };

  // triggers when file is selected with click
  const handleChange = function (e) {
    e.preventDefault();
    if (e.target.files && e.target.files[0]) {
      let file = e.target.files[0];
      let fileName = file.name.replace(/\s+/g, "_"); 
      const renamedFile = new File([file], fileName, { type: file.type });
      setFile(renamedFile);
      const formData = new FormData();
      formData.append("File", e.target.files[0]);
      UploadProductFile(formData)
        .then((res) => {
          console.log("res", res);
          props.setProductFile(res.data.result.url);
          // successNotification(res.data.result.message);
        })
        .catch((e) => console.log("e", e));
    }
  };
  const isUrl = (string) => {
    try {
      new URL(string);
      return true;
    } catch (_) {
      return false;
    }
  };

  return (
    <form
      id="form-file-upload"
      onDragEnter={handleDrag}
      onSubmit={(e) => e.preventDefault()}
    >
      <input
        ref={inputRef}
        type="file"
        id="input-file-upload"
        disabled={props?.disabled}
        multiple={true}
        onChange={handleChange}
      />
      <label
        id="label-file-upload"
        htmlFor="input-file-upload"
        className={dragActive ? "drag-active" : ""}
      >
        <div>
          {file ? (
            <img
              style={{ height: "150px" }}
              src={isUrl(file) ? file : URL.createObjectURL(file)}
            />
          ) : (
            <>
              <img style={{ height: "60px" }} src={ImageIcon} />
              <Typography
                sx={{
                  color: "grey",
                  fontSize: "30px !important",
                  textAlign: "center !important",
                }}
              >
                {LanguageReducer?.languageType?.FEATURE_IMAGE_TEXT}
              </Typography>
            </>
          )}
        </div>
      </label>
      {dragActive && (
        <div
          id="drag-file-element"
          onDragEnter={handleDrag}
          onDragLeave={handleDrag}
          onDragOver={handleDrag}
          onDrop={handleDrop}
        ></div>
      )}
    </form>
  );
}
export default DragDropFile;
