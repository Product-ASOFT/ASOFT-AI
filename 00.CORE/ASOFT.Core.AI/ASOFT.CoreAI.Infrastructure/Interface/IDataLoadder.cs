using ASOFT.CoreAI.Entities;

public interface IDataLoader
{
    Task LoadTrainingDataFromDocument(LoadFileRequest request, CancellationToken cancellationToken);
}