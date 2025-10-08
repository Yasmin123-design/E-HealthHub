using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BloodRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BloodRequest>> GetAllRequestsAsync()
        {
            return await _unitOfWork.BloodRequest.GetAllAsync();
        }

        public async Task<BloodRequest?> GetRequestByIdAsync(int id)
        {
            return await _unitOfWork.BloodRequest.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BloodRequest>> GetUnfulfilledRequestsAsync()
        {
            return await _unitOfWork.BloodRequest.GetUnfulfilledRequestsAsync();
        }

        public async Task<BloodRequest> AddRequestAsync(BloodRequest request)
        {
            await _unitOfWork.BloodRequest.AddAsync(request);
            await _unitOfWork.CompleteAsync();
            return request;
        }

        public async Task<bool> UpdateRequestAsync(int id, BloodRequest updatedRequest)
        {
            var existing = await _unitOfWork.BloodRequest.GetByIdAsync(id);
            if (existing == null) return false;

            existing.RequiredType = updatedRequest.RequiredType;
            existing.City = updatedRequest.City;
            existing.HospitalName = updatedRequest.HospitalName;
            existing.Units = updatedRequest.Units;
            existing.Fulfilled = updatedRequest.Fulfilled;

             _unitOfWork.BloodRequest.Update(existing);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            var existing = await _unitOfWork.BloodRequest.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.BloodRequest.Delete(existing);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

}
