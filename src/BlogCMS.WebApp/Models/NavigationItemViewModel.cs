namespace BlogCMS.WebApp.Models
{
    public class NavigationItemViewModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<NavigationItemViewModel> Children { get; set; } = new List<NavigationItemViewModel>();

        public bool HasChildren { get { return Children.Count > 0; } }
    }
}
