import React from "react";
import moment from "moment";
import "moment-timezone";
import CryptoJS from "crypto-js";
import { errorNotification, successNotification } from "./toast";
import { getThisKeyCookie } from "./cookies";
import { EnumCookieKeys } from "./enum";

class UtilityClass {
  static convertUtcToLocal(utcTime) {
    if (utcTime) {
      let timezone = getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        ? getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        : moment.tz.guess();
      const localTime = moment.utc(utcTime).tz(timezone);
      return localTime.format("D MMMM YYYY hh:mm:ss a");
    } else {
      return "";
    }
  }
  static convertUtcToLocalAndGetTime(utcTime) {
    if (utcTime) {
      let timezone = getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        ? getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        : moment.tz.guess();
      const localTime = moment.utc(utcTime).tz(timezone);
      return localTime.format("hh:mm:ss a");
    } else {
      return "";
    }
  }
  static toLocaleDateStringFormate() {
    return "en-IR";
  }
  static getFormatedDateWithoutTime = (date) => {
    const formattedDate = date?.toLocaleDateString(
      this.toLocaleDateStringFormate()
    );
    return formattedDate;
  };
  //not sure abbout time
  static getFormatedDateWithConfusion = (date) => {
    return date;
  };
  static convertUtcToLocalAndGetDate(utcTime) {
    if (utcTime) {
      let timezone = getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        ? getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        : moment.tz.guess();
      const localTime = moment.utc(utcTime).tz(timezone);
      return localTime.format("D MMMM YYYY");
    } else {
      return "";
    }
  }
  static convertUtcToLocalWithDashedDate(utcTime) {
    if (utcTime) {
      let timezone = getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        ? getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        : moment.tz.guess();
      const localTime = moment.utc(utcTime).tz(timezone);
      return localTime.format("DD-MM-YYYY hh:mm:ss a");
    } else {
      return "";
    }
  }
  static convertUtcToLocalAndGetShortDate(utcTime) {
    if (utcTime) {
      let timezone = getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        ? getThisKeyCookie(EnumCookieKeys.TIME_ZONE)
        : moment.tz.guess();
      const localTime = moment.utc(utcTime).tz(timezone);
      return localTime.format("D-M-YYYY");
    } else {
      return "";
    }
  }
  static getTwoCharacters(inputString) {
    if (inputString.includes(" ")) {
      // If the input string contains a space
      // We will extract the first two characters from the input string
      return inputString.substring(0, 2);
    } else {
      // If the input string does not contain a space
      // We will simply return the first two characters of the input string
      return inputString.substring(0, 2);
    }
  }
  static copyToClipboard(text) {
    if (text && text != "") {
      navigator.clipboard.writeText(text);
      successNotification("Text copied to clipboard ");
    }
  }
  static downloadPdf(data, fileName) {
    if (!fileName) {
      fileName = this.UUID();
    }
    const pdfBlob = new Blob([data], { type: "application/pdf" });

    // Create a URL for the Blob
    const pdfUrl = URL.createObjectURL(pdfBlob);

    // Create an anchor element to trigger the download
    const downloadLink = document.createElement("a");
    downloadLink.href = pdfUrl;
    downloadLink.download = fileName; // Set the desired filename

    // Programmatically click the anchor element to start the download
    downloadLink.click();

    // Clean up by revoking the URL object after the download has started
    URL.revokeObjectURL(pdfUrl);
  }
  static downloadJson(data, fileName) {
    if (!fileName) {
      fileName = this.UUID();
    }
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `${fileName}.json`);
    document.body.appendChild(link);
    link.click();
  }
  static downloadExcel(data, fileName) {
    if (!fileName) {
      fileName = this.UUID();
    }
    const url = window.URL.createObjectURL(new Blob([data]));
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", `${fileName}.xlsx`);
    document.body.appendChild(link);
    link.click();
  }
  static UUID() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, (c) =>
      (
        c ^
        (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))
      ).toString(16)
    );
  }
  static GetPaymentRef() {
    const uniqueIntegers = Math.floor(Math.random() * 1000000000);
    return uniqueIntegers;
  }
  static encryptData = (text) => {
    const data = CryptoJS.AES.encrypt(
      JSON.stringify(text),
      process.env.REACT_APP_SecretPassKey
    ).toString();
    return data;
  };
  static decryptData = (text) => {
    const bytes = CryptoJS.AES.decrypt(
      text,
      process.env.REACT_APP_SecretPassKey
    );
    const data = JSON.parse(bytes.toString(CryptoJS.enc.Utf8));
    return data;
  };
  static getChipDataFromTrackingArr = function (data) {
    let arr = [];
    if (data) {
      data.forEach((elm, i) => {
        arr.push({
          key: i,
          label: elm,
        });
      });
    }
    return arr;
  };
  static showErrorNotificationWithDictionary = function (data) {
    for (const key in data) {
      if (data.hasOwnProperty(key)) {
        const errorMessages = data[key];
        console.log(`Key: ${key}`);
        for (const errorMessage of errorMessages) {
          errorNotification(`${errorMessage}`);
        }
      }
    }
  };

  static updateDictionaryObjectValue = function (
    inputConfig,
    replacementConfig
  ) {
    // const inputConfig = `{
    //   "publicKey": "e.g. 1a2b3c4d5e6f7g8",
    //   "secretKey": "e.g. 1a2b3c4d5e6f7g8"
    // }`;

    // const replacementConfig = `{
    //   "publicKey": "4 5v43",
    //   "secretKey": "4543"
    // }`;
    // Parse the JSON strings into JavaScript objects
    const inputObject = JSON.parse(inputConfig);
    const replacementObject = JSON.parse(replacementConfig);

    // Iterate through the replacement object
    for (const key in replacementObject) {
      if (replacementObject.hasOwnProperty(key)) {
        const lowerCaseKey = key.toLowerCase();
        for (const inputKey in inputObject) {
          if (inputObject.hasOwnProperty(inputKey)) {
            if (inputKey.toLowerCase() === lowerCaseKey) {
              inputObject[inputKey] = replacementObject[key];
            }
          }
        }
      }
    }

    // Convert the updated object back to a JSON string
    const updatedConfig = JSON.stringify(inputObject, null, 2);

    return updatedConfig;
  };

  static convertArrIntoCommaSeperatedIds = function (data, propertyName) {
    // Check if data is an array
    if (!Array.isArray(data)) {
      return "";
    }
    let arr = [];
    for (var i = 0; i < data.length; i++) {
      arr.push(data[i][propertyName]);
    }
    const commaSeparatedIds = arr.join(",");
    return commaSeparatedIds;
  };
  static todayDate = function () {
    return new Date();
  };
  static getFormatedNumber = function (number) {
    if (number) {
      if (number.includes("+")) {
        number = number.replace("+", "");
      }
    } else {
      number = "";
    }
    return number;
  };
  static randomId = function () {
    const uint32 = window.crypto.getRandomValues(new Uint32Array(1))[0];
    return uint32.toString(16);
  };
  static sumOfProperty = (array, property) => {
    // Check if the array is not empty
    if (array.length === 0) {
      return 0;
    }

    // Use reduce to sum the specified property
    const total = array.reduce((accumulator, currentValue) => {
      return accumulator + currentValue[property];
    }, 0);

    return total;
  };
  static filterAndSum = (
    dataArray,
    filterProperty,
    filterValue,
    sumProperty
  ) => {
    if (dataArray.length === 0) {
      return 0;
    }
    // Filter the array based on the specified property and value
    const filteredData = dataArray?.filter(
      (item) => item[filterProperty] === filterValue
    );

    // Calculate the sum of the specified property in the filtered data
    const sum = filteredData?.reduce(
      (accumulator, currentItem) => accumulator + currentItem[sumProperty],
      0
    );

    return sum;
  };
  static getCommaSeparatedArrayWithFiltered = (
    dataArray,
    filterProperty,
    filterValue
  ) => {
    // Filter the array based on the category parameter
    const filteredData = dataArray?.filter(
      (item) => item[filterProperty] === filterValue
    );

    // Map the values to create an array of values
    const valuesArray = filteredData?.map((item) => item.value);

    // Join the array values into a comma-separated string
    const resultString = valuesArray.join(", ");

    return resultString;
  };
  static getDefaultCountryCode = () => {
    return "+971";
  };
  static getNumberfromBool = (value) => {
    return value ? 1 : 0;
  };
  
  static toPascalCase(str) {
        return str
            .replace(/(^\w|_\w)/g, match => match.replace("_", "").toUpperCase());
    }

    static convertKeysToPascalCase(obj) {
        if (Array.isArray(obj)) {
            return obj.map(item => UtilityClass.convertKeysToPascalCase(item));
        }

        if (obj !== null && typeof obj === "object") {
            return Object.fromEntries(
                Object.entries(obj).map(([key, value]) => [
                    UtilityClass.toPascalCase(key),
                    UtilityClass.convertKeysToPascalCase(value)
                ])
            );
        }

        return obj;
    }
}

export default UtilityClass;
