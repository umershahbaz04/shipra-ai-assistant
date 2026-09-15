{
  /* <CustomRHFReactDatePickerInput
                  name="chkDate"
                  control={control}
                  onChange={handleOnChange}
                  required
                  error={
                    LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                  }
                />
                <SelectComponent
                  name="employee"
                  control={control}
                  options={allEmployees}
                  requiredError={
                    LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT
                  }
                  errors={errors}
                  // onChange={handleChange}
                /> */
}
// for multiple
// <SelectComponent
// multiple={true}
// name="reason"
// options={storesForSelection}
// value={storeId}
// getOptionLabel={(option)=>option.storeName}
// onChange={(e,val)=>{
//   setStoreId(val?.map((data)=>(data.storeId)))
// }}
// />
// for single

// <SelectComponent
// name="reason"
// options={storesForSelection}
// value={storeId}
// getOptionLabel={(option)=>option.storeName}
// onChange={(e,val)=>{
//   setStoreId(val?storeId)
// }}
// />

// <CustomRHFPhoneInput
//     error={LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT}
//     name="customerServiceNo"
//     control={control}
//     required
//     inputAdornment={
//       <Box sx={{ position: "absolute", right: 27, top: 18 }}>
//         <Tooltip title="This Number will reflect on Airway Bill">
//           <InputAdornment position="end">
//             <ErrorOutlineIcon
//               fontSize="small"
//               sx={{ color: "black", cursor: "pointer" }}
//             />
//           </InputAdornment>
//         </Tooltip>
//       </Box>
//     }
//   />

// UtilityClass.convertArrIntoCommaSeperatedIds(storeId, "storeId")
