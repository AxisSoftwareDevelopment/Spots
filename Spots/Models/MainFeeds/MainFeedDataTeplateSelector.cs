namespace Spots;

public class MainFeedDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? tEmptySpotPraise { get; set; }
    public DataTemplate? tSmallSpotPraise { get; set; }
    public DataTemplate? tLargeSpotPraise { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return GetDataTemplate((SpotPraise)item);
    }

    private DataTemplate GetDataTemplate(SpotPraise praise)
    {
        if(tEmptySpotPraise == null
        || tSmallSpotPraise == null
        || tLargeSpotPraise == null)
        {
            return new DataTemplate();
        }

        DataTemplate retVal = tEmptySpotPraise;

        if (praise.sComment.Length > 0)
        {
            if (praise.dctPictures.Count > 0)
            {
                retVal = tLargeSpotPraise;
            }
            else
            {
                retVal = tSmallSpotPraise;
            }
        }

        return retVal;
    }
}
