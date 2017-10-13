

namespace Totality.CommonClasses
{
    public enum Mins
    {
        NoOne = -1,
        Industry,
        Finance,
        Military,
        Foreign,
        Media,
        Inner,
        Security,
        Science,
        Premier,
        Secret
    }

    public static class MinsNames
    {
        public static readonly string[] Names = {"Министерство Промышленности", "Министерство Финансов", "Министерство Обороны", "Министерство Иностранных Дел",
            "СМИ", "Министерство Внутренних Дел", "Министерство Государственной безопасности", "Министерство Науки", "Администрацию Премьер-министра" };
    }
}
