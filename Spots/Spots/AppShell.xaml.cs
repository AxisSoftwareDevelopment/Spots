using Spots.Models.SessionManagement;

namespace Spots;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		BindingContext = CurrentSession.currentUser;
		_ShellContent.Content = new MainPage();
	}
}
