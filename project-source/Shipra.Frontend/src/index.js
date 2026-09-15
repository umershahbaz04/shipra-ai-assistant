import "@fontsource/lato"; // Defaults to weight 400
import "@fontsource/lato/400-italic.css"; // Specify weight and style
import "@fontsource/lato/400.css"; // Specify weight
import { ThemeProvider, createTheme } from "@mui/material/styles";
import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import { styleSheet } from "./assets/styles/style";
import "./index.css";
import { store } from "./redux/store";
import reportWebVitals from "./reportWebVitals";
export const fontFamily = "'Lato Regular', 'Inter Regular', 'Arial' !important";
let theme = createTheme({
  overrides: {
    MuiFormLabel: {
      asterisk: {
        color: "#db3131", // Red color for required asterisk
      },
    },
    MuiTooltip: {
      styleOverrides: {
        tooltip: {
          fontSize: "12px", // Adjust the font size as needed
        },
      },
    },
    MuiTextField: {
      root: {
        "& .MuiOutlinedInput-root": {
          "& fieldset": {
            fontFamily: fontFamily, // Font for the outlined input border
          },
          "& input": {
            fontFamily: fontFamily, // Font for the input text
            fontSize: "14px", // Set the desired font size
          },
        },
        "& .MuiInputLabel-root": {
          fontFamily: fontFamily, // Font for the label
        },
      },
    },
    MuiAutocomplete: {
      root: {
        "& .MuiAutocomplete-inputRoot": {
          "& input": {
            fontFamily: fontFamily, // Font for the autocomplete input text
          },
        },
      },
    },
  },
  palette: {
    primary: {
      main: "#BE212F",
    },
    secondary: {
      main: "#1A1A1A",
    },
  },
  typography: {
    fontFamily: fontFamily,
    lineHeight: "1px",
    h2: { fontSize: 20, fontWeight: 600 },
    h3: { fontSize: 18, fontWeight: 600 },
    h4: { fontSize: 16, fontWeight: 600 },
    h5: { fontSize: 14, fontWeight: 600 },
    h6: { fontSize: 12, fontWeight: 300 },
    body1: { fontSize: 11, fontWeight: 300 },
  },
  components: {
    MuiOutlinedInput: {
      styleOverrides: {
        input: {
          fontSize: "14px",
          "& .MuiOutlinedInput-input": {
            fontSize: "14px", // Set the desired font size
          },
        },
      },
    },
    MuiFormHelperText: {
      //font size for error below input
      styleOverrides: {
        root: {
          fontSize: "10px", // Set the desired font size
        },
      },
    },
    MuiAutocomplete: {
      styleOverrides: {
        listbox: {
          ...styleSheet.autocompleteFontSize, // Set the font size for the listbox
        },
        option: {
          ...styleSheet.autocompleteFontSize, // Set the font size for the options
        },
      },
    },
  },
});
const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  // <React.StrictMode>
  <Provider store={store}>
    {" "}
    <ThemeProvider theme={theme}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </ThemeProvider>
  </Provider>
  // </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
