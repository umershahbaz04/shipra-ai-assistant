import { setThisKeyCookie } from "./index";


const setUserCredential = (encrypedData) => {
    setThisKeyCookie("U2FsdGVkX1", encrypedData); 
  };

  export {setUserCredential}