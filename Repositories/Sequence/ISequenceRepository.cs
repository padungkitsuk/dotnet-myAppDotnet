namespace MyBackend.Repositories.Sequence;

public interface ISequenceRepository {
    Task<string> GetNextSequenceValue();
    Task<string> GetNextFleetValue();

}