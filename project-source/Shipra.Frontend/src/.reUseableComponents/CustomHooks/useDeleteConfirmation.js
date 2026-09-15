import React, { useState } from "react";

const useDeleteConfirmation = () => {
  const [openDelete, setOpenDelete] = useState(false);
  const [deletedItem, setDeletedItem] = useState();
  const [loadingConfirmation, setLoadingConfirmation] = useState(false);

  const handleSetDeletedItem = (data) => {
    setDeletedItem(data);
    setOpenDelete(true);
  };
  const handleReset = () => {
    setDeletedItem();
    setOpenDelete(false);
  };
  return {
    openDelete,
    setOpenDelete,
    handleDeleteClose: handleReset,
    handleSetDeletedItem,
    deletedItem,
    loadingConfirmation,
    setLoadingConfirmation,
  };
};
export default useDeleteConfirmation;
