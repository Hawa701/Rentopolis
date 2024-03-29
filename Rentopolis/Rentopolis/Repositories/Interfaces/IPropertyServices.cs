﻿using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IPropertyServices
    {
        Task<Status> AddNewProperty(NewPropertyViewModel model);
        Task<int> GetNumberOfPropertiesOwned(string id);
        Task<int> GetNumberOfOwnedPropertyStatus(string id, string neededStatus);
        Task<List<Property>> GetAllProperties(string searchString);
        Task<List<Property>> GetNonApprovedProperties();
        Task<List<Property>> GetOwnedProperties(string id, string searchString);
        Task<List<Property>> GetApprovedProperties(string searchString);
        Task<int> GetApprovedPropertiesNumber();
        Task<Property> GetPropertyDetail(int id);
        Task<Status> ApproveProperty(int id);
        Task<Status> RejectProperty(int id);
        Task<UpdatePropertyInfoViewModel> GetPropertyDetailForEditing(int id);
        Task<Status> EditPropertyInfo(UpdatePropertyInfoViewModel model);
        Task<Status> ReapplyRejectedProperty(int propertyId);
        Task<Status> DeletePropertyInfo(int id);
        Task<SavedProperties> IsAreadySaved(int propertyId, string tenantId);
        Task<Status> HandleSaveRequest(int propertyId, string tenantId);
        Task<List<Property>> GetSavedProperties(string tenantId, string searchString);
        Task<RentRequests> IsAreadyRequested(int propertyId, string tenantId);
        Task<Status> HandleRentalRequest(int propertyId, string tenantId);
        Task<List<Property>> GetRequestedProperties(string tenantId, string searchString);
        Task<List<AppUser>> GetApplicants(int propertyId);
        Task<Status> AcceptApplicantsRequest(int propertyId, string tenantId);
        Task<Status> UndoApplicantsRequest(int propertyId, string tenantId);
        Task<RentRequests> IsAccepted(int propertyId, string tenantId);
        Task<RentRequests> IsRejected(int propertyId, string tenantId);
    }
}
