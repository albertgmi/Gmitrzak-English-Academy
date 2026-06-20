using inzBackend.Entities.Curriculum;
using inzBackend.Models.MatrixModels;

namespace inzBackend.Services.MatrixServices
{
    public interface IMatrixService
    {
        List<MatrixDto> GetAllMatrices();
        Matrix CreateMatrix(CreateMatrixRequest request);
        void UpdateMatrix(int matrixId, UpdateMatrixRequest request);
        void DeleteMatrix(int matrixId);
        void AssignCourse(int matrixId, int courseId);
        void DetachCourse(int matrixId, int courseId);
    }
}
