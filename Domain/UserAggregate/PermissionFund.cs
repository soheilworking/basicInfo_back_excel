using ClassEncryptionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORG.BasicInfo.Domain.UserAggregate
{
    public class PermissionFund
    {
        public Guid Id { get; set; }
        public string IdUser { get; set; }
        public string IdFund { get; set; }
        private static AesEncryption _aesEncryption;
        public PermissionFund()
        {
            setKey();
        }
        private void setKey()
        {
            if (_aesEncryption == null)
                _aesEncryption = new AesEncryption("f7dfe9e0f2369ff888255d94daa40ebec084d96cb8f0e7b1af99b99c3ba73912", "c1f8d799654d4c665c9caf823e7e8425");

        }
        public async Task SetData(Guid idUser, Guid idFund)
        {

            Id = Guid.NewGuid();
            IdUser =await _aesEncryption.Encrypt(idUser.ToString());
            IdFund = await _aesEncryption.Encrypt(idUser.ToString() + "_"+idFund.ToString());
        }
        public async Task<Guid> GetIdFund()
        {
            setKey();
            var arrTmp =  await _aesEncryption.Decrypt(IdFund);
            var arrT= arrTmp.Split("_");
            Guid tmp=Guid.Empty;
            if (arrT.Length > 0)
                tmp = Guid.Parse(arrT[1]);

            return tmp;

        }
        public static async Task<string> GetEncryptDataWithThisKey(string input)
        {
            _aesEncryption = new AesEncryption("f7dfe9e0f2369ff888255d94daa40ebec084d96cb8f0e7b1af99b99c3ba73912", "c1f8d799654d4c665c9caf823e7e8425");
            return await _aesEncryption.Encrypt(input);
        }
   


    }
}
