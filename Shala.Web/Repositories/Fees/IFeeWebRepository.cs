////using Shala.Shared.Requests.Fees;
////using Shala.Shared.Responses.Fees;

////namespace Shala.Web.Repositories.Fees;

////public interface IFeeWebRepository
////{
////    Task<List<FeeHeadResponse>> GetFeeHeadsAsync(CancellationToken cancellationToken = default);
////    Task<FeeHeadResponse?> GetFeeHeadByIdAsync(int id, CancellationToken cancellationToken = default);
////    Task<FeeHeadResponse> CreateFeeHeadAsync(CreateFeeHeadRequest request, CancellationToken cancellationToken = default);
////    Task UpdateFeeHeadAsync(int id, UpdateFeeHeadRequest request, CancellationToken cancellationToken = default);
////    Task DeleteFeeHeadAsync(int id, CancellationToken cancellationToken = default);

////    Task<List<FeeStructureResponse>> GetFeeStructuresAsync(CancellationToken cancellationToken = default);
////    Task<FeeStructureResponse?> GetFeeStructureByIdAsync(int id, CancellationToken cancellationToken = default);
////    Task<FeeStructureResponse> CreateFeeStructureAsync(CreateFeeStructureRequest request, CancellationToken cancellationToken = default);
////    Task UpdateFeeStructureAsync(int id, UpdateFeeStructureRequest request, CancellationToken cancellationToken = default);
////    Task DeleteFeeStructureAsync(int id, CancellationToken cancellationToken = default);

////    Task<List<StudentChargeResponse>> GetStudentChargesAsync(int studentId, CancellationToken cancellationToken = default);
////    Task CancelChargeAsync(int chargeId, CancellationToken cancellationToken = default);

////    Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(int studentId, CancellationToken cancellationToken = default);
////    Task<FeeReceiptResponse> CollectFeeAsync(CreateFeeReceiptRequest request, CancellationToken cancellationToken = default);
////    Task CancelReceiptAsync(int receiptId, string? reason = null, CancellationToken cancellationToken = default);

////    Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(int admissionId, CancellationToken cancellationToken = default);
////    Task<StudentFeeAssignmentResponse> AssignFeeStructureAsync(CreateStudentFeeAssignmentRequest request, CancellationToken cancellationToken = default);
////    Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(int assignmentId, UpdateStudentFeeAssignmentRequest request, CancellationToken cancellationToken = default);
////    Task DeleteAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default);
////    Task<List<StudentChargeResponse>> GenerateChargesAsync(int assignmentId, CancellationToken cancellationToken = default);


////    Task<FeeDashboardResponse> GetDashboardAsync(
////        FeeDashboardRequest request,
////        CancellationToken cancellationToken = default);

////    Task<FeeLedgerDashboardResponse> GetFeeLedgerDashboardAsync(
////    FeeLedgerDashboardRequest request,
////    CancellationToken cancellationToken = default);
////}



//using Shala.Shared.Requests.Fees;
//using Shala.Shared.Responses.Fees;

//namespace Shala.Web.Repositories.Fees;

//public interface IFeeWebRepository
//{
//    Task<List<FeeHeadResponse>> GetFeeHeadsAsync(CancellationToken cancellationToken = default);
//    Task<FeeHeadResponse?> GetFeeHeadByIdAsync(int id, CancellationToken cancellationToken = default);
//    Task<FeeHeadResponse> CreateFeeHeadAsync(CreateFeeHeadRequest request, CancellationToken cancellationToken = default);
//    Task UpdateFeeHeadAsync(int id, UpdateFeeHeadRequest request, CancellationToken cancellationToken = default);
//    Task DeleteFeeHeadAsync(int id, CancellationToken cancellationToken = default);

//    Task<List<FeeStructureResponse>> GetFeeStructuresAsync(CancellationToken cancellationToken = default);
//    Task<FeeStructureResponse?> GetFeeStructureByIdAsync(int id, CancellationToken cancellationToken = default);
//    Task<FeeStructureResponse> CreateFeeStructureAsync(CreateFeeStructureRequest request, CancellationToken cancellationToken = default);
//    Task UpdateFeeStructureAsync(int id, UpdateFeeStructureRequest request, CancellationToken cancellationToken = default);
//    Task DeleteFeeStructureAsync(int id, CancellationToken cancellationToken = default);

