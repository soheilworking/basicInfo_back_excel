using Azure.Core;
using ClassEncryptionLibrary;
using System;
using System.Reflection;
using BCrypt.Net;
namespace ORG.BasicInfo.Domain.UserAggregate;

public class User : IEntity<Guid>
{
    private static AesEncryption _aesEncryption;
    public User()
    {
        setKey();
    }
    public User(ulong nationalCodeUser, 
        string name,
       
        string lastName,
        ulong mobile,
        ulong idCodeNationalFund,
        string fundName,
        ulong fundCode,
        bool isOrgUser,
        string ipUser,
         byte[] certificate,
        Guid lUserCreate):this()
    {
        Id = Guid.NewGuid();
        Name = name;
        LastName = lastName;
        NationalCodeUser = nationalCodeUser;
        IdCodeNationalFund = idCodeNationalFund;
        IpUser = ipUser;
        Mobile = mobile;
        FundName = fundName;
        FundCode = fundCode;
        IsOrgUser = isOrgUser;
         TCreate = (DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        LUserCreate = lUserCreate;
        TEdit = 0;
        LUserEdit = default;
        State = UserState.Active;
        Certificate = certificate;
    }
    private void setKey()
    {
        if (_aesEncryption == null)
            _aesEncryption = new AesEncryption("7ab9d10db1ae2a86526a9dd18e27b9acacd53af784de774773e06fac789598c2", "e3bf2271a5974ed9f42480c0f0b46862");


    }
    public async Task SetUserRole(ushort userRole)
    {
        setKey();
        UserRole =await _aesEncryption.Encrypt(Id.ToString() + "_" + userRole.ToString());
    }
    public async Task<ushort> GetUserRole()
    {
        setKey();
        var role = await _aesEncryption.Decrypt(UserRole);

        var arrTmp=role.Split("_");
        
        if (arrTmp.Length > 0 && arrTmp[0]==Id.ToString()) return Convert.ToUInt16(arrTmp[1]);
        return 0;

    }
    public void ChangeInfo(
        ulong nationalCodeUser,
        string name,
        string lastName,
        ulong mobile,
        ulong idCodeNationalFund,
        string fundName,
         ulong fundCode,
         bool isOrgUser,
            string ipUser,
             byte[] certificate,
        Guid lUserEdit)
    {
    
        Name = name;
        LastName = lastName;
        Mobile = mobile;
        NationalCodeUser = nationalCodeUser;
        FundName = fundName;
        FundCode = fundCode;
        IdCodeNationalFund = idCodeNationalFund;
        IsOrgUser = isOrgUser;
        IpUser = ipUser;
        TEdit = (DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        LUserEdit = lUserEdit;
        Certificate = certificate;


    }
    public void ChangeState()
    {
        State = State== UserState.Active? UserState.Inactive: UserState.Active;
    }
    public void ChangePassword(string password)
    {
        Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA512);
    }
    public bool ComparePassword(string password)
    {
        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA512);
        if (hashedPassword == password)
            return true;
        return false;
    }
    public async Task<string> GetEncryptDataWithThisKey(string input)
    {
        setKey();
        return await _aesEncryption.Encrypt(input);
    }
    //idCodeNationalFund:number;
    //nationalCodeUser:number;
    //ipUser?:string;
   
    public Guid Id { get; }
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public ulong Mobile { get; private set; }
    public string FundName { get; private set; }
    public bool IsOrgUser { get; private set; }
    public ulong FundCode { get; private set; }
    public string UserRole { get; private set; }
    public ulong IdCodeNationalFund { get; private set; }
    public ulong NationalCodeUser { get; private set; }
    public string IpUser { get; private set; }
    public byte[] Certificate { get; private set; }
    public UserState State { get; private set; }
    public long TCreate { get; private set; }
    public long TEdit { get; private set; }
    public Guid LUserCreate { get; private set; }
    public Guid LUserEdit { get; private set; }
    public string Password { get; private set; }

}