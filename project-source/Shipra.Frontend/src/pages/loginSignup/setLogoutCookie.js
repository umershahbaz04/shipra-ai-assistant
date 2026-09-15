export default function setLogoutCookie(res) {
  sessionStorage.removeItem("menuPermissions");
  localStorage.removeItem("menuPermissions");
  document.cookie.split(";").forEach((cookie) => {
    const [name] = cookie.split("=");
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  });
}
