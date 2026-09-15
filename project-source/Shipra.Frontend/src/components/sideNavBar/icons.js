import React from "react";
import AssignmentIcon from "@mui/icons-material/Assignment";
import SummarizeOutlinedIcon from "@mui/icons-material/SummarizeOutlined";

export const AnalyticsIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M9 22H15C20 22 22 20 22 15V9C22 4 20 2 15 2H9C4 2 2 4 2 9V15C2 20 4 22 9 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M15.5 18.5C16.6 18.5 17.5 17.6 17.5 16.5V7.5C17.5 6.4 16.6 5.5 15.5 5.5C14.4 5.5 13.5 6.4 13.5 7.5V16.5C13.5 17.6 14.39 18.5 15.5 18.5Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8.5 18.5C9.6 18.5 10.5 17.6 10.5 16.5V13C10.5 11.9 9.6 11 8.5 11C7.4 11 6.5 11.9 6.5 13V16.5C6.5 17.6 7.39 18.5 8.5 18.5Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const StoreIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M3.00977 11.2202V15.7102C3.00977 20.2002 4.80977 22.0002 9.29977 22.0002H14.6898C19.1798 22.0002 20.9798 20.2002 20.9798 15.7102V11.2202"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M12 12C13.83 12 15.18 10.51 15 8.68L14.34 2H9.66999L8.99999 8.68C8.81999 10.51 10.17 12 12 12Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M18.3098 12C20.3298 12 21.8098 10.36 21.6098 8.35L21.3298 5.6C20.9698 3 19.9698 2 17.3498 2H14.2998L14.9998 9.01C15.1698 10.66 16.6598 12 18.3098 12Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M5.63988 12C7.28988 12 8.77988 10.66 8.93988 9.01L9.15988 6.8L9.63988 2H6.58988C3.96988 2 2.96988 3 2.60988 5.6L2.33988 8.35C2.13988 10.36 3.61988 12 5.63988 12Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M12 17C10.33 17 9.5 17.83 9.5 19.5V22H14.5V19.5C14.5 17.83 13.67 17 12 17Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const ProductIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M3.16992 7.43994L11.9999 12.5499L20.7699 7.46991"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M12 21.61V12.54"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M9.93011 2.48L4.59012 5.45003C3.38012 6.12003 2.39014 7.80001 2.39014 9.18001V14.83C2.39014 16.21 3.38012 17.89 4.59012 18.56L9.93011 21.53C11.0701 22.16 12.9401 22.16 14.0801 21.53L19.4201 18.56C20.6301 17.89 21.6201 16.21 21.6201 14.83V9.18001C21.6201 7.80001 20.6301 6.12003 19.4201 5.45003L14.0801 2.48C12.9301 1.84 11.0701 1.84 9.93011 2.48Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M16.9998 13.2401V9.58014L7.50977 4.1001"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const OrderIcon = ({ isSelected }) => {
  return (
    <svg
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M9.62012 16L11.1201 17.5L14.3701 14.5"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8.80994 2L5.18994 5.63"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M15.1899 2L18.8099 5.63"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M2 7.8501C2 6.0001 2.99 5.8501 4.22 5.8501H19.78C21.01 5.8501 22 6.0001 22 7.8501C22 10.0001 21.01 9.8501 19.78 9.8501H4.22C2.99 9.8501 2 10.0001 2 7.8501Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
      />
      <path
        d="M3.5 10L4.91 18.64C5.23 20.58 6 22 8.86 22H14.89C18 22 18.46 20.64 18.82 18.76L20.5 10"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
      />
    </svg>
  );
};

