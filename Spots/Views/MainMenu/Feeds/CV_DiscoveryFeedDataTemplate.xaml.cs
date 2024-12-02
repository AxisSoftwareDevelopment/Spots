using Android.Media;

namespace Spots.DiscoveryPage;

public partial class CV_DiscoveryFeedDataTemplate : ContentView
{
	public CV_DiscoveryFeedDataTemplate()
	{
		InitializeComponent();
	}

    private void CV_SearchBarDataTemplate_BindingContextChanged(object? sender, EventArgs e)
    {
        SetBinginds(BindingContext);
    }

    private void SetBinginds(object? Item)
    {
        if (Item?.GetType() == typeof(Client))
        {
            lblMainName.SetBinding(Label.TextProperty, "FullName");
            lblSecondaryName.SetBinding(Label.TextProperty, "Email");
            lblDetail.SetBinding(Label.TextProperty, "Description");
            imgMainImage.SetBinding(Label.TextProperty, "ProfilePictureSource");
        }
        else if (Item?.GetType() == typeof(Spot))
        {
            lblMainName.SetBinding(Label.TextProperty, "SpotName");
            lblSecondaryName.SetBinding(Label.TextProperty, "BrandName");
            lblDetail.SetBinding(Label.TextProperty, "Location.Address");
            imgMainImage.SetBinding(Label.TextProperty, "ProfilePictureSource");
        }
        else if (Item?.GetType() == typeof(SpotPraise))
        {
            lblMainName.SetBinding(Label.TextProperty, "SpotFullName");
            lblSecondaryName.SetBinding(Label.TextProperty, "AuthorFullName");
            lblDetail.SetBinding(Label.TextProperty, "Comment");
            imgMainImage.SetBinding(Label.TextProperty, "SpotProfilePicture");
            imgSecondaryImage.SetBinding(Label.TextProperty, "AuthorProfilePicture");
        }
    }
}