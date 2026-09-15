import { Box } from "@mui/material";
import React, { useState } from "react";
import { DragDropContext, Draggable, Droppable } from "react-beautiful-dnd";
import styled from "styled-components";
import { v4 as uuid } from "uuid";
import {
  DeleteIconButton,
  EditIconButton,
  GridContainer,
  GridItem,
  MenuComponent,
  PageMainBox,
  grey,
  greyBorder,
  purple,
  useForm,
  useMenu,
  useMenuForLoop,
} from "../../../utilities/helpers/Helpers";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent.js";
// a little function to help us with reordering the result
const reorder = (list, startIndex, endIndex) => {
  const result = Array.from(list);
  const [removed] = result.splice(startIndex, 1);
  result.splice(endIndex, 0, removed);

  return result;
};
/*
 * Moves an item from one list to another list.
 */
const copy = (source, destination, droppableSource, droppableDestination) => {
  const sourceClone = Array.from(source);
  const destClone = Array.from(destination);
  const item = sourceClone[droppableSource.index];

  destClone.splice(droppableDestination.index, 0, { ...item, id: uuid() });
  return destClone;
};

const move = (source, destination, droppableSource, droppableDestination) => {
  const sourceClone = Array.from(source);
  const destClone = Array.from(destination);
  const [removed] = sourceClone.splice(droppableSource.index, 1);

  destClone.splice(droppableDestination.index, 0, removed);

  const result = {};
  result[droppableSource.droppableId] = sourceClone;
  result[droppableDestination.droppableId] = destClone;

  return result;
};

const grid = 8;

const Item = styled.div`
  display: flex;
  justify-content: space-between;
  user-select: none;
  padding: 0.5rem;
  margin: 0 0 0.5rem 0;
  align-items: flex-start;
  align-content: flex-start;
  line-height: 1.5;
  border-radius: 10px;
  background: #fff;
  border: 1px ${(props) => (props.isDragging ? "dashed #000" : "solid #ddd")};
  &:hover {
    button {
      opacity: 1;
    }
  }
`;

const Clone = styled(Item)`
  ~ div {
    transform: none !important;
  }
`;

const Handle = styled.div`
  display: flex;
  align-items: center;
  align-content: center;
  user-select: none;
  margin: -0.5rem 0.5rem -0.5rem -0.5rem;
  padding: 0.5rem;
  line-height: 1.5;
  border-radius: 3px 0 0 3px;
  background: #fff;
  border-right: 1px solid #ddd;
  color: #000;
  border-top-left-radius: 10px;
  border-bottom-left-radius: 10px;
`;

const Content = styled.div`
  display: flex;
  flex-direction: column;
  gap: 10px;
`;
const section = styled.div`
  padding: 10px;
  background-color: ${grey};
  border: ${greyBorder};
  // border: 1px
  //   ${(props) => (props.isDraggingOver ? "dashed #000" : "solid #ddd")};
  // background: #fff;
  // padding: 0.5rem 0.5rem 0;
  // border-radius: 10px;
  // flex: 0 0 150px;
  // font-family: fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",;
`;

const TextFieldBox = styled(section)`
  position: sticky;
  top: 74px;
  border-radius: 10px;
`;

const Container = styled.fieldset`
  border: 2px solid ${purple};
  border-radius: 20px;
  width: 100%;
`;
const ContainerLabel = styled.legend`
  color: ${purple};
  font-size: 16;
  font-family: "Lato Regular", "Inter Regular", "Arial" !important;
  font-weight: 600;
`;

const Notice = styled.div`
  display: flex;
  align-items: center;
  align-content: center;
  justify-content: center;
  padding: 0.5rem;
  margin: 0 0.5rem 0.5rem;
  border: 1px solid transparent;
  line-height: 1.5;
  color: #aaa;
`;

const Button = styled.button`
  display: flex;
  align-items: center;
  align-content: center;
  justify-content: center;
  margin: 0.5rem;
  padding: 0.5rem;
  color: #000;
  border: 1px solid #ddd;
  background: #fff;
  border-radius: 3px;
  font-size: 1rem;
  cursor: pointer;
`;

const ButtonText = styled.div`
  margin: 0 1rem;
`;

const ITEMS = [
  {
    id: uuid(),
    content: "Text",
  },
  {
    id: uuid(),
    content: "Number",
  },
  {
    id: uuid(),
    content: "Email",
  },
  {
    id: uuid(),
    content: "Phone",
  },
];

