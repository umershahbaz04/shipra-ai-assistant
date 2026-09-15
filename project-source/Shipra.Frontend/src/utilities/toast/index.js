import { toast, Flip } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export const errorNotification = (message = 'Api call failed') => {
  console.log('error', message);
  const msg = message;
  toast.error(msg, {
    position: 'bottom-right',
    autoClose: 2000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
    transition: Flip

  });
};

export const successNotification = (message = 'Api call succeeded') => {
  const msg = message;
  toast.success(msg, {
    position: 'top-right',
    autoClose: 2000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
    transition: Flip
  });
};

export const warningNotification = (message = 'Api call warning') => {
  const msg = message;
  toast.warning(msg, {
    position: 'top-right',
    autoClose: 2000,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
    transition: Flip
  });
};