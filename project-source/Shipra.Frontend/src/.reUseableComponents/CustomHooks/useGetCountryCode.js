import { useState } from "react";

export default function useGetCountryCode() {
  const [phoneCode, setPhoneCode] = useState("ae");

  return { phoneCode, setPhoneCode };
}
