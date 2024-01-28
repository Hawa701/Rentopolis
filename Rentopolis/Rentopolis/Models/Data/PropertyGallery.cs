namespace Rentopolis.Models.Data
{
    public class PropertyGallery
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public Property Property { get; set; }
    }
}
