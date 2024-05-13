namespace Spots.SearchBar;

public partial class CV_SearchBarDataTemplate : ContentView
{
	public CV_SearchBarDataTemplate()
	{
        BindingContextChanged += CV_SearchBarDataTemplate_BindingContextChanged;

        InitializeComponent();

        //SetBinginds(BindingContext);
    }

    private void CV_SearchBarDataTemplate_BindingContextChanged(object? sender, EventArgs e)
    {
        SetBinginds(BindingContext);
    }

    private void SetBinginds(object? User)
    {
        IUser? user = User as IUser;
        switch (user?.UserType)
        {
            case EUserType.SPOT:
                {
                    lblMainName.SetBinding(Label.TextProperty, "SpotName");
                    lblSecondaryName.SetBinding(Label.TextProperty, "BrandName");
                    lblDetail.SetBinding(Label.TextProperty, "Location.Address");
                    break;
                }
            case EUserType.CLIENT:
                {
                    Client? client = User as Client;
                    lblMainName.SetBinding(Label.TextProperty, "FullName");
                    lblSecondaryName.SetBinding(Label.TextProperty, "Email");
                    lblDetail.SetBinding(Label.TextProperty, "Description");
                    break;
                }
        }
    }
}