

import jwt_decode from 'jwt-decode';
import { getThisKeyCookie , getPolicyCookie } from '../cookies';
import { getLocalStorageToken } from '../localStorage';

/* ------------------------------  CHECK COOKIE DATA FUNCTION  -------------------------------- */
export const isAuthenticated = () => {
	if (getThisKeyCookie('token')) {
		return true;
	} else {
		return false;
	}
};

/* ------------------------------  CHECK LOCAL STORAGE DATA FUNCTION  -------------------------------- */
export const isGenerateAccessToken = () => {
	if (getLocalStorageToken('local_token')) {
		return true;
	} else {
		return false;
	}
};

/* ------------------------------  CHECK COOKIE POLICY FUNCTION  -------------------------------- */
export const isCookiePolicy = () => {
	if (getPolicyCookie()) {
		return true;
    } else {
		return false;
    }
};

/* ------------------------------  DECODE JWT FUNCTION  -------------------------------- */
export const decodeJwtToken = (token) => {
	return jwt_decode(token);
};
