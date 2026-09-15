import ImageList from "@mui/material/ImageList";
import ImageListItem from "@mui/material/ImageListItem";
import IconButton from "@mui/material/IconButton";
import CloseIcon from "@mui/icons-material/Close";
import AddIcon from "@mui/icons-material/Add";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";
import { useEffect, useRef, useState } from "react";
import { Box } from "@material-ui/core";

const ImageListComponent = ({
  itemData,
  cols = 3,
  rowHeight = 160,
  handleRemoveImage,
  onUploadClick = () => {},
  onChange,
  onReorder,
  multiple,
  height = "325px",
}) => {
  const fileInputRef = useRef(null);
  const [images, setImages] = useState(Object.values(itemData));

  useEffect(() => {
    setImages(Object.values(itemData));
  }, [itemData]);

  const handleClick = () => {
    if (onChange && fileInputRef.current) {
      fileInputRef.current.click();
    } else if (onUploadClick) {
      onUploadClick();
    }
  };

  const handleDragEnd = (result) => {
    if (!result.destination) return;

    const reordered = Array.from(images);
    const [moved] = reordered.splice(result.source.index, 1);
    reordered.splice(result.destination.index, 0, moved);

    setImages(reordered);
    onReorder?.(reordered);
  };

  return (
    <DragDropContext onDragEnd={handleDragEnd}>
      <Droppable droppableId="image-grid" direction="vertical">
        {(provided) => (
          <Box
            ref={provided.innerRef}
            {...provided.droppableProps}
            height={height}
            sx={{ overflow: "auto" }}
          >
            <ImageList gap={1} cols={cols} rowHeight={rowHeight}>
              {images.map((item, index) => (
                <Draggable
                  key={index}
                  draggableId={String(index)}
                  index={index}
                >
                  {(provided) => (
                    <ImageListItem
                      ref={provided.innerRef}
                      {...provided.draggableProps}
                      {...provided.dragHandleProps}
                      sx={{
                        position: "relative",
                        width: "100%",
                        height: "100%",
                      }}
                    >
                      <img
                        src={item.imageUrl}
                        alt={item.title}
                        loading="lazy"
                        style={{
                          width: "100%",
                          height: "100%",
                          objectFit: "cover",
                          borderRadius: "8px",
                        }}
                      />
                      <IconButton
                        sx={{
                          position: "absolute",
                          top: 5,
                          right: 5,
                          backgroundColor: "rgba(255, 255, 255, 0.8)",
                          "&:hover": {
                            backgroundColor: "rgba(255, 255, 255, 1)",
                          },
                        }}
                        onClick={() => handleRemoveImage(index)}
                      >
                        <CloseIcon />
                      </IconButton>
                    </ImageListItem>
                  )}
                </Draggable>
              ))}
              {provided.placeholder}

              {/* Upload Button */}
              <ImageListItem
                onClick={handleClick}
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  backgroundColor: "#F8F8F8",
                  border: "2px dashed #ccc",
                  cursor: "pointer",
                  borderRadius: "8px",
                  height: `${rowHeight}px`,
                }}
              >
                <AddIcon sx={{ fontSize: 40, color: "#888" }} />
                <input
                  type="file"
                  ref={fileInputRef}
                  onChange={onChange}
                  accept="image/jpeg, image/png, image/gif, image/heic, image/webp"
                  style={{ display: "none" }}
                  multiple={multiple}
                />
              </ImageListItem>
            </ImageList>
          </Box>
        )}
      </Droppable>
    </DragDropContext>
  );
};

export default ImageListComponent;
