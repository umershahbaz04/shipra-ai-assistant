import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
    GridContainer,
    GridItem,
    useForm,
} from "../../../../utilities/helpers/Helpers";

export default function UpdateMetafieldModal(props) {
  const { open, onClose, data } = props;
  const [submitData, setSubmitData] = useState({
    name: "",
    description: "",
  });

  const { errors, handleInvalid, handleChange, setErrors } =
    useForm(setSubmitData);


  const handleUpdateMetafield = (e) => {
    e.preventDefault();
  };

  useEffect(() => {
    setSubmitData((prev) => ({
      ...prev,
      name: data.name,
      description: data.description,
    }));
  }, [open]);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={() => {
          onClose();
          setErrors([]);
        }}
        title={"Metafield"}
        maxWidth={"sm"}
        component={"form"}
        onInvalid={handleInvalid}
        onSubmit={handleUpdateMetafield}
        actionBtn={<ModalButtonComponent title={"Update"} type={"submit"} />}
      >
        <GridContainer>
          <GridItem xs={12}>
            <TextFieldLableComponent title={"Name"} />
            <TextFieldComponent
              name={`name`}
              value={submitData.name}
              onChange={handleChange}
              errors={errors}
            />
          </GridItem>
          <GridItem xs={12}>
            <TextFieldLableComponent title={"Description"} required={false} />
            <TextFieldComponent
              name={`description`}
              value={submitData.description}
              onChange={handleChange}
              required={false}
            />
          </GridItem>
        </GridContainer>
      </ModalComponent>
      
    </>
  );
}
