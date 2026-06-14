using inzBackend.Entities.Curriculum;
using inzBackend.Models.MatrixModels;

namespace inzBackend.Services.MatrixServices
{
    public interface IMatrixService
    {
        List<MatrixDto> getAllMatrices();
        Matrix createMatrix(CreateMatrixRequest request);
        void updateMatrix(int matrixId, UpdateMatrixRequest request);
        void deleteMatrix(int matrixId);
        void assignCourse(int matrixId, int courseId);
        void detachCourse(int matrixId, int courseId);
    }
}
