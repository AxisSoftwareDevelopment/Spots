using System.Collections.ObjectModel;

namespace Spots;

public partial class CV_MainFeed : ContentView
{
	public CV_MainFeed()
	{
		InitializeComponent();
		ObservableCollection<SpotPraise> spotPraises = new ObservableCollection<SpotPraise>
		{
			new SpotPraise(),
			new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", "MC Donalds", new DateTimeOffset(),
			comment: "Very Good Burgers!"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", "MC Donalds", new DateTimeOffset(),
            comment: "Very Good Burgers! Here is a picture:", pictureAddress: "placeholder_logo.jpg")
        };

        _colFeed.ItemsSource = spotPraises;
	}
}