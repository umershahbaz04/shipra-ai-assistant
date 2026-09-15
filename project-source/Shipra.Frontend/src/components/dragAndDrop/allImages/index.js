
import { Typography } from "@mui/material";
import "./style.css";
import React, { useRef, useState } from "react";
import ImageIcon from '../../../assets/images/uploadImage.png'
import { useSelector } from "react-redux";


function DragDropAllFiles(props) {
    // drag state
    const [dragActive, setDragActive] = useState(false);
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
            // handleFiles(e.dataTransfer.files);
        }
    };

    // triggers when file is selected with click
    const handleChange = function (e) {
        e.preventDefault();
        if (e.target.files && e.target.files[0]) {
            // handleFiles(e.target.files);
        }
    };

    // triggers the input when the button is clicked
    const onButtonClick = () => {
        inputRef.current.click();
    };


    return (
        <form id="form-all-file-upload" onDragEnter={handleDrag} onSubmit={(e) => e.preventDefault()}>
            <input ref={inputRef} type="file" disabled={props?.disabled} id="input-all-file-upload" multiple={true} onChange={handleChange} />
            <label id="label-all-file-upload" htmlFor="input-all-file-upload" className={dragActive ? "drag-active" : ""}>
                <div>
                    <img style={{ height: '60px' }} src={ImageIcon} />
                    <Typography sx={{
                        color: 'grey',
                        fontSize: "30px !important",
                        textAlign: "center !important",
                        margin: '0px 60px 0px 60px'
                    }}>{LanguageReducer?.languageType?.DRAG_AND_DROP_TEXT}</Typography>
                </div>
            </label>
            {dragActive && <div id="drag-all-file-element" onDragEnter={handleDrag} onDragLeave={handleDrag} onDragOver={handleDrag} onDrop={handleDrop}></div>}
        </form>
    );
};
export default (DragDropAllFiles);
