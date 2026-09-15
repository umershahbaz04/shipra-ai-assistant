
/* ------------------------------  SET LOCAL STORAGE DATA FUNCTION  -------------------------------- */
export const setLocalStorageToken = (data) => {
  localStorage.setItem('local_token', JSON.stringify(data));
};

/* ------------------------------  GET LOCAL STORAGE DATA FUNCTION  -------------------------------- */
export const getLocalStorageToken = () => {
	return JSON.parse(localStorage.getItem('local_token'));
};