export default function MetaFields() {
  const [state, setState] = useState({
    sections: {
      [uuid()]: [],
    },
  });

  const [selectedTextField, setSelectedTextField] = useState({
    label: "",
  });

  const onDragEnd = (result) => {
    const { source, destination } = result;

    // dropped outside the list
    if (!destination) {
      return;
    }

    switch (source.droppableId) {
      case destination.droppableId:
        setState((prev) => ({
          sections: {
            ...prev.sections,
            [destination.droppableId]: reorder(
              prev.sections[source.droppableId],
              source.index,
              destination.index
            ),
          },
        }));
        break;
      case "ITEMS":
        setState((prev) => ({
          sections: {
            ...prev.sections,
            [destination.droppableId]: copy(
              ITEMS,
              prev.sections[destination.droppableId] || [],
              source,
              destination
            ),
          },
        }));
        break;
      default:
        setState((prev) => ({
          sections: {
            ...prev.sections,
            ...move(
              prev.sections[source.droppableId],
              prev.sections[destination.droppableId] || [],
              source,
              destination
            ),
          },
        }));
        break;
    }
  };

  const handleAddNewSection = () => {
    const newsectionId = uuid();
    setState((prev) => ({
      sections: {
        ...prev.sections,
        [newsectionId]: [],
      },
    }));
  };

  const { anchorEl, openElem, handleOpen, handleClose } = useMenuForLoop();
  const { errors, handleInvalid, handleChange, setErrors } =
    useForm(setSelectedTextField);

  return (
    <PageMainBox>
      <DragDropContext onDragEnd={onDragEnd}>
        <GridContainer spacing={1}>
          <GridItem xs={4}>
            <Droppable droppableId="ITEMS" isDropDisabled={true}>
              {(provided, snapshot) => (
                <TextFieldBox
                  ref={provided.innerRef}
                  isDraggingOver={snapshot.isDraggingOver}
                >
                  {ITEMS.map((item, index) => (
                    <Draggable
                      key={item.id}
                      draggableId={item.id}
                      index={index}
                    >
                      {(provided, snapshot) => (
                        <>
                          <Item
                            ref={provided.innerRef}
                            {...provided.draggableProps}
                            {...provided.dragHandleProps}
                            isDragging={snapshot.isDragging}
                            style={provided.draggableProps.style}
                          >
                            {item.content}
                          </Item>
                          {snapshot.isDragging && <Clone>{item.content}</Clone>}
                        </>
                      )}
                    </Draggable>
                  ))}
                  {/* <ButtonComponent
                    onClick={handleAddNewSection}
                    title={"New Section"}
                  /> */}
                </TextFieldBox>
              )}
            </Droppable>
          </GridItem>
          <GridItem xs={8}>
            <Content>
              {Object.entries(state.sections).map(([sectionId, list], i) => (
                <Droppable key={sectionId} droppableId={sectionId}>
                  {(provided, snapshot) => (
                    <Box sx={{ display: "flex", gap: 0.25 }}>
                      <Container
                        ref={provided.innerRef}
                        isDraggingOver={snapshot.isDraggingOver}
                      >
                        <ContainerLabel>Customer</ContainerLabel>
                        {list.map((item, index) => (
                          <Draggable
                            key={item.id}
                            draggableId={item.id}
                            index={index}
                          >
                            {(provided, snapshot) => (
                              <Item
                                ref={provided.innerRef}
                                {...provided.draggableProps}
                                isDragging={snapshot.isDragging}
                                style={provided.draggableProps.style}
                              >
                                <Box className={"flex_between"}>
                                  <Handle {...provided.dragHandleProps}>
                                    <svg
                                      width="24"
                                      height="24"
                                      viewBox="0 0 24 24"
                                    >
                                      <path
                                        fill="currentColor"
                                        d="M3,15H21V13H3V15M3,19H21V17H3V19M3,11H21V9H3V11M3,5V7H21V5H3Z"
                                      />
                                    </svg>
                                  </Handle>
                                  <Box className={"flex_center"}>
                                    {item.content}

                                    <EditIconButton
                                      sx={{
                                        opacity: 0,
                                      }}
                                      onClick={(e) => {
                                        handleOpen(e, item.id);
                                        setSelectedTextField((prev) => ({
                                          ...prev,
                                          label: item.content,
                                        }));
                                      }}
                                    />
                                  </Box>
                                  <MenuComponent
                                    open={openElem === item.id}
                                    anchorEl={anchorEl}
                                    onClose={() => {
                                      handleClose();

                                      setErrors([]);
                                    }}
                                  >
                                    <Box
                                      sx={{ bgcolor: "#fff", px: 1.25, gap: 1 }}
                                      className={"flex_col"}
                                      component={"form"}
                                      onInvalid={handleInvalid}
                                      onSubmit={(e) => {
                                        e.preventDefault();
                                        setState((prev) => {
                                          const _sections = {
                                            ...prev.sections,
                                          };
                                          const selectedSection =
                                            _sections[sectionId];
                                          selectedSection[index].content =
                                            selectedTextField.label;
                                          return {
                                            ...prev,
                                            sections: {
                                              ...prev.sections,
                                              [sectionId]: selectedSection,
                                            },
                                          };
                                        });
                                        handleClose();
                                        console.log(sectionId, list, state);
                                      }}
                                    >
                                      <TextFieldComponent
                                        value={selectedTextField.label}
                                        name={"label"}
                                        onChange={handleChange}
                                        errors={errors}
                                      />
                                      <Box textAlign={"end"}>
                                        <ButtonComponent
                                          title={"Save"}
                                          type={"submit"}
                                        />
                                      </Box>
                                    </Box>
                                  </MenuComponent>
                                </Box>
                                <DeleteIconButton
                                  sx={{
                                    opacity: 0,
                                  }}
                                  onClick={() => {
                                    setState((prev) => {
                                      const _sections = { ...prev.sections };
                                      const selectedSection =
                                        _sections[sectionId];
                                      const filteredSectionList =
                                        selectedSection.filter(
                                          (_, _index) => _index !== index
                                        );
                                      return {
                                        ...prev,
                                        sections: {
                                          ...prev.sections,
                                          [sectionId]: filteredSectionList,
                                        },
                                      };
                                    });
                                    console.log(sectionId, list, state);
                                  }}
                                />
                              </Item>
                            )}
                          </Draggable>
                        ))}
                        {provided.placeholder}
                      </Container>
                      {/* <Box mt={"7px"}>
                        <DeleteButton
                          onClick={() => {
                            setState((prev) => {
                              const _sections = { ...prev.sections };
                              delete _sections[sectionId];
                              return { ...prev, sections: _sections };
                            });
                            console.log(sectionId, list, state);
                          }}
                        />
                      </Box> */}
                    </Box>
                  )}
                </Droppable>
              ))}
            </Content>
          </GridItem>
        </GridContainer>
      </DragDropContext>
    </PageMainBox>
  );
}
