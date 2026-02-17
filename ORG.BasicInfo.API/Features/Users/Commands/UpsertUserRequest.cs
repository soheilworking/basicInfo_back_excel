using Ardalis.Result;
using MediatR;
using ORG.BasicInfo.API.Features.Abstractions;
using ORG.BasicInfo.Domain.UserAggregate;

namespace ORG.BasicInfo.API.Features.Users.Commands;

public class UpsertUserRequest : IRequest<Result<ResponseWrite>>
{
    //ulong nationalCodeUser,

    //    string lastName,
    //    ulong mobile,
    //    ulong idCodeNationalFund,
    //    string fundName,
    //    ulong fundCode,
    //    bool isOrgUser,
    //    string ipUser,
    //    Guid lUserCreate)
    public UpsertUserRequest(
        ulong nationalCodeUser,
        string name,
        string lastName,
        string mobile,
        ulong idCodeNationalFund,
        string fundName,
        ulong fundCode,
        string userRole,
        string ipUser,
        IEnumerable<string> permissionFunds,
        string id=null)
    {
        Name = name;
        LastName = lastName;
        Mobile =ulong.Parse(mobile);
        FundName = fundName;
        FundCode = fundCode;
        UserRole =userRole;
        PermissionFunds = permissionFunds;
        Id = id;
        NationalCodeUser = nationalCodeUser;
        IdCodeNationalFund = idCodeNationalFund;
        IpUser = ipUser;

    }

    public UpsertUserRequest()
    {
    }
    public ulong NationalCodeUser { get; set; }
    public ulong IdCodeNationalFund { get; set; }
    public string IpUser { get; set; }
    public string Id { get;  set; }
    public string Name { get;  set; }
    public string LastName { get;  set; }
    public ulong Mobile { get;  set; }
    public string FundName { get;  set; }
    public string Password { get; set; }
    public ulong FundCode { get;  set; }
    //public bool IsOrgUser { get;  set; }
    public string UserRole { get; set; }

    public IEnumerable<string> PermissionFunds { get; set; }


}