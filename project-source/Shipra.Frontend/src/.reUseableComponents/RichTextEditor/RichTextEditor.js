import React, { useMemo } from "react";
import ReactQuill from "react-quill-new";
import "react-quill-new/dist/quill.snow.css";
import { Controller } from "react-hook-form";

const RichTextEditor = ({
  name,
  control,
  defaultValue = "",
  rules = {},
  required = false,
  requiredMessage = "Field Required",
}) => {
  const modules = useMemo(
    () => ({
      toolbar: [
        [{ font: [] }, { size: [] }],
        [{ header: [1, 2, 3, 4, 5, 6, false] }],
        ["bold", "italic", "underline", "strike"], // "blockquote", "code"
        [{ color: [] }, { background: [] }],
        //   [{ script: "sub" }, { script: "super" }],
        [{ list: "ordered" }, { list: "bullet" }],
        [{ indent: "-1" }, { indent: "+1" }],
        [{ align: [] }],
        //   ["link", "image", "video", "formula"],
        //   ["clean"],
      ],
    }),
    []
  );
  const formats = useMemo(
    () => [
      "header",
      "font",
      "size",
      "bold",
      "italic",
      "underline",
      "strike",
      "blockquote",
      "code",
      "color",
      "background",
      "script",
      "list",
      "bullet",
      "indent",
      // "align",
      // "link",
      // "image",
      // "video",
      // "formula",
    ],
    []
  );
  const defaultRules = required
    ? {
        validate: (value) => {
          const stripped = value.replace(/<(.|\n)*?>/g, "").trim();
          return stripped.length > 0 || requiredMessage;
        },
      }
    : {};

  return (
    <Controller
      name={name}
      control={control}
      defaultValue={defaultValue}
      rules={{ ...defaultRules, ...rules }}
      render={({ field, fieldState }) => (
        <div>
          <ReactQuill
            theme="snow"
            value={field.value}
            onChange={field.onChange}
            modules={modules}
            formats={formats}
            onBlur={field.onBlur}
          />
          {fieldState?.error && (
            <p style={{ color: "#d32f2f", marginTop: 4, paddingLeft: "14px" }}>
              {fieldState.error.message}
            </p>
          )}
        </div>
      )}
    />
  );
};

export default RichTextEditor;
