using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Common.Enums;
public enum MessageTypeEnum { [Display(Name = "Email")] Email, [Display(Name = "SMS")] SMS, }
