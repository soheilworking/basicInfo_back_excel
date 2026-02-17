using Mapster;
using ORG.BasicInfo.EXTService.DTO.UpdateServiceDto;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ORG.BasicInfo.API.Extensions
{
    public class ReadServiceInfo
    {
        private ulong _idCodeService;
        public UpdateServiceGrpcRequestServiceDto UpdateServiceGrpc=new UpdateServiceGrpcRequestServiceDto();
        public ReadServiceInfo(ulong idCodeService)
        {
            _idCodeService = idCodeService;
            
        }
        public string ReadServiceInfoToDto() {
            //check repeated idcode name action and entity from other service server side
            //check repeated idcode name action and entity in this class

            UpdateServiceGrpc.IdCodePrivate= _idCodeService;
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "ServiceInformation");
            var serviceInfoPath = Path.Combine(basePath, "ServiceInfo.json");

            var serviceInfoJson = File.ReadAllText(serviceInfoPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var serviceInfo = JsonSerializer.Deserialize<ServiceInfoJson>(serviceInfoJson, options);
            UpdateServiceGrpc.Entities = new List<EntitiesServiceDto>();
            List<ulong> idCodeActionList=new List<ulong>();
            List<ulong> idCodeEntityList = new List<ulong>();
            if (serviceInfo != null)
                idCodeEntityList.AddRange(serviceInfo.Entities.Select(item => item.IdCode));
            foreach (var entity in serviceInfo.Entities)
            {
                // مسیر فایل اکشن
                var actionFilePath = Path.Combine(basePath, entity.ActionEntity);
                

                if (!File.Exists(actionFilePath))
                {
                    Console.WriteLine($"File not found: {actionFilePath}");
                    return "false";
                    continue;
                }
   

                var actionJson = File.ReadAllText(actionFilePath);

              
                var actions = JsonSerializer.Deserialize<List<ActionsServiceJson>>(actionJson, options);
                var entitiesServiceDto = new EntitiesServiceDto { Label= entity.Label,
                    IdCode=entity.IdCode,
                    Description= entity.Description,
                    IsCategoryBranche=entity.IsCategoryBranche,
                    IsCategoryPosition=entity.IsCategoryPosition
                };
                entitiesServiceDto.ActionEntity = new List<ActionsServiceDto>();
                entitiesServiceDto.ActionEntity.AddRange(actions.Adapt<List<ActionsServiceDto>>());
                UpdateServiceGrpc.Entities.Add(entitiesServiceDto);
                if(actions!=null)
                idCodeActionList.AddRange(actions.Select(item => item.IdCode));

            }

            var resultRepeatedEntity = serachRepeatedIdCode(idCodeEntityList.ToArray());

            var resultRepeatedAction = serachRepeatedIdCode(idCodeActionList.ToArray());
            if (resultRepeatedEntity.Count() > 0)
                return $"duplicated idCode in Entities{string.Join(",", resultRepeatedEntity)}";
            if (resultRepeatedAction.Count() > 0)
                return $"duplicated idCode in Actions{string.Join(",", resultRepeatedAction)}";

            return "";

        }
        public IEnumerable<ulong> serachRepeatedIdCode(ulong[] array)
        {
            List<ulong> repeatedIdCode = new List<ulong>();
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i+1; j < array.Length-1; j++)
                {
                    if (array[i] == array[j])
                        repeatedIdCode.Add(array[i]);
                }
            }
            return repeatedIdCode.ToArray();
        }

    }
    public class ServiceInfoJson
    {
        public ulong IdCode { get; set; }
        public List<EntitiesServiceJson> Entities { get; set; }
    }
    public class EntitiesServiceJson
    {
        public ulong IdCode { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsCategoryBranche { get; set; }
        public bool IsCategoryPosition { get; set; }
        public string ActionEntity { get; set; }
    }
    public class ActionsServiceJson
    {
        public ulong IdCodeEntity { get; set; }
        public ulong IdCode { get; set; }
        public string Label { get; set; }
        public uint TypeAction { get; set; }
        public string Url { get; set; }
        public string MethodHttp { get; set; }

        public string Description { get; set; }
        public bool Notification { get; set; }
        public ulong[] RelatedActionComminucation { get; set; }


    }
}