export const ShipmentIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M11.9998 14H12.9998C14.0998 14 14.9998 13.1 14.9998 12V2H5.99976C4.49976 2 3.18977 2.82999 2.50977 4.04999"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M2 17C2 18.66 3.34 20 5 20H6C6 18.9 6.9 18 8 18C9.1 18 10 18.9 10 20H14C14 18.9 14.9 18 16 18C17.1 18 18 18.9 18 20H19C20.66 20 22 18.66 22 17V14H19C18.45 14 18 13.55 18 13V10C18 9.45 18.45 9 19 9H20.29L18.58 6.01001C18.22 5.39001 17.56 5 16.84 5H15V12C15 13.1 14.1 14 13 14H12"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8 22C9.10457 22 10 21.1046 10 20C10 18.8954 9.10457 18 8 18C6.89543 18 6 18.8954 6 20C6 21.1046 6.89543 22 8 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M16 22C17.1046 22 18 21.1046 18 20C18 18.8954 17.1046 18 16 18C14.8954 18 14 18.8954 14 20C14 21.1046 14.8954 22 16 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M22 12V14H19C18.45 14 18 13.55 18 13V10C18 9.45 18.45 9 19 9H20.29L22 12Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M2 8H8"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M2 11H6"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M2 14H4"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const CarrierReturnsIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M2 2H3.74001C4.82001 2 5.67 2.93 5.58 4L4.75 13.96C4.61 15.59 5.89999 16.99 7.53999 16.99H18.19C19.63 16.99 20.89 15.81 21 14.38L21.54 6.88C21.66 5.22 20.4 3.87 18.73 3.87H5.82001"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M16.25 22C16.9404 22 17.5 21.4404 17.5 20.75C17.5 20.0596 16.9404 19.5 16.25 19.5C15.5596 19.5 15 20.0596 15 20.75C15 21.4404 15.5596 22 16.25 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8.25 22C8.94036 22 9.5 21.4404 9.5 20.75C9.5 20.0596 8.94036 19.5 8.25 19.5C7.55964 19.5 7 20.0596 7 20.75C7 21.4404 7.55964 22 8.25 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M13.8296 7.71305C13.5071 7.61716 13.141 7.55615 12.74 7.55615C10.9356 7.55615 9.47998 9.02054 9.47998 10.8162C9.47998 12.6205 10.9444 14.0762 12.74 14.0762C14.5356 14.0762 16 12.6118 16 10.8162C16 10.145 15.7995 9.51737 15.4508 9.00309"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M14.3 7.80018L13.2104 6.55371"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M14.3 7.80029L13.0273 8.72425"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};
export const MyCarrierIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M2 2H3.74001C4.82001 2 5.67 2.93 5.58 4L4.75 13.96C4.61 15.59 5.89999 16.99 7.53999 16.99H18.19C19.63 16.99 20.89 15.81 21 14.38L21.54 6.88C21.66 5.22 20.4 3.87 18.73 3.87H5.82001"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M16.25 22C16.9404 22 17.5 21.4404 17.5 20.75C17.5 20.0596 16.9404 19.5 16.25 19.5C15.5596 19.5 15 20.0596 15 20.75C15 21.4404 15.5596 22 16.25 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M8.25 22C8.94036 22 9.5 21.4404 9.5 20.75C9.5 20.0596 8.94036 19.5 8.25 19.5C7.55964 19.5 7 20.0596 7 20.75C7 21.4404 7.55964 22 8.25 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M9 8H21"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeMiterlimit="10"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};
export const AccountsIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M12.1601 10.87C12.0601 10.86 11.9401 10.86 11.8301 10.87C9.45006 10.79 7.56006 8.84 7.56006 6.44C7.56006 3.99 9.54006 2 12.0001 2C14.4501 2 16.4401 3.99 16.4401 6.44C16.4301 8.84 14.5401 10.79 12.1601 10.87Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M7.16021 14.56C4.74021 16.18 4.74021 18.82 7.16021 20.43C9.91021 22.27 14.4202 22.27 17.1702 20.43C19.5902 18.81 19.5902 16.17 17.1702 14.56C14.4302 12.73 9.92021 12.73 7.16021 14.56Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const ContractIcon = ({ isSelected }) => {
  const color = isSelected ? "var(--primary-color)" : "#1E1E1E";

  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 576 512"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M384 0H144C119.8 0 99 20.75 99 46v420c0 25.25 20.75 46 46 46h342c25.25 0 45.99-20.75 45.99-46V128L384 0z"
        stroke={color}
        strokeWidth="20"
        strokeLinecap="round"
        strokeLinejoin="round"
      />

      <path
        d="M384 0v128h128"
        stroke={color}
        strokeWidth="20"
        strokeLinecap="round"
        strokeLinejoin="round"
      />

      <line
        x1="140"
        y1="150"
        x2="300"
        y2="150"
        stroke={color}
        strokeWidth="14"
        strokeLinecap="round"
      />

      <line
        x1="140"
        y1="210"
        x2="300"
        y2="210"
        stroke={color}
        strokeWidth="14"
        strokeLinecap="round"
      />

      <line
        x1="140"
        y1="270"
        x2="280"
        y2="270"
        stroke={color}
        strokeWidth="14"
        strokeLinecap="round"
      />

      <line
        x1="350"
        y1="280"
        x2="420"
        y2="210"
        stroke={color}
        strokeWidth="18"
        strokeLinecap="round"
      />

      <polygon
        points="420,210 435,195 410,190"
        fill="none"
        stroke={color}
        strokeWidth="16"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const SettingIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        fillRule="evenodd"
        clipRule="evenodd"
        d="M20.8064 7.62337L20.184 6.54328C19.6574 5.62936 18.4905 5.31408 17.5753 5.83847V5.83847C17.1397 6.0951 16.6198 6.16791 16.1305 6.04084C15.6411 5.91378 15.2224 5.59727 14.9666 5.16113C14.8021 4.8839 14.7137 4.56815 14.7103 4.2458V4.2458C14.7251 3.72898 14.5302 3.22816 14.1698 2.85743C13.8094 2.48669 13.3143 2.27762 12.7973 2.27783H11.5433C11.0367 2.27783 10.5511 2.47967 10.1938 2.8387C9.83644 3.19773 9.63693 3.68435 9.63937 4.19088V4.19088C9.62435 5.23668 8.77224 6.07657 7.72632 6.07646C7.40397 6.07311 7.08821 5.9847 6.81099 5.82017V5.82017C5.89582 5.29577 4.72887 5.61105 4.20229 6.52497L3.5341 7.62337C3.00817 8.53615 3.31916 9.70236 4.22975 10.2321V10.2321C4.82166 10.5738 5.18629 11.2053 5.18629 11.8888C5.18629 12.5723 4.82166 13.2038 4.22975 13.5456V13.5456C3.32031 14.0717 3.00898 15.2351 3.5341 16.1451V16.1451L4.16568 17.2344C4.4124 17.6795 4.82636 18.0081 5.31595 18.1472C5.80554 18.2863 6.3304 18.2247 6.77438 17.9758V17.9758C7.21084 17.7211 7.73094 17.6513 8.2191 17.782C8.70725 17.9126 9.12299 18.2328 9.37392 18.6714C9.53845 18.9486 9.62686 19.2644 9.63021 19.5868V19.5868C9.63021 20.6433 10.4867 21.4998 11.5433 21.4998H12.7973C13.8502 21.4998 14.7053 20.6489 14.7103 19.5959V19.5959C14.7079 19.0878 14.9086 18.5998 15.2679 18.2405C15.6272 17.8812 16.1152 17.6804 16.6233 17.6829C16.9449 17.6915 17.2594 17.7795 17.5387 17.9392V17.9392C18.4515 18.4651 19.6177 18.1541 20.1474 17.2435V17.2435L20.8064 16.1451C21.0615 15.7073 21.1315 15.1858 21.001 14.6961C20.8704 14.2065 20.55 13.7891 20.1108 13.5364V13.5364C19.6715 13.2837 19.3511 12.8663 19.2206 12.3767C19.09 11.8871 19.16 11.3656 19.4151 10.9277C19.581 10.6381 19.8211 10.398 20.1108 10.2321V10.2321C21.0159 9.70265 21.3262 8.54325 20.8064 7.63252V7.63252V7.62337Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <circle
        cx="12.1747"
        cy="11.8886"
        r="2.63616"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const IntegrationIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M7.7998 10.2002V14.4002"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M7.95001 9.89999C9.02697 9.89999 9.90002 9.02697 9.90002 7.95001C9.90002 6.87306 9.02697 6 7.95001 6C6.87306 6 6 6.87306 6 7.95001C6 9.02697 6.87306 9.89999 7.95001 9.89999Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M7.79999 17.9999C8.7941 17.9999 9.59998 17.194 9.59998 16.1999C9.59998 15.2058 8.7941 14.3999 7.79999 14.3999C6.80588 14.3999 6 15.2058 6 16.1999C6 17.194 6.80588 17.9999 7.79999 17.9999Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M16.1999 17.9999C17.194 17.9999 17.9999 17.194 17.9999 16.1999C17.9999 15.2058 17.194 14.3999 16.1999 14.3999C15.2058 14.3999 14.3999 15.2058 14.3999 16.1999C14.3999 17.194 15.2058 17.9999 16.1999 17.9999Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M7.87988 10.2002C8.14988 11.2502 9.10987 12.0302 10.2399 12.0202L12.2999 12.0102C13.8699 12.0002 15.2099 13.0102 15.6999 14.4202"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M9 22H15C20 22 22 20 22 15V9C22 4 20 2 15 2H9C4 2 2 4 2 9V15C2 20 4 22 9 22Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const DashboardIcon = ({ isSelected }) => {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <path
        d="M21 14H14V21H21V14Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M10 14H3V21H10V14Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M21 3H14V10H21V3Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <path
        d="M10 3H3V10H10V3Z"
        stroke={isSelected ? "var(--primary-color)" : "#1E1E1E"}
        strokeWidth="1.5"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
};

export const getMenuIcon = (iconName, isSelected = false) => {
  const normalized = (iconName || "").toLowerCase().replace(/[^a-z0-9]/g, "");
  switch (normalized) {
    case "dashboard":
    case "dashboards":
      return <DashboardIcon isSelected={isSelected} />;
    case "analytics":
      return <AnalyticsIcon isSelected={isSelected} />;
    case "store":
    case "stores":
    case "storesshipper":
      return <StoreIcon isSelected={isSelected} />;
    case "product":
    case "products":
    case "inventory":
      return <ProductIcon isSelected={isSelected} />;
    case "order":
    case "orders":
      return <OrderIcon isSelected={isSelected} />;
    case "shipment":
    case "shipments":
      return <ShipmentIcon isSelected={isSelected} />;
    case "carrierreturns":
    case "carrierreturn":
      return <CarrierReturnsIcon isSelected={isSelected} />;
    case "mycarrier":
    case "carriers":
    case "carrier":
      return <MyCarrierIcon isSelected={isSelected} />;
    case "account":
    case "accounts":
    case "userprofile":
      return <AccountsIcon isSelected={isSelected} />;
    case "contract":
    case "contracts":
      return <ContractIcon isSelected={isSelected} />;
    case "setting":
    case "settings":
    case "appsettings":
      return <SettingIcon isSelected={isSelected} />;
    case "integration":
    case "integrations":
      return <IntegrationIcon isSelected={isSelected} />;
    case "leads":
    case "lead":
    case "leadslist":
    case "contacts":
    case "uploadleads":
      return <AssignmentIcon sx={{ color: isSelected ? "var(--primary-color)" : "#1E1E1E" }} />;
    case "performancereport":
    case "report":
      return <SummarizeOutlinedIcon sx={{ color: isSelected ? "var(--primary-color)" : "#1E1E1E" }} />;
    default:
      return <DashboardIcon isSelected={isSelected} />;
  }
};


