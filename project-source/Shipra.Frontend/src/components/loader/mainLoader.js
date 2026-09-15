import React from 'react'
import mbLogo from '../../assets/images/header/mbLogo.png';
import { styleSheet } from "../../assets/styles/style";
function MainLoader() {
  return (
    <div style={styleSheet['@global']['.method-main-loader']}>
      <div style={styleSheet['@global']['.method-main-loader .blob .setImage-dfd']}
      >
        <img className="m-auto" src={mbLogo} alt="blob" />
      </div>
    </div>
  )
}
export default (MainLoader);