﻿using Spots.Models.ResourceManagement;

namespace Spots.Models.SessionManagement
{
    public static class LocationManager
    {
        public static Location CurrentLocation { get; private set; }

        public static async Task<Location> GetUpdatedLocaionAsync()
        {
            CurrentLocation = await GetLocation();
            return CurrentLocation;
        }

        public static async Task UpdateLocationAsync()
        {
            CurrentLocation = await GetLocation();
        }

        private static async Task<Location> GetLocation()
        {
            Location location;
            try
            {
                location = await Geolocation.Default.GetLastKnownLocationAsync();
            }
            catch (PermissionException ex)
            {
                bool permissionGranted = false;
                while (!permissionGranted)
                {
                    PermissionStatus locationWhenInUse = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                    permissionGranted = locationWhenInUse == PermissionStatus.Granted;
                }
                location = await GetLocation();
            }
            catch
            {
                location = null;
                string[] stringResources = ResourceManagement.ResourceManagement.GetStringResources(Application.Current.Resources, new string[] { "lbl_Error", "lbl_GeolocationError", "lbl_Ok" });
                await Application.Current.MainPage.DisplayAlert(stringResources[0], stringResources[1], stringResources[2]);
            }
            return location;
        }
    }
}