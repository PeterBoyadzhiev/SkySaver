namespace SkySaver.Services;

public interface IDialogService
{
    bool Confirm(string title, string message);
}
