namespace Lemoo_pos.Models.ViewModels
{
    public class CreateCategoryViewModel
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        public required bool AddProductManual { get; set; }
        public required bool MatchAllCondition { get; set; }

        public List<CreateCategoryCondition>? Conditions { get; set; }
        
    }

    public class CreateCategoryCondition
    {
        public required string ProductProperty { get; set; }

        public required string Condition { get; set; }

        public required string Value { get; set; }
    }
}