//    Task<List<StudentChargeResponse>> GetStudentChargesAsync(int studentId, CancellationToken cancellationToken = default);
//    Task CancelChargeAsync(int chargeId, CancellationToken cancellationToken = default);

//    Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(int studentId, CancellationToken cancellationToken = default);
//    Task<FeeReceiptResponse> CollectFeeAsync(CreateFeeReceiptRequest request, CancellationToken cancellationToken = default);
//    Task CancelReceiptAsync(int receiptId, string? reason = null, CancellationToken cancellationToken = default);

//    Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(int admissionId, CancellationToken cancellationToken = default);
//    Task<StudentFeeAssignmentResponse> AssignFeeStructureAsync(CreateStudentFeeAssignmentRequest request, CancellationToken cancellationToken = default);

//    Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
//        int assignmentId,
//        UpdateStudentFeeAssignmentRequest request,
//        CancellationToken cancellationToken = default);

//    Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
//        UpdateStudentFeeAssignmentRequest request,
//        CancellationToken cancellationToken = default);

//    Task DeleteAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default);
//    Task<List<StudentChargeResponse>> GenerateChargesAsync(int assignmentId, CancellationToken cancellationToken = default);

//    Task<FeeDashboardResponse> GetDashboardAsync(
//        FeeDashboardRequest request,
//        CancellationToken cancellationToken = default);

//    Task<FeeLedgerDashboardResponse> GetFeeLedgerDashboardAsync(
//        FeeLedgerDashboardRequest request,
//        CancellationToken cancellationToken = default);
//}



using Shala.Shared.Requests.Fees;
using Shala.Shared.Responses.Fees;

namespace Shala.Web.Repositories.Fees;

public interface IFeeWebRepository
{
    Task<List<FeeHeadResponse>> GetFeeHeadsAsync(CancellationToken cancellationToken = default);
    Task<FeeHeadResponse?> GetFeeHeadByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FeeHeadResponse> CreateFeeHeadAsync(CreateFeeHeadRequest request, CancellationToken cancellationToken = default);
    Task UpdateFeeHeadAsync(int id, UpdateFeeHeadRequest request, CancellationToken cancellationToken = default);
    Task DeleteFeeHeadAsync(int id, CancellationToken cancellationToken = default);

    Task<List<FeeStructureResponse>> GetFeeStructuresAsync(CancellationToken cancellationToken = default);
    Task<FeeStructureResponse?> GetFeeStructureByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FeeStructureResponse> CreateFeeStructureAsync(CreateFeeStructureRequest request, CancellationToken cancellationToken = default);
    Task UpdateFeeStructureAsync(int id, UpdateFeeStructureRequest request, CancellationToken cancellationToken = default);
    Task DeleteFeeStructureAsync(int id, CancellationToken cancellationToken = default);

    Task<List<StudentChargeResponse>> GetStudentChargesAsync(int studentId, CancellationToken cancellationToken = default);
    Task CancelChargeAsync(int chargeId, CancellationToken cancellationToken = default);

    Task<List<FeeReceiptResponse>> GetStudentReceiptsAsync(int studentId, CancellationToken cancellationToken = default);
    Task<FeeReceiptResponse?> GetReceiptByIdAsync(int receiptId, CancellationToken cancellationToken = default);
    Task<FeeReceiptResponse> CollectFeeAsync(CreateFeeReceiptRequest request, CancellationToken cancellationToken = default);
    Task CancelReceiptAsync(int receiptId, string? reason = null, CancellationToken cancellationToken = default);

    Task<StudentFeeAssignmentResponse?> GetAssignmentAsync(int admissionId, CancellationToken cancellationToken = default);
    Task<StudentFeeAssignmentResponse> AssignFeeStructureAsync(CreateStudentFeeAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
        int assignmentId,
        UpdateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken = default);
    Task<StudentFeeAssignmentResponse> UpdateAssignmentAsync(
        UpdateStudentFeeAssignmentRequest request,
        CancellationToken cancellationToken = default);
    Task<bool> CanModifyAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default);
    Task<List<StudentChargeResponse>> GenerateChargesAsync(int assignmentId, CancellationToken cancellationToken = default);

    Task<FeeDashboardResponse> GetDashboardAsync(
        FeeDashboardRequest request,
        CancellationToken cancellationToken = default);

    Task<FeeLedgerDashboardResponse> GetFeeLedgerDashboardAsync(
        FeeLedgerDashboardRequest request,
        CancellationToken cancellationToken = default);
}