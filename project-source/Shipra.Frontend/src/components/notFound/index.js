import React from "react";
import BlockIcon from "@mui/icons-material/Block";
import { styleSheet } from "../../assets/styles/style";
import { useNavigate } from "react-router-dom";

function NotFound() {
  let navigate = useNavigate();
  return (
    <div style={styleSheet['@global']['.Unauthorized-page']}>
      <div>
        {" "}
        <center>
          <BlockIcon color="error" fontSize="large" />
        </center>
        <h1>
          <span>O</span>oop!
        </h1>
        <p>
          We can't seem to find the page you're looking for.
          <br />
          <b>404 - Page not found</b>
        </p>
        <div className="form-group-element">
          <center>
            <button onClick={() => navigate(-1)}>Go Back</button>
          </center>
        </div>
      </div>
    </div>
  );
}
export default (NotFound);
