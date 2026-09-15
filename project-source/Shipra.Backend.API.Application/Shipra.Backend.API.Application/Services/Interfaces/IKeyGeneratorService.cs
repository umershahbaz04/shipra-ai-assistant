using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface IKeyGeneratorService
{
  string EncryptString(string plaintext);
  string DecryptString(string encryptedKey);
}
