namespace Spots;

public class MainFeedDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? tEmptyPraise { get; set; }
    public DataTemplate? tSpotPraise { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return GetDataTemplate((SpotPraise)item);
    }

    private DataTemplate GetDataTemplate(SpotPraise praise)
    {
        if(tEmptyPraise == null
        || tSpotPraise == null)
        {
            return new DataTemplate();
        }

        if (praise.PraiseID.Length > 0)
        {
            return tSpotPraise;
        }

        return tEmptyPraise;
    }
}
