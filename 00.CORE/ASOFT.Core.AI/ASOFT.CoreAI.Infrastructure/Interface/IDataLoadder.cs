using ASOFT.CoreAI.Entities.ViewModels.System;

public interface IDataLoader
{
    Task LoadTrainingDataFromDocument(LoadFileRequest request, CancellationToken cancellationToken);
}