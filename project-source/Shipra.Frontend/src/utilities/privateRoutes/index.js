
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { isAuthenticated } from "../authentication";

const PrivateRoute = ({ component: Component }, props) => {

  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);

  const isAuth = isAuthenticated()

  useEffect(() => {
    if (isAuth) {
      setLoading(false);
      return;
    }
    navigate("/");
  }, []);

  return (
    <>
      {loading ?
        "Loading..."
        :
        <Component {...props} />
      }
    </>
  );
};

export default PrivateRoute;