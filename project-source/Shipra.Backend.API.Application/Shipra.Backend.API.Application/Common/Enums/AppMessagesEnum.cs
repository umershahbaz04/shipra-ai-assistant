using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Common.Enums;

  public enum AppMessagesEnum
  {
      [Display(Name = "Registration SMS To Patient")]
      SMS_Patient_PatientRegistration = 1,
      [Display(Name = "Appointment Confirmation SMS To Patient")]
      SMS_Patient_AppointmentConfirmation = 2,
      [Display(Name = "Appointment Confirmation SMS For Location")]
      SMS_Location_AppointmentConfirmation = 3,
      [Display(Name = "Appointment Offer SMS To Patient")]
      SMS_Patient_AppointmentOffer = 4,
  }
