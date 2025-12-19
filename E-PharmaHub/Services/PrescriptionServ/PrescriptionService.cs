using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.PrescriptionServ
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PrescriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PrescriptionDto>> GetUserPrescriptionsAsync(string userId)
        {
            var prescriptions =
                await _unitOfWork.Prescriptions.GetByUserIdAsync(userId);

            return prescriptions
                .Select(p => p.ToDto())
                .ToList();
        }
        public async Task CreateAsync(CreatePrescriptionDto dto)
        {
            var prescription = dto.ToEntity(dto.UserId);

            await _unitOfWork.Prescriptions.AddAsync(prescription);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateItemsAsync(
            int prescriptionId,
            List<PrescriptionItemDto> items)
        {
            var prescription =
                await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId);

            if (prescription == null)
                throw new Exception("Prescription not found");

            prescription.Items.Clear();

            foreach (var dto in items)
                prescription.Items.Add(dto.ToEntity());

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int prescriptionId)
        {
            var prescription =
                await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId);

            if (prescription == null)
                throw new Exception("Prescription not found");

            _unitOfWork.Prescriptions.DeleteAsync(prescription);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AddItemAsync(int prescriptionId, PrescriptionItemDto dto)
        {
            var prescription =
                await _unitOfWork.Prescriptions.GetWithItemsAsync(prescriptionId);

            if (prescription == null)
                throw new Exception("Prescription not found");

            prescription.Items.Add(dto.ToEntity());

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateItemAsync(int itemId, PrescriptionItemDto dto)
        {
            var item =
                await _unitOfWork.PrescriptionItems.GetByIdAsync(itemId);

            if (item == null)
                throw new Exception("Item not found");

            item.MedicationName = dto.MedicationName;
            item.MedicationStrength = dto.MedicationStrength;
            item.Dosage = dto.Dosage;
            item.Quantity = dto.Quantity;
            item.Duration = dto.Duration;
            item.Notes = dto.Notes;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteItemAsync(int itemId)
        {
            var item =
                await _unitOfWork.PrescriptionItems.GetByIdAsync(itemId);

            if (item == null)
                throw new Exception("Item not found");

            _unitOfWork.PrescriptionItems.Delete(item);
            await _unitOfWork.CompleteAsync();
        }
    }

}
