using Township_API.Models;

namespace Township_API.Controllers
{
    internal class JsonWrapper
    {
        public List<PrimaryLandowner> owners { get; set; }
        public Task<List<DependentLandOwner>> DependentOwners { get; set; }
        public Task<List<Vehicle>> Vehicles { get; set; }
        public Task<List<UserNRDAccess>> UserNRDAccess { get; set; }
        public Task<List<UserBuildingAccess>> UserBuildingAccess { get; set; }
        public Task<List<UserAmenitiesAccess>> UserAmenitiesAccess { get; set; }

    }

    internal class ProfileWrapper
    { 
        public List<profileRegister> profileRegister { get; set; }
    }
}