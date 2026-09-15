// import React from "react";
// import { Box, Card, CardHeader, Divider, List, Stack, Dialog, DialogContent, Grid, Avatar, Typography } from "@mui/material";
// import { styleSheet } from "../../../assets/styles/style";
// import Slide from "@mui/material/Slide";
// import { useSelector } from "react-redux";
// import ButtonGroups from "../../shared/buttonGroup/index";
// const Transition = React.forwardRef(function Transition(props, ref) {
//   return <Slide direction="up" ref={ref} {...props} />;
// });
// function OrderDetailModal(props) {
//   let { open, setOpen } = props;
//   const LanguageReducer = useSelector((state) => state.LanguageReducer);
//   const handleClose = () => {
//     setOpen(false);
//   };
//   return (
//     <Dialog
//       open={open}
//       TransitionComponent={Transition}
//       keepMounted
//       scroll={"paper"}
//       onClose={handleClose}
//       maxWidth="lg"
//       sx={styleSheet.modelOrderDetail}
//       aria-describedby="alert-dialog-slide-description"
//     >
//       <DialogContent sx={{ width: "1000px", minHeight: "30vh" }}>
//         <Grid container spacing={1}>
//           <Grid sm={12} md={4}>
//             this is map area
//           </Grid>
//           <Grid sm={12} md={8}>
//             <Card sx={{ boxShadow: "none" }}>
//               <CardHeader
//                 avatar={
//                   <Avatar sx={{ bgcolor: "#F34F26", fontSize: "14px", fontWeight: "500" }} variant="circle">
//                     FE
//                   </Avatar>
//                 }
//                 title={<Typography sx={styleSheet.cardTitleOrder}>Fatih Ehsen</Typography>}
//                 subheader={<Typography sx={styleSheet.cardDesOrder}>alikhan@gmail.com</Typography>}
//                 action={
//                   <ButtonGroups
//                     id="category-button-menu"
//                     // bgColor="#fff"
//                     bgColorHover="#5237cc"
//                     placement={"bottom"}
//                     // color="white"
//                     placeholder="AWBZ Labels"
//                     size="small"
//                     fontSize="13px"
//                     options={[
//                       { title: "Virtual", value: "Virtual" },
//                       { title: "In Person", value: "In Person" },
//                     ]}
//                     // onChangeMenu={(value) => handleUpdateDetail("type", value)}
//                     {...props}
//                   />
//                 }
//               />
//             </Card>
//             <Box sx={styleSheet.orderDetailStats}>
//             <Box sx={styleSheet.orderDetailStatsItem}>
//              </Box>
//             </Box>
//             <Divider />
//           </Grid>
//         </Grid>
//       </DialogContent>
//     </Dialog>
//   );
// }
// export default (OrderDetailModal);
