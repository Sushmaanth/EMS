namespace EMSFrontend.Models
{
    public class DocumentCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string IconClass =>
        Name switch
        {
            "ID Proof" => "bi bi-person-badge",
            "Education" => "bi bi-mortarboard",
            "Employment" => "bi bi-briefcase",
            "Professional" => "bi bi-patch-check",
            "Bank" => "bi bi-bank",
            "Legal" => "bi bi-shield-check",
            "Health"=> "bi bi-shield-plus",
            _ => "bi bi-folder"
        };
    }
}
