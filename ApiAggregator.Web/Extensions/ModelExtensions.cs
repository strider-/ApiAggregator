using System;
using System.Linq;
using ApiAggregator.Core.Models;
using ApiAggregator.Web.ViewModels;

namespace ApiAggregator.Web.Extensions
{
    public static class ModelExtensions
    {
        public static MappingModel ToViewModel(this ApiMapping mapping)
        {
            return new MappingModel
            {
                Id = mapping.Id,
                ServiceId = mapping.Service.Id,
                ServiceName = mapping.Service.Name,
                Name = mapping.Name,
                Endpoint = mapping.Endpoint,
                Api = mapping.Api,
                Method = mapping.Method,
                Enabled = mapping.Enabled
            };
        }

        public static ApiMapping ToDomainModel(this MappingModel model, ApiMapping mapping = null)
        {
            mapping = (mapping ?? new ApiMapping());

            mapping.Api = model.Api;
            mapping.Endpoint = model.Endpoint;
            mapping.Created = DateTime.Now;
            mapping.Name = model.Name;
            mapping.Method = model.Method;
            mapping.ServiceId = model.ServiceId.Value;
            mapping.Enabled = model.Enabled;

            return mapping;
        }


        public static ServiceModel ToViewModel(this Service service)
        {
            return new ServiceModel
            {
                Id = service.Id,
                Name = service.Name,
                RootUrl = service.RootUrl,
                Enabled = service.Enabled,
                Headers = service.Headers.Select(h => new ServiceHeaderModel { Id = h.Id, Header = h.Header, Value = h.Value, ServiceId = h.ServiceId }).ToList(),
                QueryStringAppends = service.QueryStringAppends.Select(q => new ServiceQueryStringModel { Id = q.Id, Name = q.Name, Value = q.Value, ServiceId = q.ServiceId }).ToList()
            };
        }

        public static Service ToDomainModel(this ServiceModel model, Service service = null)
        {
            service = (service ?? new Service());

            service.Name = model.Name;
            service.RootUrl = model.RootUrl;
            service.Enabled = model.Enabled;
            service.Headers = model.Headers.Select(h => new ServiceHeader { Id = h.Id, Header = h.Header, Value = h.Value, ServiceId = h.ServiceId }).ToList();
            service.QueryStringAppends = model.QueryStringAppends.Select(q => new ServiceQueryString { Id = q.Id, Name = q.Name, Value = q.Value, ServiceId = q.ServiceId }).ToList();

            return service;
        }
    }
}