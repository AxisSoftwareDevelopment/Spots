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

    private void SetBinginds(object? Item)
    {

        if(Item?.GetType() == typeof(Client))
        {
            lblMainName.SetBinding(Label.TextProperty, "FullName");
            lblSecondaryName.SetBinding(Label.TextProperty, "Email");
            lblDetail.SetBinding(Label.TextProperty, "Description");
        }
        else if(Item?.GetType() == typeof(Spot))
        {
            lblMainName.SetBinding(Label.TextProperty, "SpotName");
            lblSecondaryName.SetBinding(Label.TextProperty, "BrandName");
            lblDetail.SetBinding(Label.TextProperty, "Location.Address");
        }
    }
}