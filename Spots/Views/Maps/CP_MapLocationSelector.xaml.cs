using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace eatMeet;

public partial class CP_MapLocationSelector : ContentPage
{
	private Func<MapSpan?> _MapSpanGetter;
    private Action<MapSpan, string> _MapSpanSetter;
    private bool _FieldsEnabled = true;

    public CP_MapLocationSelector(Func<MapSpan?> mapSpanGetter, Action<MapSpan, string> mapSpanSetter, string address = "")
	{
		InitializeComponent();

		_MapSpanGetter = mapSpanGetter;
        _MapSpanSetter = mapSpanSetter;
        _editorAddress.Text = address;

        MapSpan? mapSpan = _MapSpanGetter();
        if(mapSpan == null)
        {
            return;
        }

        _cvMap.MoveToRegion(mapSpan);
        _cvMap.Pins.Clear();
        _cvMap.Pins.Add(new Pin()
        {
            Label = address,
            Location = mapSpan.Center
        });

        _cvMap.MapClicked += _cvMap_MapClicked;
        _btnReturnLocation.Clicked += _btnReturnLocation_Clicked;
	}

    private void _btnReturnLocation_Clicked(object? sender, EventArgs e)
    {
        LockFields();

        if (FieldsAreValid())
        {
            SaveSelectedLocation(_cvMap.Pins[0].Location);
        }
        else
        {
            //TODO: error handling -> Fill correctly all fields
        }

        UnlockFields();
    }

    private void _cvMap_MapClicked(object? sender, Microsoft.Maui.Controls.Maps.MapClickedEventArgs e)
    {
        if(_FieldsEnabled)
        {
            _cvMap.Pins.Clear();
            _cvMap.Pins.Add(new Pin()
            {
                Label = "Selected Location",
                Location = e.Location
            });
        }
    }

    private void SaveSelectedLocation(Location location)
    {
        if(FieldsAreValid())
        {
            _MapSpanSetter(new MapSpan(location, 0.1, 0.1), _editorAddress.Text);
            Navigation.PopAsync();
        }
    }

    private bool FieldsAreValid()
    {
        return _cvMap.Pins.Count == 1
            && _editorAddress.Text.Length > 0;
    }

    private void LockFields()
    {
        _editorAddress.IsEnabled = false;
        _btnReturnLocation.IsEnabled = false;
        _FieldsEnabled = false;   
    }
    
    private void UnlockFields()
    {
        _editorAddress.IsEnabled = true;
        _btnReturnLocation.IsEnabled = true;
        _FieldsEnabled = true;
    }
}